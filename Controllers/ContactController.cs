using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using painel_conversas.Models;
using painel_conversas.Services.Export;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace painel_conversas.Controllers
{
    [Authorize] // Requer autenticação para todas as ações
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

        public async Task<IActionResult> Index()
        {
            try
            {
                var contacts = await _contactsService.GetContacts();
                return View(contacts);
            }
            catch (Exception ex)
            {
                // Log do erro
                Console.WriteLine($"Erro ao carregar contatos: {ex.Message}");
                
                // Retorna uma view com lista vazia em caso de erro
                ViewData["Error"] = "Erro ao carregar os contatos. Tente novamente.";
                return View(new List<ContactItem>());
            }
        }

        [HttpGet]
        public async Task<IActionResult> ExportJson()
        {
            try
            {
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
    }
}