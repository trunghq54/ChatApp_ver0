using ChatClient;

[Serializable]
public class Message
{
    public MessageType Type { get; set; }
    public string Content { get; set; }
    public byte[] FileData { get; set; }
    public string FileName { get; set; }
}