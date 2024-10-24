namespace OpenIPC_Config.Services;

public class MajesticContentUpdatedMessage
{
    public string Content { get; set; }

    public MajesticContentUpdatedMessage(string content)
    {
        Content = content;
    }
}