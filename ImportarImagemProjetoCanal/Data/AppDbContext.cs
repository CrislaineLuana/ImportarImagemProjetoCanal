using ImportarImagemProjetoCanal.Models;
using Microsoft.EntityFrameworkCore;
using static System.Net.Mime.MediaTypeNames;

namespace ImportarImagemProjetoCanal.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options ) : base(options)
        {
        }

        public DbSet<ImagemModel> Imagens { get; set; }
    }
}
