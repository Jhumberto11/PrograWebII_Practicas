

using Biblio.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography.X509Certificates;

namespace Biblio.Data
{
    public class BiblioContext : DbContext
    {
        public BiblioContext(DbContextOptions<BiblioContext> options) : base(options)
        {
            
        }


        public DbSet<Autor> Autores { get; set; }
        public DbSet<Libro> Libros { get; set; }



    }
}
