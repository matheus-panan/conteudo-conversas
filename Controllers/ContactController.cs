using Microsoft.AspNetCore.Mvc;
using painel_conversas.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace painel_conversas.Controllers
{
    public class ContactsController : Controller
    {
        private readonly ContactService _contactsService;

        public ContactsController(ContactService contactsService)
        {
            _contactsService = contactsService;
        }

        public async Task<IActionResult> Index()
        {
            List<string> ids = await _contactsService.GetContacts();
            return View(ids);
        }
    }
}
