using System;

namespace Microsoft.Azure.Documents;

internal readonly struct StringSegment
{
	private readonly string value;

	private int Start { get; }

	public int Length { get; }

	public StringSegment(string value)
	{
		this.value = value;
		Start = 0;
		Length = value?.Length ?? 0;
	}

	public StringSegment(string value, int start, int length)
	{
		if (value == null)
		{
			throw new ArgumentNullException("value");
		}
		if (start < 0 || (start >= value.Length && value.Length > 0))
		{
			throw new ArgumentException("start");
		}
		if (length < 0 || start + length > value.Length)
		{
			throw new ArgumentException("length");
		}
		this.value = value;
		Start = start;
		Length = length;
	}

	public static implicit operator StringSegment(string b)
	{
		return new StringSegment(b);
	}

	public bool IsNullOrEmpty()
	{
		if (!string.IsNullOrEmpty(value))
		{
			return Length == 0;
		}
		return true;
	}

	public int Compare(string other, StringComparison comparison)
	{
		return string.Compare(value, Start, other, 0, Math.Max(Length, other.Length), comparison);
	}

	public int Compare(StringSegment other, StringComparison comparison)
	{
		return string.Compare(value, Start, other.value, other.Start, Math.Max(Length, other.Length), comparison);
	}

	public bool Equals(string other, StringComparison comparison)
	{
		return Compare(other, comparison) == 0;
	}

	public StringSegment Substring(int start, int length)
	{
		if (length == 0)
		{
			return new StringSegment(string.Empty);
		}
		if (start > Length)
		{
			throw new ArgumentException("start");
		}
		if (start + length > Length)
		{
			throw new ArgumentException("length");
		}
		return new StringSegment(value, start + Start, length);
	}

	public int LastIndexOf(char segment)
	{
		if (IsNullOrEmpty())
		{
			return -1;
		}
		int num = value.LastIndexOf(segment, Start + Length - 1);
		if (num >= 0)
		{
			return num - Start;
		}
		return num;
	}

	public StringSegment Trim(char[] trimChars)
	{
		return TrimStart(trimChars).TrimEnd(trimChars);
	}

	public StringSegment TrimStart(char[] trimChars)
	{
		if (Length == 0)
		{
			return new StringSegment(string.Empty, 0, 0);
		}
		int num = Start;
		int num2 = Length;
		while (num2 > 0 && value.IndexOfAny(trimChars, num, 1) == num)
		{
			num++;
			num2--;
		}
		return new StringSegment(value, num, num2);
	}

	public StringSegment TrimEnd(char[] trimChars)
	{
		if (Length == 0)
		{
			return new StringSegment(string.Empty, 0, 0);
		}
		int num = Length;
		int num2 = Start + Length - 1;
		while (num > 0 && value.LastIndexOfAny(trimChars, num2, 1) == num2)
		{
			num--;
			num2--;
		}
		return new StringSegment(value, Start, num);
	}

	public string GetString()
	{
		if (Length == 0)
		{
			return string.Empty;
		}
		if (Length == value.Length)
		{
			return value;
		}
		return value.Substring(Start, Length);
	}
}
