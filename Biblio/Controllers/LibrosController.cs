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


        public async Task<IActionResult> DetailsLibro(int Id)
        {
            var libro = await _context.Libros.FindAsync(Id);
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

            _context.Libros.Add(libro);
            _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));

        }

        public async Task<IActionResult> EditViewLibro(int Id)
        {
            var libro = await _context.Libros.FindAsync(Id);
            
            if (libro == null) return BadRequest();
            return View(libro);
        }

        public async Task<IActionResult> Edit(Libro libroUpdated)
        {
            var libro = await _context.Libros.FindAsync(libroUpdated.ID);

            if (!ModelState.IsValid)
            {
                return View(libroUpdated);
            }

            if (libro == null)
            {
                return NotFound();
            }

            libro.Title = libroUpdated.Title;
            libro.Autor = libroUpdated.Autor;
            libro.Categoria = libroUpdated.Categoria;
            libro.Precio = libroUpdated.Precio;
            libro.Categoria = libroUpdated.Categoria;

            _context.Libros.Update(libro);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> DeleteViewLibro(int Id)
        {
            var libro = await _context.Libros.FindAsync(Id);
            if (libro == null) return BadRequest();
            return View(libro);
        }
        public async Task<IActionResult> Delete(int ID)
        {
            var libro = await _context.Libros.FindAsync(ID);
            if (libro == null) return NotFound();

            _context.Libros.Remove(libro);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }


    }
}

