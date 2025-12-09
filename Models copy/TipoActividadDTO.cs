public class TipoActividadDTO
{
    public string NombreTipo {get; set;}
    public decimal CaloriasPorMinuto {get; set;}
   
   
   
       // Totales del tipo
    public double PromedioTiempo { get; set; }
    public int DuracionTotal { get; set; }
    public decimal TotalCalorias { get; set; }
    public int TotalActividades { get; set; }
    public DateTime? FechaUltimaActividad { get; set; }
   
    public int TotalMinutos {get; set;}
    //public decimal TotalCalorias {get; set;}
    public int CantidadActividades {get; set;}
    public List<ActividadDTO> Actividades {get; set;}

}