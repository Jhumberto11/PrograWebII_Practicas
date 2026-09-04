using Biblio.Models;

namespace Biblio.Repositories
{
    public class AutorRepositoryAlternativo : IAutoresService
    {
        public static List<Autor> autores = new List<Autor>()
    {
        new Autor()
        {
            Id = 1,
            Name = "Julio",
            Lastname = "Verne",
            Country = "Francia",
            Birthdate = new DateOnly(1828, 2, 8)
        },

        new Autor()
        {
            Id = 2,
            Name = "George",
            Lastname = "Orwell",
            Country = "Reino Unido",
            Birthdate = new DateOnly(1903, 6, 25)
        },

        new Autor()
        {
            Id = 3,
            Name = "Jane",
            Lastname = "Austen",
            Country = "Reino Unido",
            Birthdate = new DateOnly(1775, 12, 16)
        }
    };


        public ICollection<Autor> GetAllAutors()
        {
            return autores;
        }


        public Autor GetAutor(int id)
        {
            var autor = autores.FirstOrDefault(a => a.Id == id);

            if (autor == null)
                return null;

            return autor;
        }


        public int CountAutors()
        {
            if (autores.Count == 0)
                return 1;

            return autores.Max(a => a.Id) + 1;
        }


        public void AddAutor(Autor autor)
        {
            if (autor != null)
            {
                autores.Add(autor);
            }
        }


        public void Remove(Autor autor)
        {
            autores.Remove(autor);
        }
    }
}
