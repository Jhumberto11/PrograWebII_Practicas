using Biblio.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;

namespace Biblio.Controllers
{
    [Authorize]
    public class CategoriasController : Controller
    {
        private readonly string _connectionString;
        private readonly ILogger<CategoriasController> _logger;

        public CategoriasController(
            IConfiguration configuration,
            ILogger<CategoriasController> logger)
        {
            _connectionString = configuration.GetConnectionString("BiblioDB");
            _logger = logger;
        }

        public IActionResult Index()
        {
            try
            {
                _logger.LogInformation("Se inició la consulta del listado de categorías");

                var categorias = new List<Categoria>();

                using (var conexion = new SqlConnection(_connectionString))
                {
                    var sql = "SELECT ID, Nombre, Descripcion FROM Categorias";

                    using (var comando = new SqlCommand(sql, conexion))
                    {
                        conexion.Open();

                        using (var lector = comando.ExecuteReader())
                        {
                            while (lector.Read())
                            {
                                categorias.Add(new Categoria
                                {
                                    ID = lector.GetInt32(0),
                                    Nombre = lector.GetString(1),
                                    Descripcion = lector.IsDBNull(2)
                                        ? null
                                        : lector.GetString(2)
                                });
                            }
                        }
                    }
                }

                _logger.LogInformation(
                    "Se consultaron las categorías correctamente. Total: {CantidadCategorias}",
                    categorias.Count);

                return View(categorias);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Se produjo un error al consultar el listado de categorías");
                throw;
            }
        }

        public IActionResult Create()
        {
            try
            {
                _logger.LogInformation(
                    "Se abrió el formulario para crear una categoría");

                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Se produjo un error al abrir el formulario de creación de categorías");
                throw;
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Categoria categoria)
        {
            try
            {
                _logger.LogInformation("Se inició el registro de una categoría");

                if (categoria == null ||
                    string.IsNullOrWhiteSpace(categoria.Nombre) ||
                    !ModelState.IsValid)
                {
                    if (categoria == null ||
                        string.IsNullOrWhiteSpace(categoria.Nombre))
                    {
                        ModelState.AddModelError(
                            "Nombre",
                            "El nombre es obligatorio");
                    }

                    _logger.LogWarning(
                        "No se pudo registrar la categoría porque los datos no son válidos");

                    return View(categoria);
                }

                using (var conexion = new SqlConnection(_connectionString))
                {
                    var sqlInsert = @"INSERT INTO Categorias
                                      (Nombre, Descripcion)
                                      VALUES (@Nombre, @Descripcion)";

                    using (var comando = new SqlCommand(sqlInsert, conexion))
                    {
                        comando.Parameters.AddWithValue(
                            "@Nombre", categoria.Nombre);

                        comando.Parameters.AddWithValue(
                            "@Descripcion",
                            (object)categoria.Descripcion ?? DBNull.Value);

                        conexion.Open();
                        comando.ExecuteNonQuery();
                    }
                }

                _logger.LogInformation(
                    "Se registró correctamente la categoría");

                TempData["SuccessMessage"] =
                    "Categoría guardada correctamente.";

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Se produjo un error al registrar una categoría");
                throw;
            }
        }

        public IActionResult Edit(int id)
        {
            try
            {
                _logger.LogInformation(
                    "Se inició la búsqueda de la categoría con ID {CategoriaId} para editar",
                    id);

                Categoria categoria = null;

                using (var conexion = new SqlConnection(_connectionString))
                {
                    var sql = @"SELECT ID, Nombre, Descripcion
                                FROM Categorias
                                WHERE ID = @ID";

                    using (var comando = new SqlCommand(sql, conexion))
                    {
                        comando.Parameters.AddWithValue("@ID", id);

                        conexion.Open();

                        using (var lector = comando.ExecuteReader())
                        {
                            if (lector.Read())
                            {
                                categoria = new Categoria
                                {
                                    ID = lector.GetInt32(0),
                                    Nombre = lector.GetString(1),
                                    Descripcion = lector.IsDBNull(2)
                                        ? null
                                        : lector.GetString(2)
                                };
                            }
                        }
                    }
                }

                if (categoria == null)
                {
                    _logger.LogWarning(
                        "No se encontró la categoría con ID {CategoriaId} para editar",
                        id);

                    return NotFound();
                }

                _logger.LogInformation(
                    "Se abrió el formulario de edición de la categoría con ID {CategoriaId}",
                    id);

                return View(categoria);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Se produjo un error al consultar la categoría con ID {CategoriaId} para editar",
                    id);
                throw;
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Categoria categoria)
        {
            try
            {
                _logger.LogInformation(
                    "Se inició la actualización de la categoría con ID {CategoriaId}",
                    categoria?.ID);

                if (categoria == null ||
                    string.IsNullOrWhiteSpace(categoria.Nombre) ||
                    !ModelState.IsValid)
                {
                    if (categoria == null ||
                        string.IsNullOrWhiteSpace(categoria.Nombre))
                    {
                        ModelState.AddModelError(
                            "Nombre",
                            "El nombre es obligatorio");
                    }

                    _logger.LogWarning(
                        "No se pudo actualizar la categoría porque los datos no son válidos");

                    return View(categoria);
                }

                using (var conexion = new SqlConnection(_connectionString))
                {
                    var sqlUpdate = @"UPDATE Categorias
                                      SET Nombre = @Nombre,
                                          Descripcion = @Descripcion
                                      WHERE ID = @ID";

                    using (var comando = new SqlCommand(sqlUpdate, conexion))
                    {
                        comando.Parameters.AddWithValue(
                            "@ID", categoria.ID);

                        comando.Parameters.AddWithValue(
                            "@Nombre", categoria.Nombre);

                        comando.Parameters.AddWithValue(
                            "@Descripcion",
                            (object)categoria.Descripcion ?? DBNull.Value);

                        conexion.Open();

                        var filasAfectadas = comando.ExecuteNonQuery();

                        if (filasAfectadas == 0)
                        {
                            _logger.LogWarning(
                                "No se encontró la categoría con ID {CategoriaId} para actualizar",
                                categoria.ID);

                            return NotFound();
                        }
                    }
                }

                _logger.LogInformation(
                    "Se actualizó correctamente la categoría con ID {CategoriaId}",
                    categoria.ID);

                TempData["SuccessMessage"] =
                    "Categoría actualizada correctamente.";

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Se produjo un error al actualizar la categoría con ID {CategoriaId}",
                    categoria?.ID);
                throw;
            }
        }

        public IActionResult Delete(int id)
        {
            try
            {
                _logger.LogInformation(
                    "Se inició la búsqueda de la categoría con ID {CategoriaId} para eliminar",
                    id);

                Categoria categoria = null;

                using (var conexion = new SqlConnection(_connectionString))
                {
                    var sql = @"SELECT ID, Nombre, Descripcion
                                FROM Categorias
                                WHERE ID = @ID";

                    using (var comando = new SqlCommand(sql, conexion))
                    {
                        comando.Parameters.AddWithValue("@ID", id);

                        conexion.Open();

                        using (var lector = comando.ExecuteReader())
                        {
                            if (lector.Read())
                            {
                                categoria = new Categoria
                                {
                                    ID = lector.GetInt32(0),
                                    Nombre = lector.GetString(1),
                                    Descripcion = lector.IsDBNull(2)
                                        ? null
                                        : lector.GetString(2)
                                };
                            }
                        }
                    }
                }

                if (categoria == null)
                {
                    _logger.LogWarning(
                        "No se encontró la categoría con ID {CategoriaId} para eliminar",
                        id);

                    return NotFound();
                }

                _logger.LogInformation(
                    "Se abrió la confirmación para eliminar la categoría con ID {CategoriaId}",
                    id);

                return View(categoria);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Se produjo un error al abrir la confirmación de eliminación de la categoría con ID {CategoriaId}",
                    id);
                throw;
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            try
            {
                _logger.LogInformation(
                    "Se inició la eliminación de la categoría con ID {CategoriaId}",
                    id);

                using (var conexion = new SqlConnection(_connectionString))
                {
                    var sqlDelete =
                        "DELETE FROM Categorias WHERE ID = @ID";

                    using (var comando = new SqlCommand(sqlDelete, conexion))
                    {
                        comando.Parameters.AddWithValue("@ID", id);

                        conexion.Open();

                        var filasAfectadas = comando.ExecuteNonQuery();

                        if (filasAfectadas == 0)
                        {
                            _logger.LogWarning(
                                "No se encontró la categoría con ID {CategoriaId} para eliminar",
                                id);

                            return NotFound();
                        }
                    }
                }

                _logger.LogInformation(
                    "Se eliminó correctamente la categoría con ID {CategoriaId}",
                    id);

                TempData["SuccessMessage"] =
                    "Categoría eliminada correctamente.";

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Se produjo un error al eliminar la categoría con ID {CategoriaId}",
                    id);
                throw;
            }
        }
    }
}