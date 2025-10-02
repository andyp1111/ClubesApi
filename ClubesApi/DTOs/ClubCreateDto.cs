using System.ComponentModel.DataAnnotations;

namespace ClubesApi.DTOs
{
    public class ClubCreateDto
    {
        [Required]
        [MaxLength(100)]
        public string Nombre { get; set; }

        [Required]
        public DateTime FechaFundacion { get; set; }

        [MaxLength(150)]
        public string? UbicacionEstadio { get; set; }

        [MaxLength(100)]
        public string? NombreEstadio { get; set; }
    }
}