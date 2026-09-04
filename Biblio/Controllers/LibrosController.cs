using Microsoft.AspNetCore.Mvc;
using Biblio.Models;
using Biblio.Repositories;

namespace Biblio.Controllers
{
    public class LibrosController : Controller
    {
        private readonly IReposotoryLibro _repositorio;


        public LibrosController(IReposotoryLibro repositorio)
        {
            _repositorio = repositorio;
        }

        public IActionResult Index()
        {
            var libros = _repositorio.GetAll();
            return View(libros);
        }


        //public IActionResult DetailsLibro(int Id)
        //{
        //    var libros = _repositorio.GetAll();
        //    var libro = libros.FirstOrDefault(x => x.ID == Id);
        //    if (libro == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(libro);
        //}

        //public IActionResult CreateView()
        //{
        //    return View();
        //}

        //[HttpPost]
        //[ValidateAntiForgeryTokenAttribute]
        //public IActionResult Create(Libro libro)
        //{

        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }

        //    if (libros.Any())
        //    {
        //        libro.ID = libros.Max(l=> l.ID) + 1;
        //    }
        //    else
        //    {
        //        libro.ID = 1;
        //    }

        //    libros.Add(libro);

        //    return RedirectToAction(nameof(Index));

        //}

        //    public IActionResult EditViewLibro(int Id)
        //    {
        //        var libros = _repositorio.GetAll();
        //        var libro = libros.FirstOrDefault(l => l.ID == Id);
        //        if (libro == null) return BadRequest();
        //        return View(libro);
        //    }

        //    public IActionResult Edit(Libro libroUpdated)
        //    {
        //        var libros = _repositorio.GetAll();

        //        if (!ModelState.IsValid)
        //        {
        //            return View(libroUpdated);
        //        }


        //        var libro = libros.FirstOrDefault(a => a.ID == libroUpdated.ID);
        //        if (libro == null)
        //        {
        //            return NotFound();
        //        }

        //        libro.Title = libroUpdated.Title;
        //        libro.Autor = libroUpdated.Autor;
        //        libro.Categoria = libroUpdated.Categoria;
        //        libro.Precio = libroUpdated.Precio;
        //        libro.Categoria = libroUpdated.Categoria;

        //        return RedirectToAction(nameof(Index));
        //    }

        //    public IActionResult DeleteViewLibro(int Id)
        //    {
        //        var libros = _repositorio.GetAll();
        //        var libro = libros.FirstOrDefault(l => l.ID == Id);
        //        if (libro == null) return BadRequest();
        //        return View(libro);
        //    }
        //    public IActionResult Delete(int ID)
        //    {
        //        var libros = _repositorio.GetAll();
        //        var libro = libros.FirstOrDefault(l=> l.ID == ID);
        //        if(libro == null) return NotFound();
        //        ///libros.Remove(libro);
        //        return RedirectToAction(nameof(Index));
        //    }


        //}
    }
}
