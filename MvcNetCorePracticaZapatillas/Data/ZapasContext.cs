using Microsoft.EntityFrameworkCore;
using MvcNetCorePracticaZapatillas.Models;

namespace MvcNetCorePracticaZapatillas.Data
{
    public class ZapasContext : DbContext
    {
        public ZapasContext(DbContextOptions<ZapasContext> options) :
            base(options)
        { }

        public DbSet<Zapatilla> Zapatillas { get; set; }
        public DbSet<ImagenZapatilla> ImagenesZapatillas { get; set; }
    }
}
