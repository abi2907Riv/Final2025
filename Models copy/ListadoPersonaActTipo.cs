public class AgruparXPersona
{
    public string Nombre {get; set;}
    public decimal PesoInicial {get; set;}
    public decimal PesoFinal {get; set;}
    public decimal VariacionPeso {get; set;}
    public int TotalActividades {get; set;}
    public int TiempoTotalMinutos {get; set;}
    public int PromedioDuracion {get; set;}
    public decimal TotalCalorias {get; set;}

    public List<AgruparXTipo> TiposAgrupados {get; set;}
}


public class AgruparXTipo
{
    public string NombreActividad {get; set;}
    public int TotalActividades {get; set;}
    public int TiempoTotalMinutos{get; set;}
    public int PromedioDuracion {get; set;}
    public decimal TotalCalorias {get; set;}
    public decimal VariacionPeso {get; set;}
    public decimal PesoFinal {get; set;}
}