namespace painel_conversas.Controllers;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using painel_conversas.Services;

public class ChatController : Controller
{
    private readonly ApiService apiService;

    public ChatController(ApiService apiService)
    {
        this.apiService = apiService;
    }

    public async Task<IActionResult> Index()
    {
        var chats = await apiService.GetChat();
        return View(chats);
    }
}