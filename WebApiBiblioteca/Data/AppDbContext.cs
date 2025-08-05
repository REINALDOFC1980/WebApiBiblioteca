using Microsoft.EntityFrameworkCore;
using WebApiBiblioteca.Models;


namespace WebApiBiblioteca.Data
{
    public class AppDbContext : DbContext
    {

        public AppDbContext( DbContextOptions<AppDbContext> options ) :base(options)
        {           
        }

        public DbSet<AutorModel> Autores { get; set; }
        public DbSet<LivroModel> Livros { get; set; }

    }
}
