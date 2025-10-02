using System.ComponentModel.DataAnnotations;

namespace ClubesApi.DTOs
{
    public class DirigenteCreateDto
    {
        [Required]
        public int ClubId { get; set; }
        [Required]
        [MaxLength(80)]
        public string Nombre { get; set; }
        [Required]
        [MaxLength(80)]
        public string Apellido { get; set; }
        public DateTime FechaNacimiento { get; set; }
        [Required]
        [MaxLength(80)]
        public string Rol { get; set; }
        [Required]
        public int Dni { get; set; }
    }
}
