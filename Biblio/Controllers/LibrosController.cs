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
        private readonly ILogger<LibrosController> _logger;
        private readonly BiblioContext _context;

        public LibrosController(
            BiblioContext context,
            ILogger<LibrosController> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                _logger.LogInformation("Se inició la consulta de libros");

                var libros = await _context.Libros.ToListAsync();

                _logger.LogInformation("Se consultó el listado de libros correctamente");

                return View(libros);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ocurrió un error al consultar los libros");
                throw;
            }
        }

        public async Task<IActionResult> DetailsLibro(int Id)
        {
            try
            {
                _logger.LogInformation("Se inició la consulta del libro con ID {LibroId}", Id);

                var libro = await _context.Libros.FindAsync(Id);

                if (libro == null)
                {
                    _logger.LogWarning("No se encontró el libro con ID {LibroId}", Id);
                    return NotFound();
                }

                _logger.LogInformation("Se consultaron los detalles del libro con ID {LibroId}", Id);

                return View(libro);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ocurrió un error al consultar el libro con ID {LibroId}", Id);
                throw;
            }
        }

        [Authorize(Roles = "Administrador,Bibliotecario")]
        public IActionResult CreateView()
        {
            try
            {
                _logger.LogInformation("Se abrió el formulario para agregar un libro");

                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ocurrió un error al abrir el formulario para agregar un libro");
                throw;
            }
        }

        [Authorize(Roles = "Administrador,Bibliotecario")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Libro libro)
        {
            try
            {
                _logger.LogInformation("Se inició el registro de un libro");

                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("No se pudo registrar el libro porque los datos no son válidos");
                    return View("CreateView", libro);
                }

                _context.Libros.Add(libro);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Se registró correctamente el libro con ID {LibroId}", libro.ID);

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ocurrió un error al registrar un libro");
                throw;
            }
        }

        [Authorize(Roles = "Administrador,Bibliotecario")]
        public async Task<IActionResult> EditViewLibro(int Id)
        {
            try
            {
                _logger.LogInformation("Se inició la búsqueda del libro con ID {LibroId} para editar", Id);

                var libro = await _context.Libros.FindAsync(Id);

                if (libro == null)
                {
                    _logger.LogWarning("No se encontró el libro con ID {LibroId} para editar", Id);
                    return NotFound();
                }

                _logger.LogInformation("Se abrió el formulario de edición del libro con ID {LibroId}", Id);

                return View(libro);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ocurrió un error al abrir la edición del libro con ID {LibroId}", Id);
                throw;
            }
        }

        [Authorize(Roles = "Administrador,Bibliotecario")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Libro libroUpdated)
        {
            try
            {
                _logger.LogInformation("Se inició la edición del libro con ID {LibroId}", libroUpdated.ID);

                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("Los datos para editar el libro con ID {LibroId} no son válidos", libroUpdated.ID);
                    return View("EditViewLibro", libroUpdated);
                }

                var libro = await _context.Libros.FindAsync(libroUpdated.ID);

                if (libro == null)
                {
                    _logger.LogWarning("No se encontró el libro con ID {LibroId} para actualizar", libroUpdated.ID);
                    return NotFound();
                }

                libro.Title = libroUpdated.Title;
                libro.Autor = libroUpdated.Autor;
                libro.Categoria = libroUpdated.Categoria;
                libro.Precio = libroUpdated.Precio;

                _context.Libros.Update(libro);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Se actualizó correctamente el libro con ID {LibroId}", libro.ID);

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ocurrió un error al editar el libro con ID {LibroId}", libroUpdated.ID);
                throw;
            }
        }

        [Authorize(Roles = "Administrador,Bibliotecario")]
        public async Task<IActionResult> DeleteViewLibro(int Id)
        {
            try
            {
                _logger.LogInformation("Se inició la búsqueda del libro con ID {LibroId} para eliminar", Id);

                var libro = await _context.Libros.FindAsync(Id);

                if (libro == null)
                {
                    _logger.LogWarning("No se encontró el libro con ID {LibroId} para eliminar", Id);
                    return NotFound();
                }

                _logger.LogInformation("Se abrió la confirmación para eliminar el libro con ID {LibroId}", Id);

                return View(libro);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ocurrió un error al abrir la eliminación del libro con ID {LibroId}", Id);
                throw;
            }
        }

        [Authorize(Roles = "Administrador,Bibliotecario")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int ID)
        {
            try
            {
                _logger.LogInformation("Se inició la eliminación del libro con ID {LibroId}", ID);

                var libro = await _context.Libros.FindAsync(ID);

                if (libro == null)
                {
                    _logger.LogWarning("No se encontró el libro con ID {LibroId} para eliminar", ID);
                    return NotFound();
                }

                _context.Libros.Remove(libro);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Se eliminó correctamente el libro con ID {LibroId}", ID);

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ocurrió un error al eliminar el libro con ID {LibroId}", ID);
                throw;
            }
        }
    }
}