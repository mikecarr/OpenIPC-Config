namespace OpenIPC_Config.Services;

public class WfbConfContentUpdatedMessage
{
    public string Content { get; set; }

    public WfbConfContentUpdatedMessage(string content)
    {
        Content = content;
    }
}