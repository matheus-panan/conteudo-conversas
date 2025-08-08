namespace painel_conversas.Models;

public class ContactItems
{
    public string id { get; set; }
    public string name { get; set; }
    public string chatbotId { get; set; }
    public string channelId { get; set; }
    public string channelType { get; set; }
    public string phone { get; set; }
    public string email { get; set; }
    public int gender { get; set; }
    public DateTime dateCreate { get; set; }
    public DateTime lastActivity { get; set; }
}