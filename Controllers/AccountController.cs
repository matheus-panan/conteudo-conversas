using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using painel_conversas.Models.Account;

namespace painel_conversas.Controllers;

public class AccountController : Controller
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly SignInManager<IdentityUser> _signInManager;

    public AccountController(
        UserManager<IdentityUser> userManager,
        SignInManager<IdentityUser> signInManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
    }

    // GET: /Account/Login
    [HttpGet]
    [AllowAnonymous]
    public IActionResult Login(string returnUrl = null)
    {
        if (_signInManager.IsSignedIn(User))
        {
            return RedirectToAction("Index", "Home");
        }
        
        // Normalizar returnUrl - se for null ou vazio, usar Home
        if (string.IsNullOrEmpty(returnUrl) || returnUrl == "/")
        {
            returnUrl = Url.Action("Index", "Home");
        }
        
        ViewData["ReturnUrl"] = returnUrl;
        return View(new LoginViewModel { ReturnUrl = returnUrl });
    }

    // POST: /Account/Login
    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model, string returnUrl = null)
    {
        // Normalizar returnUrl
        if (string.IsNullOrEmpty(returnUrl))
        {
            returnUrl = model.ReturnUrl;
        }
        
        if (string.IsNullOrEmpty(returnUrl) || returnUrl == "/")
        {
            returnUrl = Url.Action("Index", "Home");
        }

        if (ModelState.IsValid)
        {
            var result = await _signInManager.PasswordSignInAsync(
                model.Email, 
                model.Password, 
                model.RememberMe, 
                lockoutOnFailure: false);

            if (result.Succeeded)
            {
                // Usar LocalRedirect apenas se a URL for local, caso contrário usar redirecionamento seguro
                if (Url.IsLocalUrl(returnUrl))
                {
                    return LocalRedirect(returnUrl);
                }
                else
                {
                    return RedirectToAction("Index", "Home");
                }
            }
            if (result.RequiresTwoFactor)
            {
                // Redirecionar para página de verificação de dois fatores
                return RedirectToPage("./LoginWith2fa", new { ReturnUrl = returnUrl, RememberMe = model.RememberMe });
            }
            if (result.IsLockedOut)
            {
                return View("Lockout");
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Email ou senha incorretos.");
                
                // Manter o returnUrl no modelo para preservar na view
                model.ReturnUrl = returnUrl;
                ViewData["ReturnUrl"] = returnUrl;
                return View(model);
            }
        }

        // Se chegou aqui, há erros de validação
        model.ReturnUrl = returnUrl;
        ViewData["ReturnUrl"] = returnUrl;
        return View(model);
    }

    // GET: /Account/Register
    [HttpGet]
    [AllowAnonymous]
    public IActionResult Register(string returnUrl = null)
    {
        if (_signInManager.IsSignedIn(User))
        {
            return RedirectToAction("Index", "Home");
        }
        
        // Normalizar returnUrl
        if (string.IsNullOrEmpty(returnUrl) || returnUrl == "/")
        {
            returnUrl = Url.Action("Index", "Home");
        }
        
        ViewData["ReturnUrl"] = returnUrl;
        return View(new RegisterViewModel());
    }

    // POST: /Account/Register
    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel model, string returnUrl = null)
    {
        // Normalizar returnUrl
        if (string.IsNullOrEmpty(returnUrl) || returnUrl == "/")
        {
            returnUrl = Url.Action("Index", "Home");
        }

        if (ModelState.IsValid)
        {
            var user = new IdentityUser 
            { 
                UserName = model.Email, 
                Email = model.Email,
                EmailConfirmed = true // Para evitar problemas de confirmação
            };
            
            var result = await _userManager.CreateAsync(user, model.Password);
            
            if (result.Succeeded)
            {
                // Fazer login automático após registro
                await _signInManager.SignInAsync(user, isPersistent: false);
                
                // Redirecionar para a URL especificada ou para Home
                if (Url.IsLocalUrl(returnUrl))
                {
                    return LocalRedirect(returnUrl);
                }
                else
                {
                    return RedirectToAction("Index", "Home");
                }
            }
            
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }
        
        ViewData["ReturnUrl"] = returnUrl;
        return View(model);
    }

    // POST: /Account/Logout
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction("Login", "Account");
    }

    // GET: /Account/AccessDenied
    [AllowAnonymous]
    public IActionResult AccessDenied()
    {
        return View();
    }
}