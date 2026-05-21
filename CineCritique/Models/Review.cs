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
}
