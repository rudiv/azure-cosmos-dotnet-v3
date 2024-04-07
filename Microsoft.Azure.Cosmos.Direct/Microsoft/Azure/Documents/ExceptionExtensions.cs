using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;

namespace Microsoft.Azure.Documents;

internal static class ExceptionExtensions
{
	public static string ToLoggingString(this Exception exception)
	{
		IEnumerable<string> values = from property in exception.GetType().GetProperties()
			select new
			{
				Name = property.Name,
				Value = property.GetValue(exception, null)
			} into x
			select string.Format(CultureInfo.InvariantCulture, "{0}:{1}", x.Name, (x.Value != null) ? x.Value.ToString() : string.Empty);
		return string.Concat(exception.GetType(), " : ", string.Join(",", values));
	}

	public static string ToStringWithData(this Exception exception)
	{
		StringBuilder stringBuilder = new StringBuilder(exception.ToString());
		List<string> list = new List<string>();
		CaptureExceptionData(exception, list);
		if (list.Count() > 0)
		{
			stringBuilder.Append(Environment.NewLine);
			stringBuilder.Append("AdditionalData:");
			foreach (string item in list)
			{
				stringBuilder.Append(Environment.NewLine);
				stringBuilder.Append(item);
			}
		}
		return stringBuilder.ToString();
	}

	public static string ToStringWithMessageAndData(this Exception exception)
	{
		StringBuilder stringBuilder = new StringBuilder(exception.Message);
		List<string> list = new List<string>();
		CaptureExceptionData(exception, list);
		if (list.Count() > 0)
		{
			stringBuilder.Append(Environment.NewLine);
			stringBuilder.Append("AdditionalData:");
			foreach (string item in list)
			{
				stringBuilder.Append(Environment.NewLine);
				stringBuilder.Append(item);
			}
		}
		return stringBuilder.ToString();
	}

	public static DocumentClientException GetTranslatedStoredProcedureException(DocumentClientException dce)
	{
		if (dce == null)
		{
			return dce;
		}
		if (dce.StatusCode.HasValue)
		{
			SubStatusCodes subStatus = dce.GetSubStatus();
			if (dce.StatusCode.Value == HttpStatusCode.BadRequest)
			{
				return (HttpStatusCode)subStatus switch
				{
					HttpStatusCode.BadRequest => new BadRequestException(dce.Message), 
					HttpStatusCode.Forbidden => new ForbiddenException(dce.Message), 
					HttpStatusCode.NotFound => new NotFoundException(dce.Message), 
					HttpStatusCode.RequestTimeout => new RequestTimeoutException(dce.Message), 
					HttpStatusCode.Conflict => new ConflictException(dce.Message), 
					HttpStatusCode.PreconditionFailed => new PreconditionFailedException(dce.Message), 
					HttpStatusCode.RequestEntityTooLarge => new RequestEntityTooLargeException(dce.Message), 
					(HttpStatusCode)449 => new RetryWithException(dce.Message), 
					(HttpStatusCode)1004 => new NotFoundException(dce, SubStatusCodes.CrossPartitionQueryNotServable), 
					(HttpStatusCode)3207 => new ConflictException(dce.Message, SubStatusCodes.ConfigurationNameAlreadyExists), 
					(HttpStatusCode)3001 => new InternalServerErrorException(dce.Message), 
					HttpStatusCode.ServiceUnavailable => new ServiceUnavailableException(dce.Message, SubStatusCodes.Unknown), 
					HttpStatusCode.Gone => new GoneException(dce.Message, SubStatusCodes.Unknown), 
					_ => dce, 
				};
			}
			return dce;
		}
		return dce;
	}

	private static void CaptureExceptionData(Exception exception, List<string> exceptionData)
	{
		if (exception.Data != null && exception.Data.Count > 0)
		{
			foreach (object key in exception.Data.Keys)
			{
				exceptionData.Add(string.Format(CultureInfo.InvariantCulture, "{0}: {1}", key.ToString(), exception.Data[key].ToString()));
			}
		}
		if (exception.InnerException != null)
		{
			CaptureExceptionData(exception.InnerException, exceptionData);
		}
	}
}
