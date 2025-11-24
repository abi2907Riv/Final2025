
namespace Final2025.Models.Users
{
    public class Register
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string NombreCompleto { get; set; } = string.Empty;

        public DateOnly FechaNacimiento { get; set; } 
        public decimal Peso { get; set; } 
    }
}
