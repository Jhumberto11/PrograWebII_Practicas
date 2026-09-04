using Biblio.Models;

namespace Biblio.Repositories
{
    public interface IReposotoryLibro
    {
        IEnumerable<Libro> GetAll();

    }
}