using Microsoft.AspNetCore.Mvc;
using Biblio.Models;
using Biblio.Repositories;
using Biblio.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace Biblio.Controllers
{
    [Authorize(Roles = "Administrador,Bibliotecario,Usuario")]
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
        [Authorize(Roles = "Administrador,Bibliotecario")]
        public IActionResult CreateView()
        {
            return View();
        }
        [Authorize(Roles = "Administrador,Bibliotecario")]
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
        [Authorize(Roles = "Administrador,Bibliotecario")]
        public async Task<IActionResult> EditViewLibro(int Id)
        {
            var libro = await _context.Libros.FindAsync(Id);
            
            if (libro == null) return BadRequest();
            return View(libro);
        }
        [Authorize(Roles = "Administrador,Bibliotecario")]
        [HttpPost]
        [ValidateAntiForgeryToken]
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
        [Authorize(Roles = "Administrador,Bibliotecario")]

        public async Task<IActionResult> DeleteViewLibro(int Id)
        {
            var libro = await _context.Libros.FindAsync(Id);
            if (libro == null) return BadRequest();
            return View(libro);
        }

        [Authorize(Roles = "Administrador,Bibliotecario")]
        [HttpPost]
        [ValidateAntiForgeryToken]
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

