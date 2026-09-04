using Biblio.Models;
using Biblio.Repositories;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;

namespace Biblio.Controllers
{
    public class AutoresController : Controller
    {
        private readonly IAutoresService _autores;
        public AutoresController(IAutoresService service)
        {
            _autores = service;
        }

        
        public IActionResult Index()
        { 
            
            return View(_autores.GetAllAutors());
        }

        public IActionResult Details(int id)
        {
            var autor = _autores.GetAutor(id);
            if(autor == null)
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
        public IActionResult AddAutor(Autor autor)
        {
            if (!ModelState.IsValid)
            {
                return View(autor);
            }

            if (_autores != null)
            {
                autor.Id = _autores.CountAutors();
            }
            else
            {
                autor.Id = 1;
            }

            _autores.AddAutor(autor);

            return RedirectToAction(nameof(Index));
        }

        public IActionResult EditView(int id)
        {
            var autor  = _autores.GetAutor(id);
            if (autor == null)
            {
                return NotFound();
            }
            return View(autor); 
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditAutor(Autor autorUpdated)
        {

            if (!ModelState.IsValid)
            {
                return View(autorUpdated);
            }


            var autor = _autores.GetAutor(autorUpdated.Id); ;
            if (autor == null)
            {
                return NotFound();
            }

            autor.Name = autorUpdated.Name;
            autor.Birthdate = autorUpdated.Birthdate;
            autor.Lastname = autorUpdated.Lastname;
            autor.Country = autorUpdated.Country;

            return RedirectToAction(nameof(Index));
        }
        [HttpGet]
        public IActionResult DeleteAutor(int id)
        {
            var autor = _autores.GetAutor(id);

            if (autor == null)
            {
                return NotFound();
            }

            return View(autor);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteAutorConfirmed(int id)
        {
            var autor = _autores.GetAutor(id);

            if (autor == null)
            {
                return NotFound();
            }

            _autores.Remove(autor);

            return RedirectToAction(nameof(Index));
        }
    }

}
