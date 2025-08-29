using System.Text;
using painel_conversas.Models;

namespace painel_conversas.Services.Export;

public class CSVService
{
    public byte[] ExportContacts(List<ContactItem> contacts)
    {
        try
        {
            var csv = new StringBuilder();
            
            // Cabeçalho do CSV
            csv.AppendLine("Id,Nome,Telefone,Email,CPF,Nota,Data_Criacao,Ultima_Atividade,Ultima_Mensagem");
            
            // Dados dos contatos
            foreach (var contact in contacts)
            {
                var line = new StringBuilder();
                line.Append(EscapeCsvValue(contact.Id));
                line.Append(",");
                line.Append(EscapeCsvValue(contact.Name ?? "N/A"));
                line.Append(",");
                line.Append(EscapeCsvValue(contact.Phone ?? "N/A"));
                line.Append(",");
                line.Append(EscapeCsvValue(contact.Email ?? "N/A"));
                line.Append(",");
                line.Append(EscapeCsvValue(contact.IdentificationDocument ?? "N/A"));
                line.Append(",");
                line.Append(EscapeCsvValue(contact.Note ?? "N/A"));
                line.Append(",");
                line.Append(EscapeCsvValue(contact.DateCreate ?? "N/A"));
                line.Append(",");
                line.Append(EscapeCsvValue(contact.LastActivity ?? "N/A"));
                line.Append(",");
                line.Append(EscapeCsvValue(contact.LastMessageActivity ?? "N/A"));
                
                csv.AppendLine(line.ToString());
            }
            
            return Encoding.UTF8.GetBytes(csv.ToString());
        }
        catch (Exception ex)
        {
            throw new Exception($"Erro ao gerar CSV dos contatos: {ex.Message}", ex);
        }
    }

    public byte[] ExportChats(List<Chat> chats)
    {
        try
        {
            var csv = new StringBuilder();
            
            // Cabeçalho do CSV
            csv.AppendLine("Id,Id_Chat,Texto,Tipo,Origem,Id_Remetente,Nome_Remetente,Tipo_Remetente,Id_Conversa,Data_Hora,Data_Timestamp");
            
            // Dados das conversas ordenadas por data
            var sortedChats = chats.OrderByDescending(c => c.CreatedAt);
            
            foreach (var chat in sortedChats)
            {
                var line = new StringBuilder();
                line.Append(EscapeCsvValue(chat.Id ?? "N/A"));
                line.Append(",");
                line.Append(EscapeCsvValue(chat.IdChat ?? "N/A"));
                line.Append(",");
                line.Append(EscapeCsvValue(chat.Text ?? "N/A"));
                line.Append(",");
                line.Append(EscapeCsvValue(chat.Type ?? "N/A"));
                line.Append(",");
                line.Append(EscapeCsvValue(chat.Origin ?? "N/A"));
                line.Append(",");
                line.Append(EscapeCsvValue(chat.SenderId ?? "N/A"));
                line.Append(",");
                line.Append(EscapeCsvValue(chat.SenderName ?? "N/A"));
                line.Append(",");
                line.Append(EscapeCsvValue(chat.SenderType ?? "N/A"));
                line.Append(",");
                line.Append(EscapeCsvValue(chat.ConversationId ?? "N/A"));
                line.Append(",");
                line.Append(EscapeCsvValue(DateTimeOffset.FromUnixTimeMilliseconds(chat.CreatedAt)
                    .LocalDateTime.ToString("dd/MM/yyyy HH:mm:ss")));
                line.Append(",");
                line.Append(chat.CreatedAt.ToString());
                
                csv.AppendLine(line.ToString());
            }
            
            return Encoding.UTF8.GetBytes(csv.ToString());
        }
        catch (Exception ex)
        {
            throw new Exception($"Erro ao gerar CSV das conversas: {ex.Message}", ex);
        }
    }

    private string EscapeCsvValue(string value)
    {
        if (string.IsNullOrEmpty(value))
            return "\"\"";

        // Se contém vírgula, aspas duplas ou quebra de linha, precisa ser escapado
        if (value.Contains(",") || value.Contains("\"") || value.Contains("\n") || value.Contains("\r"))
        {
            // Duplica as aspas duplas existentes
            value = value.Replace("\"", "\"\"");
            // Envolve em aspas duplas
            return $"\"{value}\"";
        }

        return value;
    }
}