using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using painel_conversas.Services.Export;
using System.Threading.Tasks;

namespace painel_conversas.Controllers;

[Authorize] // Requer autenticação para todas as ações
public class ChatController : Controller
{
    private readonly ChatService _chatService;
    private readonly JsonService _jsonService;
    private readonly CSVService _csvService;

    public ChatController(ChatService chatService, JsonService jsonService, CSVService csvService)
    {
        this._chatService = chatService;
        _jsonService = jsonService;
        _csvService = csvService;
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

    [HttpGet]
    public async Task<IActionResult> ExportJson(string contactId = null)
    {
        try
        {
            List<painel_conversas.Models.Chat> chats;
            string fileName;

            if (string.IsNullOrEmpty(contactId))
            {
                chats = await _chatService.GetAllChats();
                fileName = $"conversas_todas_{DateTime.Now:yyyyMMdd_HHmmss}.json";
            }
            else
            {
                chats = await _chatService.GetChatByContact(contactId);
                fileName = $"conversas_{contactId}_{DateTime.Now:yyyyMMdd_HHmmss}.json";
            }

            var jsonResult = _jsonService.ExportChats(chats);
            return File(jsonResult, "application/json", fileName);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao exportar conversas para JSON: {ex.Message}");
            TempData["Error"] = "Erro ao exportar conversas. Tente novamente.";
            return RedirectToAction("Index", new { contactId });
        }
    }

    [HttpGet]
    public async Task<IActionResult> ExportCsv(string contactId = null)
    {
        try
        {
            List<painel_conversas.Models.Chat> chats;
            string fileName;

            if (string.IsNullOrEmpty(contactId))
            {
                chats = await _chatService.GetAllChats();
                fileName = $"conversas_todas_{DateTime.Now:yyyyMMdd_HHmmss}.csv";
            }
            else
            {
                chats = await _chatService.GetChatByContact(contactId);
                fileName = $"conversas_{contactId}_{DateTime.Now:yyyyMMdd_HHmmss}.csv";
            }

            var csvResult = _csvService.ExportChats(chats);
            return File(csvResult, "text/csv", fileName);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao exportar conversas para CSV: {ex.Message}");
            TempData["Error"] = "Erro ao exportar conversas. Tente novamente.";
            return RedirectToAction("Index", new { contactId });
        }
    }
}