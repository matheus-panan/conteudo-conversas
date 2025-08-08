using Microsoft.AspNetCore.Mvc;
using painel_conversas.Models;
using painel_conversas.Services;

namespace painel_conversas.Controllers;

public class ContactController : Controller
{
    private readonly ApiService apiService;

    public ContactController(ApiService apiService)
    {
        this.apiService = apiService;
    }

    public async Task<IActionResult> Index()
    {
        var contacts = await apiService.GetContact() ?? new List<ContactItems>();
        return View(contacts);

    }
}