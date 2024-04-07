using System;
using System.Diagnostics;
using System.Globalization;

namespace Microsoft.Azure.Documents.Rntbd;

internal static class TransportExceptions
{
	internal static string LocalIpv4Address;

	private static bool AddSourceIpAddressInNetworkExceptionMessagePrivate;

	public static bool AddSourceIpAddressInNetworkExceptionMessage
	{
		get
		{
			return AddSourceIpAddressInNetworkExceptionMessagePrivate;
		}
		set
		{
			if (value && !AddSourceIpAddressInNetworkExceptionMessagePrivate)
			{
				LocalIpv4Address = NetUtil.GetNonLoopbackIpV4Address() ?? string.Empty;
			}
			AddSourceIpAddressInNetworkExceptionMessagePrivate = value;
		}
	}

	internal static GoneException GetGoneException(Uri targetAddress, Guid activityId, Exception inner = null, TransportRequestStats transportRequestStats = null)
	{
		Trace.CorrelationManager.ActivityId = activityId;
		GoneException ex = ((inner == null) ? ((!AddSourceIpAddressInNetworkExceptionMessage) ? new GoneException(string.Format(CultureInfo.CurrentUICulture, RMResources.ExceptionMessage, RMResources.Gone), inner, SubStatusCodes.TransportGenerated410, targetAddress) : new GoneException(string.Format(CultureInfo.CurrentUICulture, RMResources.ExceptionMessage, RMResources.Gone), inner, SubStatusCodes.TransportGenerated410, targetAddress, LocalIpv4Address)) : ((!AddSourceIpAddressInNetworkExceptionMessage) ? new GoneException(string.Format(CultureInfo.CurrentUICulture, RMResources.ExceptionMessage, RMResources.Gone), inner, SubStatusCodes.TransportGenerated410, targetAddress) : new GoneException(string.Format(CultureInfo.CurrentUICulture, RMResources.ExceptionMessage, RMResources.Gone), inner, SubStatusCodes.TransportGenerated410, targetAddress, LocalIpv4Address)));
		ex.Headers.Set("x-ms-activity-id", activityId.ToString());
		ex.TransportRequestStats = transportRequestStats;
		return ex;
	}

	internal static RequestTimeoutException GetRequestTimeoutException(Uri targetAddress, Guid activityId, Exception inner = null, TransportRequestStats transportRequestStats = null)
	{
		Trace.CorrelationManager.ActivityId = activityId;
		RequestTimeoutException ex = ((inner == null) ? ((!AddSourceIpAddressInNetworkExceptionMessage) ? new RequestTimeoutException(string.Format(CultureInfo.CurrentUICulture, RMResources.ExceptionMessage, RMResources.RequestTimeout), inner, targetAddress) : new RequestTimeoutException(string.Format(CultureInfo.CurrentUICulture, RMResources.ExceptionMessage, RMResources.RequestTimeout), inner, targetAddress, LocalIpv4Address)) : ((!AddSourceIpAddressInNetworkExceptionMessage) ? new RequestTimeoutException(string.Format(CultureInfo.CurrentUICulture, RMResources.ExceptionMessage, RMResources.RequestTimeout), inner, targetAddress) : new RequestTimeoutException(string.Format(CultureInfo.CurrentUICulture, RMResources.ExceptionMessage, RMResources.RequestTimeout), inner, targetAddress, LocalIpv4Address)));
		ex.Headers.Add("x-ms-request-validation-failure", "1");
		ex.TransportRequestStats = transportRequestStats;
		return ex;
	}

	internal static ServiceUnavailableException GetServiceUnavailableException(Uri targetAddress, Guid activityId, Exception inner = null, TransportRequestStats transportRequestStats = null)
	{
		Trace.CorrelationManager.ActivityId = activityId;
		ServiceUnavailableException ex = ((inner != null) ? ServiceUnavailableException.Create(SubStatusCodes.Channel_Closed, inner, null, targetAddress) : ServiceUnavailableException.Create(SubStatusCodes.Channel_Closed, null, null, targetAddress));
		ex.Headers.Add("x-ms-request-validation-failure", "1");
		ex.TransportRequestStats = transportRequestStats;
		return ex;
	}

	internal static InternalServerErrorException GetInternalServerErrorException(Uri targetAddress, Guid activityId, Exception inner = null)
	{
		Trace.CorrelationManager.ActivityId = activityId;
		InternalServerErrorException ex = ((inner != null) ? new InternalServerErrorException(string.Format(CultureInfo.CurrentUICulture, RMResources.ExceptionMessage, RMResources.ChannelClosed), inner, targetAddress) : new InternalServerErrorException(string.Format(CultureInfo.CurrentUICulture, RMResources.ExceptionMessage, RMResources.ChannelClosed), targetAddress));
		ex.Headers.Add("x-ms-request-validation-failure", "1");
		return ex;
	}

	internal static InternalServerErrorException GetInternalServerErrorException(Uri targetAddress, string exceptionMessage)
	{
		return new InternalServerErrorException(string.Format(CultureInfo.CurrentUICulture, RMResources.ExceptionMessage, exceptionMessage), targetAddress)
		{
			Headers = { { "x-ms-request-validation-failure", "1" } }
		};
	}
}
