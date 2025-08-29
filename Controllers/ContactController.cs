// Controllers/ContactController.cs - OTIMIZADO
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using painel_conversas.Models;
using painel_conversas.Models.Pagination;
using painel_conversas.Services.Export;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace painel_conversas.Controllers
{
    [Authorize]
    public class ContactController : Controller
    {
        private readonly ContactService _contactsService;
        private readonly JsonService _jsonService;
        private readonly CSVService _csvService;

        public ContactController(ContactService contactsService, JsonService jsonService, CSVService csvService)
        {
            _contactsService = contactsService;
            _jsonService = jsonService;
            _csvService = csvService;
        }

        public async Task<IActionResult> Index(int page = 1, int pageSize = 50)
        {
            try
            {
                // Validar parâmetros
                if (page < 1) page = 1;
                if (pageSize < 10) pageSize = 10;
                if (pageSize > 100) pageSize = 100;

                // **OTIMIZAÇÃO**: Usar método paginado diretamente
                var pagedContacts = await _contactsService.GetContactsPaged(page, pageSize);
                
                // Configurar ViewData para paginação
                ViewData["Pagination"] = new PaginationViewModel
                {
                    CurrentPage = pagedContacts.CurrentPage,
                    TotalPages = pagedContacts.TotalPages,
                    HasPreviousPage = pagedContacts.HasPreviousPage,
                    HasNextPage = pagedContacts.HasNextPage,
                    TotalItems = pagedContacts.TotalItems,
                    StartItem = pagedContacts.StartItem,
                    EndItem = pagedContacts.EndItem,
                    PageSize = pagedContacts.PageSize,
                    Action = "Index",
                    Controller = "Contact"
                };

                ViewData["CurrentPageSize"] = pageSize;
                
                return View(pagedContacts);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao carregar contatos: {ex.Message}");
                
                ViewData["Error"] = "Erro ao carregar os contatos. Tente novamente.";
                var emptyResult = new PagedResult<ContactItem>
                {
                    Items = new List<ContactItem>(),
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
                    Controller = "Contact"
                };
                
                return View(emptyResult);
            }
        }

        [HttpGet]
        public async Task<IActionResult> ExportJson()
        {
            try
            {
                // Para exportação, sempre pegar todos os contatos
                var contacts = await _contactsService.GetContacts();
                var jsonResult = _jsonService.ExportContacts(contacts);
                
                var fileName = $"contatos_{DateTime.Now:yyyyMMdd_HHmmss}.json";
                return File(jsonResult, "application/json", fileName);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao exportar contatos para JSON: {ex.Message}");
                TempData["Error"] = "Erro ao exportar contatos. Tente novamente.";
                return RedirectToAction("Index");
            }
        }

        [HttpGet]
        public async Task<IActionResult> ExportCsv()
        {
            try
            {
                var contacts = await _contactsService.GetContacts();
                var csvResult = _csvService.ExportContacts(contacts);
                
                var fileName = $"contatos_{DateTime.Now:yyyyMMdd_HHmmss}.csv";
                return File(csvResult, "text/csv", fileName);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao exportar contatos para CSV: {ex.Message}");
                TempData["Error"] = "Erro ao exportar contatos. Tente novamente.";
                return RedirectToAction("Index");
            }
        }

        // Novo método para limpar cache se necessário
        [HttpPost]
        public IActionResult ClearCache()
        {
            _contactsService.ClearCache();
            TempData["Success"] = "Cache limpo com sucesso!";
            return RedirectToAction("Index");
        }
    }
}

