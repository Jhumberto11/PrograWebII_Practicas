using Microsoft.AspNetCore.Mvc;
using Biblio.Models;

namespace Biblio.Controllers
{
    public class LibrosController : Controller
    {
        private static List<Libro> libros = new List<Libro>()
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
        public IActionResult Index()
        {

            

            ViewBag.Nombre = "Jonathan";
            ViewBag.Libros = libros;
            return View();
        }


        public IActionResult DetailsLibro(int Id) 
        {
            var libro = libros.FirstOrDefault(x => x.ID == Id);
            if (libro == null)
            {
                return NotFound();
            }

            return View(libro);
        }

        public IActionResult CreateView()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryTokenAttribute]
        public IActionResult Create(Libro libro)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (libros.Any())
            {
                libro.ID = libros.Max(l=> l.ID) + 1;
            }
            else
            {
                libro.ID = 1;
            }

            libros.Add(libro);

            return RedirectToAction(nameof(Index));

        }

        public IActionResult EditViewLibro(int Id)
        {
            var libro = libros.FirstOrDefault(l => l.ID == Id);
            if (libro == null) return BadRequest();
            return View(libro);
        }

        public IActionResult Edit(Libro libroUpdated)
        {

            if (!ModelState.IsValid)
            {
                return View(libroUpdated);
            }


            var libro = libros.FirstOrDefault(a => a.ID == libroUpdated.ID);
            if (libro == null)
            {
                return NotFound();
            }

            libro.Title = libroUpdated.Title;
            libro.Autor = libroUpdated.Autor;
            libro.Categoria = libroUpdated.Categoria;
            libro.Precio = libroUpdated.Precio;
            libro.Categoria = libroUpdated.Categoria;

            return RedirectToAction(nameof(Index));
        }

        public IActionResult DeleteViewLibro(int Id)
        {
            var libro = libros.FirstOrDefault(l => l.ID == Id);
            if (libro == null) return BadRequest();
            return View(libro);
        }
        public IActionResult Delete(int ID)
        {
            var libro = libros.FirstOrDefault(l=> l.ID == ID);
            if(libro == null) return NotFound();
            libros.Remove(libro);
            return RedirectToAction(nameof(Index));
        }


    }
}
