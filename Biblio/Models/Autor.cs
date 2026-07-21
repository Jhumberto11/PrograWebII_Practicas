namespace Biblio.Models
{
    public class Autor
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Lastname { get; set; }
        public string Country { get; set; }
        public DateOnly Birthdate { get; set; }
        public bool IsActive { get; set; } = true;

    }
}
