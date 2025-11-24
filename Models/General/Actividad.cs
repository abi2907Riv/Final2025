using System.ComponentModel.DataAnnotations.Schema;
using NuGet.Common;
namespace Final2025.Models.General
{
    public class Actividad
    {
        public int ActividadID { get; set; }
        public int PersonaID { get; set; }
        [NotMapped]
        public string TipoActividadString { get { return TipoActividad != null ? TipoActividad.Nombre : "Sin categoría"; } }
        public int TipoActividadID { get; set; }

        [NotMapped]
        public string FechaString { get { return Fecha.ToString("dd/MM/yyyy"); } }
        public DateTime Fecha { get; set; }

        public TimeSpan DuracionMinutos { get; set; }
        public string? Observaciones { get; set; }

        public virtual Persona? Persona { get; set; }
        public virtual TipoActividad? TipoActividad { get; set; }
    }

    // public class VistaActividad
    // {
    //     public int ActividadID { get; set; } 
    //     public int? TipoActividadID { get; set; }
    //     public string FechaString { get; set; }
    //     public string Nombre { get; set; }
    //     public int DuracionMinutos { get; set; }

    // }


    //public class FiltroActividad
    //{
        //public int? TipoActividadID { get; set; }
        //public DateTime? FechaActividad { get; set; }
        //public DateTime? FechaDesde { get; set; }   // Fecha inicial del rango
        //public DateTime? FechaHasta { get; set; }
        //public int? DuracionMinutos { get; set; }
    //}
}