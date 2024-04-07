using System;
using System.Globalization;
using System.Text;

namespace Microsoft.Azure.Documents;

internal sealed class TransportRequestStats
{
	public enum RequestStage
	{
		Created,
		ChannelAcquisitionStarted,
		Pipelined,
		Sent,
		Received,
		Completed,
		Failed
	}

	private const string RequestStageCreated = "Created";

	private const string RequestStageChannelAcquisitionStarted = "ChannelAcquisitionStarted";

	private const string RequestStagePipelined = "Pipelined";

	private const string RequestStageSent = "Transit Time";

	private const string RequestStageReceived = "Received";

	private const string RequestStageCompleted = "Completed";

	private const string RequestStageFailed = "Failed";

	private readonly ValueStopwatch stopwatch;

	private readonly DateTime requestCreatedTime;

	private TimeSpan? channelAcquisitionStartedTime;

	private TimeSpan? requestPipelinedTime;

	private TimeSpan? requestSentTime;

	private TimeSpan? requestReceivedTime;

	private TimeSpan? requestCompletedTime;

	private TimeSpan? requestFailedTime;

	public RequestStage CurrentStage { get; private set; }

	public long? RequestSizeInBytes { get; set; }

	public long? RequestBodySizeInBytes { get; set; }

	public long? ResponseMetadataSizeInBytes { get; set; }

	public long? ResponseBodySizeInBytes { get; set; }

	public int? NumberOfInflightRequestsToEndpoint { get; set; }

	public int? NumberOfOpenConnectionsToEndpoint { get; set; }

	public bool? RequestWaitingForConnectionInitialization { get; set; }

	public int? NumberOfInflightRequestsInConnection { get; set; }

	public DateTime? ConnectionLastSendAttemptTime { get; set; }

	public DateTime? ConnectionLastSendTime { get; set; }

	public DateTime? ConnectionLastReceiveTime { get; set; }

	public TransportRequestStats()
	{
		CurrentStage = RequestStage.Created;
		requestCreatedTime = DateTime.UtcNow;
		stopwatch = ValueStopwatch.StartNew();
	}

	public void RecordState(RequestStage requestStage)
	{
		TimeSpan elapsed = stopwatch.Elapsed;
		switch (requestStage)
		{
		case RequestStage.ChannelAcquisitionStarted:
			channelAcquisitionStartedTime = elapsed;
			CurrentStage = RequestStage.ChannelAcquisitionStarted;
			break;
		case RequestStage.Pipelined:
			requestPipelinedTime = elapsed;
			CurrentStage = RequestStage.Pipelined;
			break;
		case RequestStage.Sent:
			requestSentTime = elapsed;
			CurrentStage = RequestStage.Sent;
			break;
		case RequestStage.Received:
			requestReceivedTime = elapsed;
			CurrentStage = RequestStage.Received;
			break;
		case RequestStage.Completed:
			requestCompletedTime = elapsed;
			CurrentStage = RequestStage.Completed;
			break;
		case RequestStage.Failed:
			requestFailedTime = elapsed;
			CurrentStage = RequestStage.Failed;
			break;
		default:
			throw new InvalidOperationException($"No transition to {requestStage}");
		}
	}

	public override string ToString()
	{
		StringBuilder stringBuilder = new StringBuilder();
		AppendJsonString(stringBuilder);
		return stringBuilder.ToString();
	}

	public void AppendJsonString(StringBuilder stringBuilder)
	{
		stringBuilder.Append("{\"requestTimeline\":[");
		AppendRequestStats(stringBuilder, "Created", requestCreatedTime, TimeSpan.Zero, channelAcquisitionStartedTime, requestFailedTime);
		if (channelAcquisitionStartedTime.HasValue)
		{
			stringBuilder.Append(",");
			AppendRequestStats(stringBuilder, "ChannelAcquisitionStarted", requestCreatedTime, channelAcquisitionStartedTime.Value, requestPipelinedTime, requestFailedTime);
		}
		if (requestPipelinedTime.HasValue)
		{
			stringBuilder.Append(",");
			AppendRequestStats(stringBuilder, "Pipelined", requestCreatedTime, requestPipelinedTime.Value, requestSentTime, requestFailedTime);
		}
		if (requestSentTime.HasValue)
		{
			stringBuilder.Append(",");
			AppendRequestStats(stringBuilder, "Transit Time", requestCreatedTime, requestSentTime.Value, requestReceivedTime, requestFailedTime);
		}
		if (requestReceivedTime.HasValue)
		{
			stringBuilder.Append(",");
			AppendRequestStats(stringBuilder, "Received", requestCreatedTime, requestReceivedTime.Value, requestCompletedTime, requestFailedTime);
		}
		if (requestCompletedTime.HasValue)
		{
			stringBuilder.Append(",");
			AppendRequestStats(stringBuilder, "Completed", requestCreatedTime, requestCompletedTime.Value, requestCompletedTime, requestFailedTime);
		}
		if (requestFailedTime.HasValue)
		{
			stringBuilder.Append(",");
			AppendRequestStats(stringBuilder, "Failed", requestCreatedTime, requestFailedTime.Value, requestFailedTime, requestFailedTime);
		}
		stringBuilder.Append("]");
		AppendServiceEndpointStats(stringBuilder);
		AppendConnectionStats(stringBuilder);
		if (RequestSizeInBytes.HasValue)
		{
			stringBuilder.Append(",\"requestSizeInBytes\":");
			stringBuilder.Append(RequestSizeInBytes.Value.ToString(CultureInfo.InvariantCulture));
		}
		if (RequestBodySizeInBytes.HasValue)
		{
			stringBuilder.Append(",\"requestBodySizeInBytes\":");
			stringBuilder.Append(RequestBodySizeInBytes.Value.ToString(CultureInfo.InvariantCulture));
		}
		if (ResponseMetadataSizeInBytes.HasValue)
		{
			stringBuilder.Append(",\"responseMetadataSizeInBytes\":");
			stringBuilder.Append(ResponseMetadataSizeInBytes.Value.ToString(CultureInfo.InvariantCulture));
		}
		if (ResponseBodySizeInBytes.HasValue)
		{
			stringBuilder.Append(",\"responseBodySizeInBytes\":");
			stringBuilder.Append(ResponseBodySizeInBytes.Value.ToString(CultureInfo.InvariantCulture));
		}
		stringBuilder.Append("}");
	}

	private void AppendServiceEndpointStats(StringBuilder stringBuilder)
	{
		stringBuilder.Append(",\"serviceEndpointStats\":");
		stringBuilder.Append("{");
		if (NumberOfInflightRequestsToEndpoint.HasValue)
		{
			stringBuilder.Append("\"inflightRequests\":");
			stringBuilder.Append(NumberOfInflightRequestsToEndpoint.Value.ToString(CultureInfo.InvariantCulture));
		}
		if (NumberOfOpenConnectionsToEndpoint.HasValue)
		{
			stringBuilder.Append(",\"openConnections\":");
			stringBuilder.Append(NumberOfOpenConnectionsToEndpoint.Value.ToString(CultureInfo.InvariantCulture));
		}
		stringBuilder.Append("}");
	}

	private void AppendConnectionStats(StringBuilder stringBuilder)
	{
		stringBuilder.Append(",\"connectionStats\":");
		stringBuilder.Append("{");
		if (RequestWaitingForConnectionInitialization.HasValue)
		{
			stringBuilder.Append("\"waitforConnectionInit\":\"");
			stringBuilder.Append(RequestWaitingForConnectionInitialization.Value.ToString());
			stringBuilder.Append("\"");
		}
		if (NumberOfInflightRequestsInConnection.HasValue)
		{
			stringBuilder.Append(",\"callsPendingReceive\":");
			stringBuilder.Append(NumberOfInflightRequestsInConnection.Value.ToString(CultureInfo.InvariantCulture));
		}
		if (ConnectionLastSendAttemptTime.HasValue)
		{
			stringBuilder.Append(",\"lastSendAttempt\":\"");
			stringBuilder.Append(ConnectionLastSendAttemptTime.Value.ToString("o", CultureInfo.InvariantCulture));
			stringBuilder.Append("\"");
		}
		if (ConnectionLastSendTime.HasValue)
		{
			stringBuilder.Append(",\"lastSend\":\"");
			stringBuilder.Append(ConnectionLastSendTime.Value.ToString("o", CultureInfo.InvariantCulture));
			stringBuilder.Append("\"");
		}
		if (ConnectionLastReceiveTime.HasValue)
		{
			stringBuilder.Append(",\"lastReceive\":\"");
			stringBuilder.Append(ConnectionLastReceiveTime.Value.ToString("o", CultureInfo.InvariantCulture));
			stringBuilder.Append("\"");
		}
		stringBuilder.Append("}");
	}

	private static void AppendRequestStats(StringBuilder stringBuilder, string eventName, DateTime requestStartTime, TimeSpan startTime, TimeSpan? endTime, TimeSpan? failedTime)
	{
		stringBuilder.Append("{\"event\": \"");
		stringBuilder.Append(eventName);
		stringBuilder.Append("\", \"startTimeUtc\": \"");
		stringBuilder.Append((requestStartTime + startTime).ToString("o", CultureInfo.InvariantCulture));
		stringBuilder.Append("\", \"durationInMs\": ");
		TimeSpan? timeSpan = endTime ?? failedTime;
		if (timeSpan.HasValue)
		{
			stringBuilder.Append((timeSpan.Value - startTime).TotalMilliseconds.ToString(CultureInfo.InvariantCulture));
		}
		else
		{
			stringBuilder.Append("\"Not Set\"");
		}
		stringBuilder.Append("}");
	}
}
