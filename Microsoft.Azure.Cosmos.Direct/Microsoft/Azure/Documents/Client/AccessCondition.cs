namespace Microsoft.Azure.Documents.Client;

internal sealed class AccessCondition
{
	public AccessConditionType Type { get; set; }

	public string Condition { get; set; }
}
