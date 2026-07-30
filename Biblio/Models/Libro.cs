namespace Biblio.Models
{
    public class Libro
    {

        public int ID { get; set; }
        public string Title { get; set; }
        public string Autor { get; set; }
        public int Year { get; set; }

        public string Categoria { get; set; }
        public decimal Precio { get; set; }
        public bool Disponible { get; set; } = true;

        public string? ImageURL { get; set; }

    }
}
