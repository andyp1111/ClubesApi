using System.ComponentModel.DataAnnotations;
namespace ClubesApi.DTOs
{
    public class LoginRequest
    {
        public string Username { get; set; } 
        public string Password { get; set; } 
    }
}
