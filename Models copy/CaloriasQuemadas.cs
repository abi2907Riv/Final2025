public class PersonaDTO
{
    public string Nombre { get; set; }
    public decimal Peso { get; set; }
    public string Email { get; set; }
    public double MinutosTotales { get; set; }
    public decimal CaloriasRealesTotales { get; set; }
    public int CantidadActividades { get; set; }
    public List<TiposDTO> TiposActividades { get; set; } = new List<TiposDTO>();
}

public class TiposDTO
{
    public string NombreTipo { get; set; }
    public double MinutosTotales { get; set; }
    public decimal CaloriasRealesTotales { get; set; }
    public List<ActividadesDTO> Actividades { get; set; } = new List<ActividadesDTO>(); 
}

public class ActividadesDTO
{
    public DateTime Fecha { get; set; }
    public double DuracionMinutos { get; set; }
    public decimal CaloriasReales { get; set; }
    public string Observaciones { get; set; }
}


public class CalcularCalorias
{
    public DateTime Fecha { get; set; }
    public decimal Calorias { get; set; }
}