using System;
using System.Collections.Generic;

namespace Microsoft.Azure.Documents;

internal static class OperationTypeExtensions
{
	private static readonly Dictionary<int, string> OperationTypeNames;

	static OperationTypeExtensions()
	{
		OperationTypeNames = new Dictionary<int, string>();
		foreach (OperationType value in Enum.GetValues(typeof(OperationType)))
		{
			OperationTypeNames[(int)value] = value.ToString();
		}
	}

	public static string ToOperationTypeString(this OperationType type)
	{
		return OperationTypeNames[(int)type];
	}

	public static bool IsWriteOperation(this OperationType type)
	{
		if (type != 0 && type != OperationType.Patch && type != OperationType.Delete && type != OperationType.Replace && type != OperationType.ExecuteJavaScript && type != OperationType.BatchApply && type != OperationType.Batch && type != OperationType.Upsert)
		{
			return type == OperationType.CompleteUserTransaction;
		}
		return true;
	}

	public static bool IsPointOperation(this OperationType type)
	{
		if (type != 0 && type != OperationType.Delete && type != OperationType.Read && type != OperationType.Patch && type != OperationType.Upsert)
		{
			return type == OperationType.Replace;
		}
		return true;
	}

	public static bool IsReadOperation(this OperationType type)
	{
		if (type != OperationType.Read && type != OperationType.ReadFeed && type != OperationType.Query && type != OperationType.SqlQuery && type != OperationType.Head && type != OperationType.HeadFeed && type != OperationType.MetadataCheckAccess)
		{
			return type == OperationType.QueryPlan;
		}
		return true;
	}
}
