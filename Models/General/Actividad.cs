using System.ComponentModel.DataAnnotations.Schema;
namespace Final2025.Models.General
{
    public class Actividad
    {
        public int ActividadID { get; set; }
        public int PersonaID { get; set; }
        public int TipoActividadID { get; set; }

        [NotMapped]
        public string FechaString { get { return Fecha.ToString("dd/MM/yyyy") ;} }
        public DateTime Fecha { get; set; }

        public int DuracionMinutos { get; set; }
        public string? Observaciones { get; set; }

        public virtual Persona Persona { get; set; }
        public virtual TipoActividad TipoActividad { get; set; }
    }
}