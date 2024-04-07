using System.Text;

namespace Microsoft.Azure.Documents;

internal class UserAgentContainer
{
	private static readonly string baseUserAgent;

	private string userAgent;

	private byte[] userAgentUTF8;

	private string suffix;

	private const int maxSuffixLength = 64;

	public string UserAgent => userAgent;

	public byte[] UserAgentUTF8 => userAgentUTF8;

	public string Suffix
	{
		get
		{
			return suffix;
		}
		set
		{
			suffix = value;
			if (suffix.Length > 64)
			{
				suffix = suffix.Substring(0, 64);
			}
			userAgent = BaseUserAgent + suffix;
			userAgentUTF8 = Encoding.UTF8.GetBytes(userAgent);
		}
	}

	internal virtual string BaseUserAgent => baseUserAgent;

	static UserAgentContainer()
	{
		baseUserAgent = CustomTypeExtensions.GenerateBaseUserAgentString();
	}

	public UserAgentContainer()
	{
		userAgent = BaseUserAgent;
		userAgentUTF8 = Encoding.UTF8.GetBytes(BaseUserAgent);
	}

	public UserAgentContainer(string suffix)
		: this()
	{
		Suffix = suffix;
	}
}
