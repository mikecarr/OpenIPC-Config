namespace OpenIPC_Config.Services;

public class TelemetryContentUpdatedMessage
{
    public string Content { get; set; }

    public TelemetryContentUpdatedMessage(string content)
    {
        Content = content;
    }
}