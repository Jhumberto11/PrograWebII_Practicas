using Biblio.Data;
using Biblio.Models;
using Biblio.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;

namespace Biblio.Controllers
{
    public class AutoresController : Controller
    {
        private readonly IAutoresService _autores;

        private readonly BiblioContext _context;
        public AutoresController(BiblioContext context)
        {
            _context = context;
        }


        //public AutoresController(IAutoresService service)
        //{
        //    _autores = service;
        //}

        
        public async Task<IActionResult> Index()
        { 
            var autores = await _context.Autores.ToListAsync(); 
            return View(autores);
        }

        public async Task<IActionResult> Details(int id)
        {
            var autor = await _context.Autores.FindAsync(id);

            if (autor == null)
            {
                return NotFound();
            }
            return View(autor);

        }

        public IActionResult Create()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryTokenAttribute]
        public async Task<IActionResult> AddAutor(Autor autor)
        {
            if (!ModelState.IsValid)
            {
                return View(autor);
            }

            _context.Autores.Add(autor);
            await _context.SaveChangesAsync();


            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> EditView(int id)
        {
            var autor = await _context.Autores.FindAsync(id);
            if (autor == null)
            {
                return NotFound();
            }
            return View(autor);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditAutor(Autor autorUpdated)
        {

            if (!ModelState.IsValid)
            {
                return View(autorUpdated);
            }


            var autor = await _context.Autores.FindAsync(autorUpdated.Id);
            if (autor == null)
            {
                return NotFound();
            }

            autor.Name = autorUpdated.Name;
            autor.Birthdate = autorUpdated.Birthdate;
            autor.Lastname = autorUpdated.Lastname;
            autor.Country = autorUpdated.Country;

            _context.Update(autor);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }


        public async Task<IActionResult> DeleteAutor(int id)
        {
            var autor = await _context.Autores.FindAsync(id);

            if (autor == null)
            {
                return NotFound();
            }

            return View(autor);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteAutorConfirmed(int id)
        {
            var autor = await _context.Autores.FindAsync(id);

            if (autor == null)
            {
                return NotFound();
            }

            _context.Autores.Remove(autor);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }

}
