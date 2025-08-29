// Controllers/ChatController.cs - OTIMIZADO
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using painel_conversas.Models.Pagination;
using painel_conversas.Services.Export;
using System.Threading.Tasks;

namespace painel_conversas.Controllers;

[Authorize]
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

    public async Task<IActionResult> Index(string contactId = null, int page = 1, int pageSize = 50)
    {
        try
        {
            // Validar parâmetros
            if (page < 1) page = 1;
            if (pageSize < 10) pageSize = 10;
            if (pageSize > 100) pageSize = 100;

            PagedResult<painel_conversas.Models.Chat> pagedChats;
            
            if (string.IsNullOrEmpty(contactId))
            {
                // **OTIMIZAÇÃO CRÍTICA**: Usar método otimizado para todas as conversas
                pagedChats = await _chatService.GetAllChatsOptimized(page, pageSize, maxContacts: 15);
                
                // Adicionar aviso sobre limitação
                if (page == 1)
                {
                    TempData["Info"] = "Para melhor performance, exibindo conversas dos primeiros 15 contatos. Use filtros específicos para ver mais.";
                }
            }
            else
            {
                // Para contato específico, usar método paginado
                pagedChats = await _chatService.GetChatByContactPaged(contactId, page, pageSize);
                ViewData["ContactId"] = contactId;
            }

            // Configurar ViewData para paginação
            ViewData["Pagination"] = new PaginationViewModel
            {
                CurrentPage = pagedChats.CurrentPage,
                TotalPages = pagedChats.TotalPages,
                HasPreviousPage = pagedChats.HasPreviousPage,
                HasNextPage = pagedChats.HasNextPage,
                TotalItems = pagedChats.TotalItems,
                StartItem = pagedChats.StartItem,
                EndItem = pagedChats.EndItem,
                PageSize = pagedChats.PageSize,
                Action = "Index",
                Controller = "Chat",
                RouteValues = string.IsNullOrEmpty(contactId) ? null : new { contactId = contactId }
            };

            ViewData["CurrentPageSize"] = pageSize;
            
            return View(pagedChats);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao carregar chats: {ex.Message}");
            
            ViewData["Error"] = "Erro ao carregar as conversas. Tente novamente.";
            
            var emptyResult = new PagedResult<painel_conversas.Models.Chat>
            {
                Items = new List<painel_conversas.Models.Chat>(),
                CurrentPage = 1,
                TotalPages = 0,
                PageSize = pageSize,
                TotalItems = 0
            };
            
            ViewData["Pagination"] = new PaginationViewModel
            {
                CurrentPage = 1,
                TotalPages = 0,
                HasPreviousPage = false,
                HasNextPage = false,
                TotalItems = 0,
                StartItem = 0,
                EndItem = 0,
                PageSize = pageSize,
                Action = "Index",
                Controller = "Chat",
                RouteValues = string.IsNullOrEmpty(contactId) ? null : new { contactId = contactId }
            };
            
            if (!string.IsNullOrEmpty(contactId))
            {
                ViewData["ContactId"] = contactId;
            }
            
            return View(emptyResult);
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
                // Para exportação completa, usar método otimizado com mais contatos
                var pagedResult = await _chatService.GetAllChatsOptimized(1, 5000, maxContacts: 50);
                chats = pagedResult.Items;
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
                var pagedResult = await _chatService.GetAllChatsOptimized(1, 5000, maxContacts: 50);
                chats = pagedResult.Items;
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