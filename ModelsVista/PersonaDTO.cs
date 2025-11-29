using Final2025.Models.General;

public class PersonasDTO
{
    public string Nombre {get; set;}
    public string Email {get; set;}
    public List<TipoActividadDTO> TiposActividad {get; set;}




    public int TotalMinutos { get; set; }
    public int CantidadActividades { get; set; }
    public double PromedioMinutos { get; set; }
    public double PromedioCalorias { get; set; }
    public decimal TotalCalorias { get; set; }
}