public class TipoActividadDTO
{
    public string NombreTipo {get; set;}
    public decimal CaloriasPorMinuto {get; set;}
    public int Total {get; set;}
    public int TotalRegistros {get; set;}
    
    public List<ActividadDTO> Actividades {get; set;}
   
   







    public int TotalMayor30 { get; set; }
    public int TotalMenor30 { get; set; }
    public int porcentajeIntensivas { get; set; }
       // Totales del tipo
    public double PromedioTiempo { get; set; }
    public int DuracionTotal { get; set; }
    public decimal TotalCalorias { get; set; }
    public int TotalActividades { get; set; }
    public DateTime? FechaUltimaActividad { get; set; }
   
    public int TotalMinutos {get; set;}
    //public decimal TotalCalorias {get; set;}
    public int CantidadActividades {get; set;}
    

}

public class FechaActividadDTO
{
    public DateTime Fecha { get; set; }
    public List<TipoActividadDTO> TiposActividad { get; set; }
}