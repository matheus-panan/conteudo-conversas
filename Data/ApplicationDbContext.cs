using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
//using conteudo_conversas.Data; // Substitua pelo namespace do seu projeto

namespace conteudo_conversas.Data
{
    public class ApplicationDbContext : IdentityDbContext<IdentityUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // Adicione aqui seus DbSets para as entidades existentes do projeto
        // Exemplo:
        // public DbSet<Chat> Chats { get; set; }
        // public DbSet<Message> Messages { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder); // Importante para Identity
            
            // Adicione aqui configurações adicionais do modelo, se necessário
            // Exemplo:
            // modelBuilder.Entity<Chat>().HasKey(c => c.Id);
        }
    }
}