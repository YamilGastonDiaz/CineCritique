using System.ComponentModel.DataAnnotations;

namespace CineCritique.Models
{
    public class Review
    {
        public int Id { get; set; }
        [MaxLength(500)]
        public string Comentario { get; set; }
        [Range(1, 5)]
        public int Rating { get; set; }
        [Required]
        [DataType(DataType.Date)]
        public DateTime FechaReview { get; set; }
        public string UsuarioId { get; set; }
        public Usuario? Usuario { get; set; }
        public int PeliculaId { get; set; }
        public Pelicula? Pelicula { get; set; }

        //row version for currency control
        [Timestamp]
        public byte[] RowVersion { get; set; }
    }

    public class ReviewCreateViewModel
    {
        public int? Id { get; set; }
        [StringLength(500, ErrorMessage = "El comentario no debe exceder 500 caracteres")]
        [Required]
        public string Comentario { get; set; } = string.Empty;
        [Range(1, 5, ErrorMessage = "la clasificacion debe ser entre 1 y 5 estrellas")]
        [Required]
        public int Rating { get; set; }
        public int PeliculaId { get; set; }
        public string? PeliculaTitulo { get; set; }
        public string UsuarioId { get; set; } = string.Empty;
    }
}
