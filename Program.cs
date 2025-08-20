using conteudo_conversas.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using painel_conversas;

var builder = WebApplication.CreateBuilder(args);

// Configuração do DbContext (substitua pela sua string de conexão)
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// Configuração do Identity com políticas de senha
builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
{
    // Configurações de senha
    options.Password.RequiredLength = 6;
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = false;
    options.Password.RequireNonAlphanumeric = false;

    // Configurações de usuário
    options.User.RequireUniqueEmail = true;

    // Configurações de login
    options.SignIn.RequireConfirmedAccount = false;
    options.SignIn.RequireConfirmedEmail = false;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

// Configuração das rotas de autenticação
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.LogoutPath = "/Account/Logout";
    options.AccessDeniedPath = "/Account/AccessDenied";
    options.ExpireTimeSpan = TimeSpan.FromHours(12);
    options.SlidingExpiration = true;
});

// Add services to the container.
builder.Services.AddControllersWithViews();

// Registrar os HttpClients
builder.Services.AddHttpClient<ChatService>();
builder.Services.AddHttpClient<ContactService>();

// Registrar os serviços
builder.Services.AddScoped<ContactService>();
builder.Services.AddScoped<ChatService>();

var app = builder.Build();

// Garantir que o banco de dados seja criado
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    context.Database.EnsureCreated();
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

// IMPORTANTE: A ordem é crucial aqui
app.UseAuthentication(); // Primeiro a autenticação
app.UseAuthorization();  // Depois a autorização

// Rotas específicas primeiro
app.MapControllerRoute(
    name: "account",
    pattern: "Account/{action=Login}",
    defaults: new { controller = "Account" });

// Rota padrão
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();