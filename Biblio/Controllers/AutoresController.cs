
using System;
using System.Threading.Tasks;
using Biblio.Data;
using Biblio.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Biblio.Controllers
{
    [Authorize]
    public class AutoresController : Controller
    {
        private readonly ILogger<AutoresController> _logger;
        private readonly BiblioContext _context;

        public AutoresController(BiblioContext context, ILogger<AutoresController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [Authorize(Roles = "Administrador,Lector")]
        public async Task<IActionResult> Index()
        {
            try
            {
                _logger.LogInformation("Se inicio la consulta del listado de autores");

                var autores = await _context.Autores.ToListAsync();

                _logger.LogInformation("Se consulto el listado de autores correctamente");
                return View(autores);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Se produjo un error al consultar el listado de autores");
                throw;
            }
        }

        [Authorize(Roles = "Administrador,Lector")]
        public async Task<IActionResult> Details(int id)
        {
            try
            {
                _logger.LogInformation("Se inicio la consulta de los detalles del autor con ID {AutorId}", id);

                var autor = await _context.Autores.FindAsync(id);

                if (autor == null)
                {
                    _logger.LogWarning("No se encontro el autor con ID {AutorId}", id);
                    return NotFound();
                }

                _logger.LogInformation("Se consultaron los detalles del autor con ID {AutorId}", id);
                return View(autor);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Se produjo un error al consultar los detalles del autor con ID {AutorId}", id);
                throw;
            }
        }

        [Authorize(Roles = "Administrador")]
        public IActionResult Create()
        {
            try
            {
                _logger.LogInformation("Se abrio el formulario para agregar un autor");
                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Se produjo un error al abrir el formulario para agregar un autor");
                throw;
            }
        }

        [Authorize(Roles = "Administrador")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddAutor(Autor autor)
        {
            try
            {
                _logger.LogInformation("Se inicio el registro de un autor");

                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("No se pudo registrar el autor porque los datos no son validos");
                    return View("Create", autor);
                }

                _context.Autores.Add(autor);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Se registro correctamente el autor con ID {AutorId}", autor.Id);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Se produjo un error al registrar un autor");
                throw;
            }
        }

        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> EditView(int id)
        {
            try
            {
                _logger.LogInformation("Se inicio la busqueda del autor con ID {AutorId} para editar", id);

                var autor = await _context.Autores.FindAsync(id);

                if (autor == null)
                {
                    _logger.LogWarning("No se encontro el autor con ID {AutorId} para editar", id);
                    return NotFound();
                }

                _logger.LogInformation("Se abrio el formulario de edicion del autor con ID {AutorId}", id);
                return View(autor);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Se produjo un error al abrir la edicion del autor con ID {AutorId}", id);
                throw;
            }
        }

        [Authorize(Roles = "Administrador")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditAutor(Autor autorUpdated)
        {
            try
            {
                _logger.LogInformation("Se inicio la edicion del autor con ID {AutorId}", autorUpdated.Id);

                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("Los datos para editar el autor con ID {AutorId} no son validos", autorUpdated.Id);
                    return View("EditView", autorUpdated);
                }

                var autor = await _context.Autores.FindAsync(autorUpdated.Id);

                if (autor == null)
                {
                    _logger.LogWarning("No se encontro el autor con ID {AutorId} para actualizar", autorUpdated.Id);
                    return NotFound();
                }

                autor.Name = autorUpdated.Name;
                autor.Birthdate = autorUpdated.Birthdate;
                autor.Lastname = autorUpdated.Lastname;
                autor.Country = autorUpdated.Country;

                _context.Update(autor);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Se actualizo correctamente el autor con ID {AutorId}", autor.Id);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Se produjo un error al editar el autor con ID {AutorId}", autorUpdated.Id);
                throw;
            }
        }

        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> DeleteAutor(int id)
        {
            try
            {
                _logger.LogInformation("Se inicio la busqueda del autor con ID {AutorId} para eliminar", id);

                var autor = await _context.Autores.FindAsync(id);

                if (autor == null)
                {
                    _logger.LogWarning("No se encontro el autor con ID {AutorId} para eliminar", id);
                    return NotFound();
                }

                _logger.LogInformation("Se abrio la confirmacion para eliminar el autor con ID {AutorId}", id);
                return View(autor);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Se produjo un error al abrir la eliminacion del autor con ID {AutorId}", id);
                throw;
            }
        }

        [Authorize(Roles = "Administrador")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteAutorConfirmed(int id)
        {
            try
            {
                _logger.LogInformation("Se inicio la eliminacion del autor con ID {AutorId}", id);

                var autor = await _context.Autores.FindAsync(id);

                if (autor == null)
                {
                    _logger.LogWarning("No se encontro el autor con ID {AutorId} para eliminar", id);
                    return NotFound();
                }

                _context.Autores.Remove(autor);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Se elimino correctamente el autor con ID {AutorId}", id);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Se produjo un error al eliminar el autor con ID {AutorId}", id);
                throw;
            }
        }
    }
}
