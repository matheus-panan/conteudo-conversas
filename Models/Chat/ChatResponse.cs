namespace painel_conversas.Models;

public class ChatResponse
{
    public bool Success { get; set; }
    public List<Chat> Data { get; set; }
    public object Error { get; set; }
}
