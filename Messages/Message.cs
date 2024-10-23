namespace OpenIPC_Config.Messages;

public class Message
{
    public string Text { get; set; }

    public Message(string text)
    {
        Text = text;
    }

}