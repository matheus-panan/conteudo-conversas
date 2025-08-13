namespace painel_conversas.Models;

public class Chat
{
    public string Id { get; set; }
    public string IdChat { get; set; }
    public string Text { get; set; }
    public string Type { get; set; }
    public string Origin { get; set; }
    public string SenderId { get; set; }
    public string ConversationId { get; set; }
    public long CreatedAt { get; set; }
    public object Attachment { get; set; }
    
    // Propriedades adicionais para exibir informações do remetente
    public string SenderName { get; set; }
    public string SenderType { get; set; } // "Cliente", "Atendente", "Sistema"
}