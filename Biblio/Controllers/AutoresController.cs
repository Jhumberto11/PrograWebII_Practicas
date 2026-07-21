using Biblio.Models;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;

namespace Biblio.Controllers
{
    public class AutoresController : Controller
    {
        public IActionResult Index()
        {
            List<Autor> autores = new List<Autor>()
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




            ViewBag.Autores = autores;
            return View();
        }
    }
}
