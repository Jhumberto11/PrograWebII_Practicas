using System.ComponentModel.DataAnnotations;

namespace Biblio.Models
{
    public class Autor
    {
        public int Id { get; set; }
        [Required]
        [StringLength(100)]
        public string Name { get; set; }
        public string Lastname { get; set; }
        [StringLength(50)]
        public string Country { get; set; }
        [DataType(DataType.Date)]
        public DateOnly Birthdate { get; set; }
        
        public bool IsActive { get; set; } = true;

    }
}
