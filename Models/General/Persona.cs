namespace Final2025.Models.General;

    public class Persona
    {
        public int PersonaID { get; set; }
        public string Nombre { get; set; }
        public DateOnly FechaNacimiento { get; set; }
        public decimal Peso { get; set; }
        public string? UsuarioID { get; set; }

        public virtual ICollection<Actividad> Actividades { get; set; } = new List<Actividad>();
    }

    public class CrearUsuario
    {
        public string? Email { get; set; }
        public Persona? Persona { get; set; }
    }

    // public class FiltroPersona
    // {
    //     public string Nombre { get; set; }
    //     public DateTime? FechaNacimiento { get; set; }
    //     public int? Peso { get; set; }
    // }