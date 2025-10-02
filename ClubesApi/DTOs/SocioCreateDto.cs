using System.ComponentModel.DataAnnotations;

namespace ClubesApi.DTOs
{
    public class SocioCreateDto
    {
        [Required]
        public int ClubId { get; set; }

        [Required]
        [MaxLength(80)]
        public string Nombre { get; set; }

        [Required]
        [MaxLength(80)]
        public string Apellido { get; set; }

        [Required]
        public DateTime FechaNacimiento { get; set; }

        [Required]
        public DateTime FechaAsociado { get; set; }

        [Required]
        public int Dni { get; set; }
    }
}