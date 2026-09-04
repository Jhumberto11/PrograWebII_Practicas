using Biblio.Models;

namespace Biblio.Repositories
{
    public class RepositorioEnMemoria : IReposotoryLibro
    {
        public IEnumerable<Libro> GetAll()
        {
            return new List<Libro>()
            {
                new Libro()
                {
                    ID = 1,
                    Title = "Test",
                     Autor = "Jona",
                      Categoria = "Test",
                       Disponible = true,
                         Precio = 13,
                          Year = 2013,
                },
                new Libro()
                {
                    ID = 2,
                    Title = "El Inversor Inteligente",
                     Autor = "Jonathan",
                      Categoria = "Finanzas",
                       Disponible = false,
                         Precio = 199,
                          Year = 2026,
                          ImageURL = "~/images/libros/inversorInteligente.jpeg"
                }
            };
        }
    }
}
