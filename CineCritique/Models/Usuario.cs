using Microsoft.AspNetCore.Identity;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace CineCritique.Models
{
    public class Usuario : IdentityUser
    {
        [Required]
        [StringLength(50)]
        public string Nombre { get; set; }
        [Required]
        [StringLength(50)]
        public string Apellido { get; set; }
        [DataType(DataType.Date)]
        public DateTime FechaNacimiento { get; set; }
        public string ImagenUrlPerfil { get; set; }
        public List<Favorito>? PeliculasFavoritas { get; set; }
        public List<Review>? ReviewsUsuario { get; set; }
    }

    public class RegistroViewModel
    {
        [Required]
        [StringLength(50)]
        public string Nombre { get; set; }
        [Required]
        [StringLength(50)]
        public string Apellido { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string Clave { get; set; }
        [Required]
        [DataType(DataType.Password)]
        [Compare("Clave", ErrorMessage = "Las claves deben coincidir")]
        public string ConfirmarClave { get; set; }
    }

    public class LoginViewModel
    {
        [EmailAddress]
        [Required]
        public string Email { get; set; }
        [DataType(DataType.Password)]
        [Required]
        public string Clave { get; set; }
        public bool Recordarme { get; set; }
    }

    public class MiPerfilViewModel
    {
        [Required]
        [StringLength(50)]
        public string Nombre { get; set; }
        [Required]
        [StringLength(50)]
        public string Apellido { get; set; }
        [EmailAddress]
        public string? Email { get; set; }
        public IFormFile? ImagenPerfil { get; set; }
        public string? ImagenUrlPerfil { get; set; }
    }
}
