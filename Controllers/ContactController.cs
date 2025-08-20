using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using painel_conversas.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace painel_conversas.Controllers
{
    [Authorize] // Requer autenticação para todas as ações
    public class ContactController : Controller
    {
        private readonly ContactService _contactsService;

        public ContactController(ContactService contactsService)
        {
            _contactsService = contactsService;
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
    }
}