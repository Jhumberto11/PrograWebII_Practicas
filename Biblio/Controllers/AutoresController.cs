using Biblio.Models;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;

namespace Biblio.Controllers
{
    public class AutoresController : Controller
    {
        private static List<Autor> _autores = new List<Autor>()
            {
                 new Autor()
                 {
                     Id = 1,
                     Name = "Gabriel",
                     Lastname = "García Márquez",
                     Country = "Colombia",
                     Birthdate = new DateOnly(1927, 3, 6)
                 },

                 new Autor()
                 {
                     Id = 2,
                     Name = "Isabel",
                     Lastname = "Allende",
                     Country = "Chile",
                     Birthdate = new DateOnly(1942, 8, 2),
                     IsActive = false
                 },

                 new Autor()
                 {
                     Id = 3,
                     Name = "Jorge Luis",
                     Lastname = "Borges",
                     Country = "Argentina",
                     Birthdate = new DateOnly(1899, 8, 24)
                 },

                 new Autor()
                 {
                     Id = 4,
                     Name = "Miguel",
                     Lastname = "Cervantes",
                     Country = "España",
                     Birthdate = new DateOnly(1547, 9, 29)
                 },

                 new Autor()
                 {
                     Id = 5,
                     Name = "Mario",
                     Lastname = "Vargas Llosa",
                     Country = "Perú",
                     Birthdate = new DateOnly(1936, 3, 28)
                 }
            };
        public IActionResult Index()
        { 
            ViewBag.Autores = _autores;
            return View();
        }

        public IActionResult Details(int id)
        {
            var autor = _autores.FirstOrDefault(x => x.Id == id);
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

            if (_autores.Any())
            {
                autor.Id = _autores.Max(a => a.Id) + 1;
            }
            else
            {
                autor.Id = 1;
            }

            _autores.Add(autor);

            return RedirectToAction(nameof(Index));
        }

        public IActionResult EditView(int id)
        {
            var autor  = _autores.FirstOrDefault(a=> a.Id == id);
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


            var autor = _autores.FirstOrDefault(a => a.Id == autorUpdated.Id);
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
            var autor = _autores.FirstOrDefault(a => a.Id == id);

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
            var autor = _autores.FirstOrDefault(a => a.Id == id);

            if (autor == null)
            {
                return NotFound();
            }

            _autores.Remove(autor);

            return RedirectToAction(nameof(Index));
        }
    }

}
