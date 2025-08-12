namespace painel_conversas.Controllers;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
//using painel_conversas.Services;

public class ChatController : Controller
{
    private readonly ChatService _chatService;

    public ChatController(ChatService chatService)
    {
        this._chatService = chatService;
    }

    public async Task<IActionResult> Index()
    {
        var chats = await _chatService.GetChat();
        return View(chats);

    }
}