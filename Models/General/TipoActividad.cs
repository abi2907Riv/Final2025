namespace Final2025.Models.General
{
    public class TipoActividad
    {
        public int TipoActividadID { get; set; }
        public string Nombre { get; set; }
        public decimal CaloriasPorMinuto { get; set; }

        public virtual ICollection<Actividad>? Actividades { get; set; }
    }
}