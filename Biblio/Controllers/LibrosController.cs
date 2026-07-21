using Microsoft.AspNetCore.Mvc;
using Biblio.Models;

namespace Biblio.Controllers
{
    public class LibrosController : Controller
    {
        public IActionResult Index()
        {

            List<Libro> libros = new List<Libro>()
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
                    Title = "Test 2",
                     Autor = "Jonathan",
                      Categoria = "Tests",
                       Disponible = false,
                         Precio = 199,
                          Year = 2026,
                }
            };

            ViewBag.Nombre = "Jonathan";
            ViewBag.Libros = libros;
            return View();
        }
    }
}
