using System;
using System.Buffers;
using System.Collections.Generic;
using Microsoft.Azure.Cosmos.Core.Trace;
using Microsoft.Azure.Cosmos.Rntbd;
using Microsoft.Azure.Documents.Collections;

namespace Microsoft.Azure.Documents;

internal abstract class RntbdTokenStream<T> where T : Enum
{
	internal RntbdToken[] tokens;

	private ArrayPool<byte> arrayPool = ArrayPool<byte>.Create();

	private List<byte[]> borrowedBytes = new List<byte[]>();

	public abstract int RequiredTokenCount { get; }

	public byte[] GetBytes(int length)
	{
		byte[] array = arrayPool.Rent(length);
		borrowedBytes.Add(array);
		return array;
	}

	public void Reset()
	{
		for (int i = 0; i < tokens.Length; i++)
		{
			if (tokens[i] != null)
			{
				tokens[i].isPresent = false;
				tokens[i].value.valueBytes = default(ReadOnlyMemory<byte>);
			}
		}
		foreach (byte[] borrowedByte in borrowedBytes)
		{
			arrayPool.Return(borrowedByte);
		}
		borrowedBytes.Clear();
	}

	public int CalculateLength()
	{
		int num = 0;
		RntbdToken[] array = tokens;
		foreach (RntbdToken rntbdToken in array)
		{
			if (rntbdToken != null && rntbdToken.isPresent)
			{
				num++;
				num += 2;
				switch (rntbdToken.GetTokenType())
				{
				case RntbdTokenTypes.Byte:
					num++;
					break;
				case RntbdTokenTypes.UShort:
					num += 2;
					break;
				case RntbdTokenTypes.ULong:
				case RntbdTokenTypes.Long:
					num += 4;
					break;
				case RntbdTokenTypes.ULongLong:
				case RntbdTokenTypes.LongLong:
					num += 8;
					break;
				case RntbdTokenTypes.Float:
					num += 4;
					break;
				case RntbdTokenTypes.Double:
					num += 8;
					break;
				case RntbdTokenTypes.Guid:
					num += 16;
					break;
				case RntbdTokenTypes.SmallString:
				case RntbdTokenTypes.SmallBytes:
					num++;
					num += rntbdToken.value.valueBytes.Length;
					break;
				case RntbdTokenTypes.String:
				case RntbdTokenTypes.Bytes:
					num += 2;
					num += rntbdToken.value.valueBytes.Length;
					break;
				case RntbdTokenTypes.ULongString:
				case RntbdTokenTypes.ULongBytes:
					num += 4;
					num += rntbdToken.value.valueBytes.Length;
					break;
				default:
					throw new BadRequestException();
				}
			}
		}
		return num;
	}

	public void SerializeToBinaryWriter(ref BytesSerializer writer, out int tokensLength)
	{
		tokensLength = 0;
		RntbdToken[] array = tokens;
		foreach (RntbdToken rntbdToken in array)
		{
			if (rntbdToken != null)
			{
				int written = 0;
				rntbdToken.SerializeToBinaryWriter(ref writer, out written);
				tokensLength += written;
			}
		}
	}

	public void ParseFrom(ref BytesDeserializer reader)
	{
		int num = 0;
		while (reader.Position < reader.Length)
		{
			ushort num2 = reader.ReadUInt16();
			RntbdTokenTypes type = (RntbdTokenTypes)reader.ReadByte();
			RntbdToken rntbdToken = ((num2 >= tokens.Length || tokens[num2] == null) ? new RntbdToken(isRequired: false, type, num2) : tokens[num2]);
			if (rntbdToken.isPresent)
			{
				DefaultTrace.TraceError("Duplicate token with identifier {0} type {1} found in RNTBD token stream", rntbdToken.GetTokenIdentifier(), rntbdToken.GetTokenType());
				throw new InternalServerErrorException(RMResources.InternalServerError, GetValidationFailureHeader());
			}
			switch (rntbdToken.GetTokenType())
			{
			case RntbdTokenTypes.Byte:
				rntbdToken.value.valueByte = reader.ReadByte();
				break;
			case RntbdTokenTypes.UShort:
				rntbdToken.value.valueUShort = reader.ReadUInt16();
				break;
			case RntbdTokenTypes.ULong:
				rntbdToken.value.valueULong = reader.ReadUInt32();
				break;
			case RntbdTokenTypes.Long:
				rntbdToken.value.valueLong = reader.ReadInt32();
				break;
			case RntbdTokenTypes.ULongLong:
				rntbdToken.value.valueULongLong = reader.ReadUInt64();
				break;
			case RntbdTokenTypes.LongLong:
				rntbdToken.value.valueLongLong = reader.ReadInt64();
				break;
			case RntbdTokenTypes.Float:
				rntbdToken.value.valueFloat = reader.ReadSingle();
				break;
			case RntbdTokenTypes.Double:
				rntbdToken.value.valueDouble = reader.ReadDouble();
				break;
			case RntbdTokenTypes.Guid:
				rntbdToken.value.valueGuid = reader.ReadGuid();
				break;
			case RntbdTokenTypes.SmallString:
			case RntbdTokenTypes.SmallBytes:
			{
				byte length3 = reader.ReadByte();
				rntbdToken.value.valueBytes = reader.ReadBytes(length3);
				break;
			}
			case RntbdTokenTypes.String:
			case RntbdTokenTypes.Bytes:
			{
				ushort length2 = reader.ReadUInt16();
				rntbdToken.value.valueBytes = reader.ReadBytes(length2);
				break;
			}
			case RntbdTokenTypes.ULongString:
			case RntbdTokenTypes.ULongBytes:
			{
				uint length = reader.ReadUInt32();
				rntbdToken.value.valueBytes = reader.ReadBytes((int)length);
				break;
			}
			default:
				DefaultTrace.TraceError("Unrecognized token type {0} with identifier {1} found in RNTBD token stream", rntbdToken.GetTokenType(), rntbdToken.GetTokenIdentifier());
				throw new InternalServerErrorException(RMResources.InternalServerError, GetValidationFailureHeader());
			}
			rntbdToken.isPresent = true;
			if (rntbdToken.IsRequired())
			{
				num++;
			}
		}
		if (num == RequiredTokenCount)
		{
			return;
		}
		RntbdToken[] array = tokens;
		foreach (RntbdToken rntbdToken2 in array)
		{
			if (rntbdToken2 != null && !rntbdToken2.isPresent && rntbdToken2.IsRequired())
			{
				DefaultTrace.TraceError("Required token with identifier {0} not found in RNTBD token stream", rntbdToken2.GetTokenIdentifier());
				throw new InternalServerErrorException(RMResources.InternalServerError, GetValidationFailureHeader());
			}
		}
	}

	private INameValueCollection GetValidationFailureHeader()
	{
		return new DictionaryNameValueCollection { { "x-ms-request-validation-failure", "1" } };
	}
}
