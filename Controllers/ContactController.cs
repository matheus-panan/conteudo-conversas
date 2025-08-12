using Microsoft.AspNetCore.Mvc;
using painel_conversas.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace painel_conversas.Controllers
{
    public class ContactController : Controller
    {
        private readonly ContactService _contactsService;

        public ContactController(ContactService contactsService)
        {
            _contactsService = contactsService;
        }

        public async Task<IActionResult> Index()
        {
            var contacts = await _contactsService.GetContacts();
            return View(contacts);
        }
    }
}