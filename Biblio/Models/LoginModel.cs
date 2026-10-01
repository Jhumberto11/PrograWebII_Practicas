using System.ComponentModel.DataAnnotations;

namespace Biblio.Models
{
    public class LoginModel
    {
        [Required(ErrorMessage = "El nombre de usuario o correo electrónico es requerido.")]
        [Display(Name = "Usuario o Correo Electrónico")]
        public string UserNameOrEmail { get; set; } = string.Empty;

        [Required(ErrorMessage = "La contraseña es requerida.")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        [Display(Name = "Recuerdame")]
        public bool RememberMe { get; set; } = false;
        public string? ReturnUrl { get; set; }
    }
}
