namespace painel_conversas.Controllers;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

public class ChatController : Controller
{
    private readonly ChatService _chatService;

    public ChatController(ChatService chatService)
    {
        this._chatService = chatService;
    }

    public async Task<IActionResult> Index(string contactId = null)
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
}