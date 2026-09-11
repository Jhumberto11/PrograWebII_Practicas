using Biblio.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.FileProviders.Physical;

namespace Biblio.Controllers
{
    public class CategoriasController : Controller
    {
        private readonly string _connectionString;

        public CategoriasController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("BiblioDB");
        }
        public IActionResult Index()
        {
            var categorias = new List<Categoria>();
            using(var conexion = new SqlConnection(_connectionString))
            {
                var sql = "select ID,Nombre, Descripcion from Categorias";
                using (var comando = new SqlCommand(sql, conexion))
                {
                    conexion.Open();
                    using( var lector = comando.ExecuteReader())
                    {
                        while (lector.Read())
                        {
                            categorias.Add(new Categoria
                            {
                                ID = lector.GetInt32(0),
                                Nombre = lector.GetString(1),
                                Descripcion = lector.IsDBNull(2) ? null : lector.GetString(2),
                            });


                        }
                    }
                }
            }
            return View(categorias);
        }



        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Categoria categoria)
        {
            if(categoria == null || string.IsNullOrWhiteSpace(categoria.Nombre))
            {
                ModelState.AddModelError("Nombre", "Nombre es obligatorio");
                return View(categoria);
            }



            using(var conexion = new SqlConnection(_connectionString))
            {
                var sqlInsert = "insert into Categorias (Nombre, Descripcion) values (@Nombre , @Descripcion)";
                using (var command = new SqlCommand(sqlInsert, conexion)) 
                {
                    command.Parameters.AddWithValue("@Nombre", categoria.Nombre);
                    command.Parameters.AddWithValue("@Descripcion", (object)categoria.Descripcion ?? System.DBNull.Value);

                    conexion.Open();
                    command.ExecuteNonQuery();


                }
            }

            TempData["SuccessMessage"] = "Categoria Guardada Correctamente .";
            return RedirectToAction("Index");

        }
        public IActionResult Edit(int id)
        {
            Categoria categoria = null;

            using (var conexion = new SqlConnection(_connectionString))
            {
                var sql = "select ID, Nombre, Descripcion from Categorias where ID=@ID";

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
                return NotFound();
            }


            return View(categoria);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Categoria categoria)
        {

            if (categoria == null || string.IsNullOrWhiteSpace(categoria.Nombre))
            {
                ModelState.AddModelError("Nombre", "Nombre es obligatorio");
                return View(categoria);
            }


            using (var conexion = new SqlConnection(_connectionString))
            {

                var sqlUpdate = @"update Categorias 
                          set Nombre=@Nombre,
                              Descripcion=@Descripcion
                          where ID=@ID";


                using (var comando = new SqlCommand(sqlUpdate, conexion))
                {

                    comando.Parameters.AddWithValue("@ID", categoria.ID);
                    comando.Parameters.AddWithValue("@Nombre", categoria.Nombre);
                    comando.Parameters.AddWithValue("@Descripcion",
                        (object)categoria.Descripcion ?? DBNull.Value);


                    conexion.Open();

                    comando.ExecuteNonQuery();

                }

            }


            TempData["SuccessMessage"] = "Categoría actualizada correctamente.";

            return RedirectToAction("Index");
        }

        public IActionResult Delete(int id)
        {

            using (var conexion = new SqlConnection(_connectionString))
            {
                var sqlDelete = "delete from Categorias where ID=@ID";
                using (var comando = new SqlCommand(sqlDelete, conexion))
                {

                    comando.Parameters.AddWithValue("@ID", id);


                    conexion.Open();

                    comando.ExecuteNonQuery();

                }

            }


            TempData["SuccessMessage"] = "Categoría eliminada correctamente.";


            return RedirectToAction("Index");
        }


    }



}
