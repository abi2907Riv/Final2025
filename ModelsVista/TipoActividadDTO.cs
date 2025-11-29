public class TipoActividadDTO
{
    public string NombreTipo {get; set;}
    public decimal CaloriasPorMinuto {get; set;}
   
   
   
   
   
    public int TotalMinutos {get; set;}
    public decimal TotalCalorias {get; set;}
    public int CantidadActividades {get; set;}
    public List<ActividadDTO> Actividades {get; set;}

}