namespace ClubesApi.Models
{
    public class Dirigente
    {
        public int DirigenteId { get; set; } 
        public int ClubId { get; set; }      
        public string Nombre { get; set; } = string.Empty;
        public string Apellido { get; set; } = string.Empty;
        public DateTime FechaNacimiento { get; set; }
        public string Rol { get; set; } = string.Empty;
        public int Dni { get; set; }
    }
}
