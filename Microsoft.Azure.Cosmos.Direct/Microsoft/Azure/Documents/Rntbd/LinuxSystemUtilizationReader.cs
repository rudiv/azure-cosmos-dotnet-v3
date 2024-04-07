using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.Azure.Cosmos.Core.Trace;

namespace Microsoft.Azure.Documents.Rntbd;

internal sealed class LinuxSystemUtilizationReader : SystemUtilizationReaderBase
{
	private class ProcStatFileParser
	{
		private const string cpuPrefixFirstLine = "cpu";

		private const string DefaultProcStatFilePath = "/proc/stat";

		private readonly string procStatFilePath;

		private readonly ReusableTextReader reusableReader;

		public ProcStatFileParser()
			: this("/proc/stat")
		{
		}

		internal ProcStatFileParser(string procStatFilePath)
		{
			if (string.IsNullOrWhiteSpace(procStatFilePath))
			{
				throw new ArgumentNullException("procStatFilePath");
			}
			reusableReader = new ReusableTextReader(Encoding.UTF8, 256);
			this.procStatFilePath = procStatFilePath;
		}

		public bool TryParseStatFile(out ulong userJiffiesElaped, out ulong kernelJiffiesElapsed, out ulong idleJiffiesElapsed, out ulong otherJiffiesElapsed)
		{
			userJiffiesElaped = 0uL;
			kernelJiffiesElapsed = 0uL;
			idleJiffiesElapsed = 0uL;
			otherJiffiesElapsed = 0uL;
			if (!TryReadProcStatFirstLine(reusableReader, out var firstLine))
			{
				return false;
			}
			try
			{
				StringParser stringParser = new StringParser(firstLine, ' ', skipEmpty: true);
				string value = stringParser.MoveAndExtractNext();
				if (!"cpu".Equals(value, StringComparison.OrdinalIgnoreCase))
				{
					DefaultTrace.TraceCritical("Unexpected procfs/cpu-file format. '$" + firstLine + "'");
					return false;
				}
				ulong num = stringParser.ParseNextUInt64();
				ulong num2 = stringParser.ParseNextUInt64();
				ulong num3 = stringParser.ParseNextUInt64();
				ulong num4 = stringParser.ParseNextUInt64();
				ulong num5 = 0uL;
				while (stringParser.HasNext)
				{
					num5 += stringParser.ParseNextUInt64();
				}
				userJiffiesElaped = num + num2;
				kernelJiffiesElapsed = num3;
				idleJiffiesElapsed = num4;
				otherJiffiesElapsed = num5;
				return true;
			}
			catch (InvalidDataException)
			{
				return false;
			}
		}

		private bool TryReadProcStatFirstLine(ReusableTextReader reusableReader, out string firstLine)
		{
			try
			{
				using FileStream source = new FileStream(procStatFilePath, FileMode.Open, FileAccess.Read, FileShare.Read, 1, useAsync: false);
				firstLine = reusableReader.ReadJustFirstLine(source);
				return true;
			}
			catch (IOException ex)
			{
				DefaultTrace.TraceError(ex.Message);
				firstLine = null;
				return false;
			}
		}
	}

	private class ProcMemInfoFileParser
	{
		private const string DefaultProcMemInfoFilePath = "/proc/meminfo";

		private readonly string procMemInfoFilePath;

		private readonly ReusableTextReader reusableReader;

		public ProcMemInfoFileParser()
			: this("/proc/meminfo")
		{
		}

		internal ProcMemInfoFileParser(string procMemInfoFilePath)
		{
			if (string.IsNullOrWhiteSpace(procMemInfoFilePath))
			{
				throw new ArgumentNullException("procMemInfoFilePath");
			}
			reusableReader = new ReusableTextReader(Encoding.UTF8, 256);
			this.procMemInfoFilePath = procMemInfoFilePath;
		}

		public bool TryParseMemInfoFile(out long? freeMemory, out long? availableMemory)
		{
			freeMemory = null;
			availableMemory = null;
			if (!TryReadProcMemInfo(reusableReader, out var data))
			{
				DefaultTrace.TraceCritical("Not able to read memory information from /proc/meminfo");
				return false;
			}
			try
			{
				foreach (string item in data)
				{
					StringParser stringParser = new StringParser(item, ' ', skipEmpty: true);
					if (stringParser.MoveAndExtractNext().Contains("MemFree"))
					{
						freeMemory = (long)stringParser.ParseNextUInt64();
					}
					else if (item.Contains("MemAvailable"))
					{
						availableMemory = (long)stringParser.ParseNextUInt64();
					}
				}
				if (!freeMemory.HasValue && !availableMemory.HasValue)
				{
					throw new InvalidDataException("Free Memory and Available Memory information is not available.");
				}
				return true;
			}
			catch (InvalidDataException ex)
			{
				DefaultTrace.TraceError(ex.Message);
				return false;
			}
		}

		private bool TryReadProcMemInfo(ReusableTextReader reusableReader, out List<string> data)
		{
			try
			{
				using FileStream source = new FileStream(procMemInfoFilePath, FileMode.Open, FileAccess.Read, FileShare.Read, 1, useAsync: false);
				data = reusableReader.ReadMultipleLines(source);
				return true;
			}
			catch (IOException ex)
			{
				DefaultTrace.TraceError(ex.Message);
				data = null;
				return false;
			}
		}
	}

	private struct StringParser
	{
		private readonly string buffer;

		private readonly char separator;

		private readonly bool skipEmpty;

		private int endIndex;

		private int startIndex;

		public bool HasNext => endIndex < buffer.Length;

		public StringParser(string buffer, char separator, bool skipEmpty = false)
		{
			if (buffer == null)
			{
				throw new ArgumentNullException("buffer");
			}
			this.buffer = buffer;
			this.separator = separator;
			this.skipEmpty = skipEmpty;
			startIndex = -1;
			endIndex = -1;
		}

		public string ExtractCurrent()
		{
			if (buffer == null || startIndex == -1)
			{
				throw new InvalidOperationException();
			}
			return buffer.Substring(startIndex, endIndex - startIndex);
		}

		public string MoveAndExtractNext()
		{
			MoveNextOrFail();
			return buffer.Substring(startIndex, endIndex - startIndex);
		}

		public bool MoveNext()
		{
			if (buffer == null)
			{
				throw new InvalidOperationException();
			}
			do
			{
				if (endIndex >= buffer.Length)
				{
					startIndex = endIndex;
					return false;
				}
				int num = buffer.IndexOf(separator, endIndex + 1);
				startIndex = endIndex + 1;
				endIndex = ((num >= 0) ? num : buffer.Length);
			}
			while (skipEmpty && endIndex < startIndex + 1);
			return true;
		}

		public void MoveNextOrFail()
		{
			if (!MoveNext())
			{
				ThrowForInvalidData();
			}
		}

		public unsafe ulong ParseNextUInt64()
		{
			MoveNextOrFail();
			ulong num = 0uL;
			fixed (char* ptr = buffer)
			{
				char* ptr2 = ptr + startIndex;
				for (char* ptr3 = ptr + endIndex; ptr2 != ptr3; ptr2++)
				{
					int num2 = *ptr2 - 48;
					if (num2 < 0 || num2 > 9)
					{
						ThrowForInvalidData();
					}
					num = checked(num * 10 + (ulong)num2);
				}
			}
			return num;
		}

		private static void ThrowForInvalidData()
		{
			throw new InvalidDataException();
		}
	}

	private sealed class ReusableTextReader
	{
		private static readonly char[] lineBreakChars = Environment.NewLine.ToCharArray();

		private readonly StringBuilder builder;

		private readonly byte[] bytes;

		private readonly char[] chars;

		private readonly Decoder decoder;

		private readonly Encoding encoding;

		public ReusableTextReader(Encoding encoding = null, int bufferSize = 1024)
		{
			if (encoding == null)
			{
				this.encoding = Encoding.UTF8;
			}
			else
			{
				this.encoding = encoding;
			}
			builder = new StringBuilder();
			decoder = encoding.GetDecoder();
			bytes = new byte[bufferSize];
			chars = new char[encoding.GetMaxCharCount(bytes.Length)];
		}

		public string ReadJustFirstLine(Stream source)
		{
			int byteCount;
			while ((byteCount = source.Read(bytes, 0, bytes.Length)) != 0)
			{
				int num = decoder.GetChars(bytes, 0, byteCount, chars, 0);
				int num2 = -1;
				for (int i = 0; i < num; i++)
				{
					if (lineBreakChars.Contains(chars[i]))
					{
						num2 = i;
						break;
					}
				}
				if (num2 < 0)
				{
					builder.Append(chars, 0, num);
					continue;
				}
				builder.Append(chars, 0, num2);
				break;
			}
			string result = builder.ToString();
			builder.Clear();
			decoder.Reset();
			return result;
		}

		public List<string> ReadMultipleLines(Stream source)
		{
			List<string> list = new List<string>();
			using StreamReader streamReader = new StreamReader(source, encoding);
			string item;
			while ((item = streamReader.ReadLine()) != null)
			{
				list.Add(item);
			}
			return list;
		}
	}

	private readonly ProcStatFileParser procStatFileParser;

	private readonly ProcMemInfoFileParser procMemInfoFileParser;

	private ulong lastIdleJiffies;

	private ulong lastKernelJiffies;

	private ulong lastOtherJiffies;

	private ulong lastUserJiffies;

	public LinuxSystemUtilizationReader()
		: this(null, null)
	{
	}

	internal LinuxSystemUtilizationReader(string procStatFilePath, string procMemoryInfoPath)
	{
		procStatFileParser = (string.IsNullOrWhiteSpace(procStatFilePath) ? new ProcStatFileParser() : new ProcStatFileParser(procStatFilePath));
		procMemInfoFileParser = (string.IsNullOrWhiteSpace(procMemoryInfoPath) ? new ProcMemInfoFileParser() : new ProcMemInfoFileParser(procMemoryInfoPath));
		lastIdleJiffies = 0uL;
		lastKernelJiffies = 0uL;
		lastOtherJiffies = 0uL;
		lastUserJiffies = 0uL;
	}

	protected override float GetSystemWideCpuUsageCore()
	{
		if (!procStatFileParser.TryParseStatFile(out var userJiffiesElaped, out var kernelJiffiesElapsed, out var idleJiffiesElapsed, out var otherJiffiesElapsed))
		{
			return float.NaN;
		}
		float result = 0f;
		if (lastIdleJiffies != 0L)
		{
			ulong num = kernelJiffiesElapsed - lastKernelJiffies;
			ulong num2 = userJiffiesElaped - lastUserJiffies;
			ulong num3 = num + num2 + otherJiffiesElapsed - lastOtherJiffies;
			ulong num4 = num3 + idleJiffiesElapsed - lastIdleJiffies;
			if (num4 == 0L)
			{
				return float.NaN;
			}
			result = 100f * ((float)num3 / (float)num4);
		}
		lastUserJiffies = userJiffiesElaped;
		lastKernelJiffies = kernelJiffiesElapsed;
		lastIdleJiffies = idleJiffiesElapsed;
		lastOtherJiffies = otherJiffiesElapsed;
		return result;
	}

	protected override long? GetSystemWideMemoryAvailabiltyCore()
	{
		if (!procMemInfoFileParser.TryParseMemInfoFile(out var freeMemory, out var availableMemory))
		{
			return null;
		}
		return availableMemory ?? freeMemory;
	}
}
