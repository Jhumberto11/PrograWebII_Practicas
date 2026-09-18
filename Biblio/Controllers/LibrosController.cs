using Microsoft.AspNetCore.Mvc;
using Biblio.Models;
using Biblio.Repositories;
using Biblio.Data;
using Microsoft.EntityFrameworkCore;

namespace Biblio.Controllers
{
    public class LibrosController : Controller
    {
        private readonly IReposotoryLibro _repositorio;

        private readonly BiblioContext _context;
        public LibrosController(BiblioContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var libros = await  _context.Libros.ToListAsync();
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

            _context.Libros.Add(libro);
            _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));

        }

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
