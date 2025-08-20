using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace painel_conversas.Controllers;

[Authorize] // Requer autenticação para todas as ações
public class ChatController : Controller
{
    private readonly ChatService _chatService;

    public ChatController(ChatService chatService)
    {
        this._chatService = chatService;
    }

    public async Task<IActionResult> Index(string contactId = null)
    {
        try
        {
            if (string.IsNullOrEmpty(contactId))
            {
                // Se não há contactId, busca conversas de todos os contatos
                var allChats = await _chatService.GetAllChats();
                return View(allChats);
            }
            else
            {
                // Se há contactId específico, busca apenas as conversas desse contato
                var contactChats = await _chatService.GetChatByContact(contactId);
                ViewData["ContactId"] = contactId;
                return View(contactChats);
            }
        }
        catch (Exception ex)
        {
            // Log do erro
            Console.WriteLine($"Erro ao carregar chats: {ex.Message}");
            
            // Retorna uma view com lista vazia em caso de erro
            ViewData["Error"] = "Erro ao carregar as conversas. Tente novamente.";
            return View(new List<painel_conversas.Models.Chat>());
        }
    }
}