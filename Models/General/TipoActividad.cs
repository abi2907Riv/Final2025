namespace Final2025.Models.General;
    public class TipoActividad
    {
        public int TipoActividadID { get; set; }
        public string Nombre { get; set; }
        public decimal CaloriasPorMinuto { get; set; }
        public bool Eliminado { get; set; }

        public virtual ICollection<Actividad>? Actividades { get; set; }
    }

    public class FiltroTipoActividad
    {
        public string Nombre { get; set; }
        public int? Eliminado { get; set; }
        public int? CaloriasPorMinuto { get; set; }
    }
