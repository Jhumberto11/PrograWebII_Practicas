using Biblio.Models;

namespace Biblio.Repositories
{
    public interface IAutoresService
    {
        public ICollection<Autor> GetAllAutors();
        public Autor GetAutor(int id);
        public int CountAutors();
        public void AddAutor(Autor autor);
        public void Remove(Autor autor);

    }
}
