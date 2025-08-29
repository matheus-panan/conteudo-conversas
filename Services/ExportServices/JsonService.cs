using System.Text.Json;
using System.Text;
using painel_conversas.Models;

namespace painel_conversas.Services.Export;

public class JsonService
{
    public byte[] ExportContacts(List<ContactItem> contacts)
    {
        try
        {
            var exportData = new
            {
                ExportDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                TotalContacts = contacts.Count,
                Contacts = contacts.Select(c => new
                {
                    Id = c.Id,
                    Name = c.Name ?? "N/A",
                    Phone = c.Phone ?? "N/A",
                    Email = c.Email ?? "N/A",
                    IdentificationDocument = c.IdentificationDocument ?? "N/A",
                    Note = c.Note ?? "N/A",
                    DateCreate = c.DateCreate,
                    LastActivity = c.LastActivity,
                    LastMessageActivity = c.LastMessageActivity
                }).ToList()
            };

            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            };

            var jsonString = JsonSerializer.Serialize(exportData, options);
            return Encoding.UTF8.GetBytes(jsonString);
        }
        catch (Exception ex)
        {
            throw new Exception($"Erro ao gerar JSON dos contatos: {ex.Message}", ex);
        }
    }

    public byte[] ExportChats(List<Chat> chats)
    {
        try
        {
            var exportData = new
            {
                ExportDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                TotalMessages = chats.Count,
                Messages = chats.Select(c => new
                {
                    Id = c.Id,
                    IdChat = c.IdChat,
                    Text = c.Text ?? "N/A",
                    Type = c.Type ?? "N/A",
                    Origin = c.Origin ?? "N/A",
                    SenderId = c.SenderId ?? "N/A",
                    SenderName = c.SenderName ?? "N/A",
                    SenderType = c.SenderType ?? "N/A",
                    ConversationId = c.ConversationId ?? "N/A",
                    CreatedAt = c.CreatedAt,
                    CreatedAtFormatted = DateTimeOffset.FromUnixTimeMilliseconds(c.CreatedAt)
                        .LocalDateTime.ToString("dd/MM/yyyy HH:mm:ss"),
                    Attachment = c.Attachment
                }).OrderByDescending(m => m.CreatedAt).ToList()
            };

            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            };

            var jsonString = JsonSerializer.Serialize(exportData, options);
            return Encoding.UTF8.GetBytes(jsonString);
        }
        catch (Exception ex)
        {
            throw new Exception($"Erro ao gerar JSON das conversas: {ex.Message}", ex);
        }
    }
}