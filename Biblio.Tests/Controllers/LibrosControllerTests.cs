using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Biblio.Controllers;
using Biblio.Data;
using Biblio.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Biblio.Tests.Controllers
{
    public class LibrosControllerTests : IDisposable
    {
        private readonly BiblioContext _context;
        private readonly Mock<ILogger<LibrosController>> _loggerMock;
        private readonly LibrosController _controller;

        public LibrosControllerTests()
        {
            var options = new DbContextOptionsBuilder<BiblioContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            _context = new BiblioContext(options);

            _loggerMock = new Mock<ILogger<LibrosController>>();

            _loggerMock
                .Setup(logger => logger.IsEnabled(It.IsAny<LogLevel>()))
                .Returns(true);

            _controller = new LibrosController(
                _context,
                _loggerMock.Object);
        }

        private static Libro CrearLibro(int id, string titulo)
        {
            return new Libro
            {
                ID = id,
                Title = titulo,
                Autor = "Autor de prueba",
                Categoria = "Novela",
                Precio = 15.50m
            };
        }

        [Fact]
        public async Task Index_CuandoHayLibros_DevuelveElListado()
        {
            await _context.Libros.AddRangeAsync(
                CrearLibro(1, "El principito"),
                CrearLibro(2, "Don Quijote"));

            await _context.SaveChangesAsync();

            var resultado = await _controller.Index();

            var vista = Assert.IsType<ViewResult>(resultado);
            var libros = Assert.IsAssignableFrom<List<Libro>>(vista.Model);

            Assert.Equal(2, libros.Count);
            Assert.Contains(libros, libro => libro.Title == "El principito");
            Assert.Contains(libros, libro => libro.Title == "Don Quijote");
        }

        [Fact]
        public async Task Index_CuandoNoHayLibros_DevuelveListaVacia()
        {
            var resultado = await _controller.Index();

            var vista = Assert.IsType<ViewResult>(resultado);
            var libros = Assert.IsAssignableFrom<List<Libro>>(vista.Model);

            Assert.Empty(libros);
        }

        [Fact]
        public async Task DetailsLibro_CuandoExiste_DevuelveElLibro()
        {
            var libro = CrearLibro(1, "El principito");

            _context.Libros.Add(libro);
            await _context.SaveChangesAsync();

            var resultado = await _controller.DetailsLibro(1);

            var vista = Assert.IsType<ViewResult>(resultado);
            var libroDevuelto = Assert.IsType<Libro>(vista.Model);

            Assert.Equal("El principito", libroDevuelto.Title);
            Assert.Equal(1, libroDevuelto.ID);
        }

        [Theory]
        [InlineData(99)]
        [InlineData(100)]
        public async Task DetailsLibro_CuandoNoExiste_DevuelveNotFound(int id)
        {
            var resultado = await _controller.DetailsLibro(id);

            Assert.IsType<NotFoundResult>(resultado);
        }

        [Fact]
        public async Task Create_CuandoLosDatosSonValidos_GuardaElLibro()
        {
            var libro = CrearLibro(0, "Nuevo libro");

            var resultado = await _controller.Create(libro);

            var redireccion = Assert.IsType<RedirectToActionResult>(resultado);
            var libroGuardado = await _context.Libros.SingleAsync();

            Assert.Equal(nameof(LibrosController.Index), redireccion.ActionName);
            Assert.Equal("Nuevo libro", libroGuardado.Title);
            Assert.Equal("Autor de prueba", libroGuardado.Autor);
            Assert.Equal("Novela", libroGuardado.Categoria);
        }

        [Fact]
        public async Task Create_CuandoElModeloNoEsValido_RegresaAlFormulario()
        {
            var libro = CrearLibro(0, "Libro inválido");

            _controller.ModelState.AddModelError(
                "Title",
                "El título es obligatorio");

            var resultado = await _controller.Create(libro);

            var vista = Assert.IsType<ViewResult>(resultado);

            Assert.Equal("CreateView", vista.ViewName);
            Assert.Same(libro, vista.Model);
            Assert.Empty(await _context.Libros.ToListAsync());
        }

        [Fact]
        public async Task Edit_CuandoElLibroExiste_ActualizaSusDatos()
        {
            var libroOriginal = CrearLibro(1, "Título anterior");

            _context.Libros.Add(libroOriginal);
            await _context.SaveChangesAsync();

            var libroActualizado = CrearLibro(1, "Título actualizado");
            libroActualizado.Autor = "Nuevo autor";
            libroActualizado.Categoria = "Historia";
            libroActualizado.Precio = 25.00m;

            var resultado = await _controller.Edit(libroActualizado);

            var redireccion = Assert.IsType<RedirectToActionResult>(resultado);
            var libroGuardado = await _context.Libros
                .AsNoTracking()
                .SingleAsync(libro => libro.ID == 1);

            Assert.Equal(nameof(LibrosController.Index), redireccion.ActionName);
            Assert.Equal("Título actualizado", libroGuardado.Title);
            Assert.Equal("Nuevo autor", libroGuardado.Autor);
            Assert.Equal("Historia", libroGuardado.Categoria);
            Assert.Equal(25.00m, libroGuardado.Precio);
        }

        [Fact]
        public async Task Edit_CuandoElLibroNoExiste_DevuelveNotFound()
        {
            var libro = CrearLibro(99, "Libro inexistente");

            var resultado = await _controller.Edit(libro);

            Assert.IsType<NotFoundResult>(resultado);
        }

        [Fact]
        public async Task Edit_CuandoElModeloNoEsValido_RegresaAlFormulario()
        {
            var libro = CrearLibro(1, "Libro con datos inválidos");

            _controller.ModelState.AddModelError(
                "Title",
                "El título no es válido");

            var resultado = await _controller.Edit(libro);

            var vista = Assert.IsType<ViewResult>(resultado);

            Assert.Equal("EditViewLibro", vista.ViewName);
            Assert.Same(libro, vista.Model);
            Assert.Empty(await _context.Libros.ToListAsync());
        }

        [Fact]
        public async Task Delete_CuandoElLibroExiste_EliminaElRegistro()
        {
            var libro = CrearLibro(1, "Libro para eliminar");

            _context.Libros.Add(libro);
            await _context.SaveChangesAsync();

            var resultado = await _controller.Delete(1);

            var redireccion = Assert.IsType<RedirectToActionResult>(resultado);
            var existe = await _context.Libros.AnyAsync(libro => libro.ID == 1);

            Assert.Equal(nameof(LibrosController.Index), redireccion.ActionName);
            Assert.False(existe);
        }

        [Fact]
        public async Task Delete_CuandoElLibroNoExiste_DevuelveNotFound()
        {
            var resultado = await _controller.Delete(99);

            Assert.IsType<NotFoundResult>(resultado);
        }

        [Fact]
        public async Task Index_RegistraUnMensajeDeInformacion()
        {
            await _controller.Index();

            _loggerMock.Verify(
                logger => logger.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((estado, tipo) =>
                        estado.ToString()!.Contains("Se inició la consulta de libros")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
