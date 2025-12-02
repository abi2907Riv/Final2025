using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Final2025.Models.General;
// using Final2025.ModelsVista;
using System.Security.Claims;

namespace Final2025.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EstadisticasController : ControllerBase
    {
        private readonly Context _context;

        public EstadisticasController

        (Context context)
        {
            _context = context;
        }



        /////////////////////////////////INFORMES/////////////////////////////////
        ///////////////////////////////////////////////INFORMES////////////////////////////////////////////////
        /// Muestra las actividades realizadas de cada persona, calculando ademas 
        /// las calorias quemadas al reralizar esa actividad
        [HttpPost("InformeActividadXPersona")]
        public async Task<ActionResult<IEnumerable<PersonasDTO>>> InformeActividadXPersona([FromBody] FiltroActividad filtro)
        {
            //Obtengo el ID del usuario logueado
            var userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            //Trae todas las personas
            var personas = await _context.Personas.ToListAsync();
            //.Where(p => p.UsuarioID == userId)

            // Lista final que tendra los resultados finales
            List<PersonasDTO> personasMostrar = new List<PersonasDTO>();

            foreach (var persona in personas)
            {
                // Compara el Id de la tabla Users con el UsuarioID de la tabla Persona, y luego trae el Email de esa persona.
                var email = await _context.Users
                    .Where(u => u.Id == persona.UsuarioID)
                    .Select(u => u.Email)
                    .FirstOrDefaultAsync();

                //Busco las actividades pertenecientes a esa persona, ordenadas de mayor a menos.
                var actividades = _context.Actividades
                .Where(a => a.PersonaID == persona.PersonaID)
                .OrderByDescending(a => a.Fecha)
                .Include(a => a.TipoActividad).AsQueryable();

                //FILTRO POR PERSONA
                if (filtro.PersonaID > 0)
                    actividades = actividades.Where(a => a.PersonaID == filtro.PersonaID);
                //FILTRO POR TIPO ACTIVIDAD
                if (filtro.TipoActividadID > 0)
                    actividades = actividades.Where(a => a.TipoActividadID == filtro.TipoActividadID);
                //FILTRO POR FECHA DESDE Y HASTA
                DateTime fechaDesde = new DateTime();
                bool fechaDesdeValido = !string.IsNullOrEmpty(filtro.FechaDesde) && DateTime.TryParse(filtro.FechaDesde, out fechaDesde);
                DateTime fechaHasta = new DateTime();
                bool fechaHastaValido = !string.IsNullOrEmpty(filtro.FechaHasta) && DateTime.TryParse(filtro.FechaHasta, out fechaHasta);
                // Si ambas fechas son válidas, filtrar actividades dentro del rango
                if (fechaDesdeValido && fechaHastaValido)
                    actividades = actividades.Where(a => a.Fecha >= fechaDesde && a.Fecha <= fechaHasta);
                else if (fechaDesdeValido) // Solo desde
                    actividades = actividades.Where(a => a.Fecha >= fechaDesde);
                else if (fechaHastaValido) // Solo hasta
                    actividades = actividades.Where(a => a.Fecha <= fechaHasta);
                //FILTRO POR FECHA 
                if (!string.IsNullOrEmpty(filtro.FechaActividad))
                {
                    if (DateTime.TryParse(filtro.FechaActividad, out DateTime fechaFiltro))
                    {
                        actividades = actividades.Where(a => a.Fecha.Date == fechaFiltro.Date);
                    }
                }
                //FILTRO POR DURACION
                if (filtro.DuracionMinutos.HasValue)
                {
                    actividades = actividades.Where(c => c.DuracionMinutos == filtro.DuracionMinutos.Value);
                }

                var actividadesFiltradas = await actividades.ToListAsync();
                //FILTRO POR CALORIAS QUEMADAS
                if (filtro.CaloriasTotales.HasValue)
                {
                    actividadesFiltradas = actividadesFiltradas
                        .Where(a => (int)a.DuracionMinutos.TotalMinutes * a.TipoActividad.CaloriasPorMinuto
                                    == filtro.CaloriasTotales.Value)
                        .ToList();
                }
                //si esa persona no tiene actividades no muestra su email ni nombre
                if (!actividadesFiltradas.Any())
                    continue;

                //Agrupa las actividades de la persona segun su  tipo de actividad
                var tipoActividadAgrupado = actividadesFiltradas
                .GroupBy(a => a.TipoActividadID);

                //Se crea una lista que contendra los tipos de actividades a mostrar
                List<TipoActividadDTO> tiposActividadMostrar = new List<TipoActividadDTO>();
                // Recorre cada grupo de actividades 
                foreach (var actividad in tipoActividadAgrupado)
                {
                    // Obtiene el primer TipoActividad del grupo 
                    var tiposActividad = actividad.Select(a => a.TipoActividad).First();
                    // Agrega a la lista este tipo de actividad, poniendo su nombre, sus calorías y todas 
                    // las actividades que pertenecen a este tipo.
                    tiposActividadMostrar.Add(new TipoActividadDTO
                    {
                        NombreTipo = tiposActividad.Nombre,
                        CaloriasPorMinuto = tiposActividad.CaloriasPorMinuto,
                        Actividades = actividad
                        .Select(a => new ActividadDTO
                        {
                            Fecha = a.Fecha,
                            DuracionMinutos = (int)a.DuracionMinutos.TotalMinutes, // Convierte TimeSpan a minutos
                            CaloriasQuemadas = (int)a.DuracionMinutos.TotalMinutes * a.TipoActividad.CaloriasPorMinuto
                        })
                    .ToList()
                    });
                }
                //Agrega al listado de persona las actividades asociadas a esa persona, y muestra el nombre y email
                personasMostrar.Add(new PersonasDTO
                {
                    Nombre = persona.Nombre,
                    Email = email,
                    TiposActividad = tiposActividadMostrar
                });
            }
            return Ok(personasMostrar);
        }


        ///Permite seleccionar la persona mediante un select y trae las actividades de esa persona
        [HttpPost("FiltrarPersona")]
        public async Task<ActionResult<IEnumerable<Actividad>>> FiltroPersonaActividad([FromBody] FiltroPersonaActividad filtro)
        {
            //Trae todas las personas, filtrando las actividades de esa personas para que solo se 
            //vean las suyas al selecciona la persona.
            var persona = await _context.Personas
            .Where(p => p.PersonaID == filtro.PersonaID)
            .FirstOrDefaultAsync();
            // Obtener todas las actividades incluidos los tipos de actividad
            var actividades = await _context.Actividades
            .Where(a => a.PersonaID == filtro.PersonaID)
            .Include(a => a.TipoActividad).ToListAsync();
            // solo filtra si se seleccionó una persona específica
            if (filtro.PersonaID > 0)
                actividades = actividades.Where(a => a.PersonaID == filtro.PersonaID).ToList();
            return Ok(actividades);
        }



        /// Muestra cada tipo de actividad y dentro de cada uno lista todas las actividades 
        /// que pertenecen a ese tipo, indicando su duración y las calorías quemadas 
        /// (duración × calorías por minuto)
        [HttpPost("InformeActividadXTipoActividad")]
        public async Task<ActionResult<IEnumerable<TipoActividadDTO>>> InformeActividadXTipoActividad()
        {
            //Trae todos los tipos de actividad
            var tipoActividad = await _context.TipoActividades.ToListAsync();
            //Trae todas las actividades con los tipos de actividades
            var actividades = await _context.Actividades.Include(a => a.TipoActividad).ToListAsync();
            //Crea una lista que contendra los tipos de actividades
            List<TipoActividadDTO> tiposActividadMostrar = new List<TipoActividadDTO>();
            foreach (var tiposActividades in tipoActividad)
            {
                // Filtra las actividades que pertenecen a este tipo
                var actividadesDelTipo = actividades
                .Where(a => a.TipoActividadID == tiposActividades.TipoActividadID)
               .Select(t => new ActividadDTO
               {
                   Fecha = t.Fecha,
                   DuracionMinutos = (int)t.DuracionMinutos.TotalMinutes,
                   CaloriasQuemadas = (int)t.DuracionMinutos.TotalMinutes * t.TipoActividad.CaloriasPorMinuto
               })
               .ToList();
                // Si no tiene actividades, no se muestr
                if (!actividadesDelTipo.Any())
                    continue;
                // Agrega el tipo y sus actividades
                tiposActividadMostrar.Add(new TipoActividadDTO
                {
                    NombreTipo = tiposActividades.Nombre,
                    CaloriasPorMinuto = tiposActividades.CaloriasPorMinuto,
                    Actividades = actividadesDelTipo
                });
            }
            return Ok(tiposActividadMostrar);
        }



        ///Por cada persona, lista los tipos de actividad que realizó, mostrando cuántas actividades 
        //hizo de cada tipo, el total de minutos acumulados y las calorías totales quemadas
        [HttpPost("InformeActividadXPersonaYTipoAcividad")]
        public async Task<ActionResult<IEnumerable<PersonasDTO>>> InformeActividadXPersonaYTipoAcividad()
        {
            //Obtengo el ID del usuario logueado
            var userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            //Trae todas las personas
            var personas = await _context.Personas.ToListAsync();
            //.Where(p => p.UsuarioID == userId)

            // Lista final que tendra los resultados finales
            List<PersonasDTO> personasMostrar = new List<PersonasDTO>();

            foreach (var persona in personas)
            {
                var email = await _context.Users
                    .Where(u => u.Id == persona.UsuarioID)
                    .Select(u => u.Email)
                    .FirstOrDefaultAsync();

                //Trae las actividades de cada persona
                var actividades = await _context.Actividades
                .Where(a => a.PersonaID == persona.PersonaID)
                .OrderByDescending(a => a.Fecha)
                    .Include(a => a.TipoActividad).ToListAsync();

                // if (!actividades.Any())
                //     continue;
                // Agrupa las actividades por el tipo (correr, nadar, caminar, etc.)
                var tipoActividadAgrupado = actividades
                .GroupBy(a => a.TipoActividadID);

                List<TipoActividadDTO> tiposActividadMostrar = new List<TipoActividadDTO>();

                foreach (var tipoActividades in tipoActividadAgrupado)
                {
                    var tiposActividad = tipoActividades.Select(a => a.TipoActividad).First();
                    // Suma todos los minutos realizados de este tipo
                    var totalMinutos = tipoActividades.Sum(a => (int)a.DuracionMinutos.TotalMinutes);
                    // Suma total de calorías (minutos × caloríasPorMinuto)
                    var totalCalorias = (int)Math.Round(tipoActividades.Sum(a =>
                        (int)a.DuracionMinutos.TotalMinutes * a.TipoActividad.CaloriasPorMinuto));
                    // Cantidad de actividades registradas de este tipo
                    var cantidadActividades = tipoActividades.Count();

                    tiposActividadMostrar.Add(new TipoActividadDTO
                    {
                        NombreTipo = tiposActividad.Nombre,
                        TotalMinutos = totalMinutos,
                        TotalCalorias = totalCalorias,
                        CantidadActividades = cantidadActividades
                    });
                }
                personasMostrar.Add(new PersonasDTO
                {
                    Nombre = persona.Nombre,
                    Email = email,
                    TiposActividad = tiposActividadMostrar
                });
            }
            return Ok(personasMostrar);
        }


        //Nombre de la persona, total de minutos acumulados en todas las actividades, 
        //cantidad de actividades realizadas y el promedio de minutos por actividad
        [HttpPost("DuracionTotalActividadesPersona")]
        public async Task<ActionResult<IEnumerable<PersonasDTO>>> DuracionTotalActividadesPersona()
        {
            //Obtengo el ID del usuario logueado
            var userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            //Trae todas las personas
            var personas = await _context.Personas.ToListAsync();
            //.Where(p => p.UsuarioID == userId)

            List<PersonasDTO> personasMostrar = new List<PersonasDTO>();

            foreach (var persona in personas)
            {
                var email = await _context.Users
                    .Where(u => u.Id == persona.UsuarioID)
                    .Select(u => u.Email)
                    .FirstOrDefaultAsync();

                //Busco las actividades que le pertenecenn a esa persona
                var actividades = await _context.Actividades
                .Where(a => a.PersonaID == persona.PersonaID && a.TipoActividad.Eliminado == false)
                .OrderByDescending(a => a.Fecha)
                    .Include(a => a.TipoActividad).ToListAsync();
                //Suma los minutos de todas las actividades realizadas, sin dividir por tipo
                var totalMinutos = actividades.Sum(a => (int)a.DuracionMinutos.TotalMinutes);
                // Cantidad de actividades registradas de este tipo
                var cantidadActividades = actividades.Count();
                //Obtiene el promedio del total de minutos
                var promedio = Math.Round(cantidadActividades > 0 ? totalMinutos / (double)cantidadActividades : 0);

                if (!actividades.Any())
                    continue;

                personasMostrar.Add(new PersonasDTO
                {
                    Nombre = persona.Nombre,
                    Email = email,
                    TotalMinutos = totalMinutos,
                    CantidadActividades = cantidadActividades,
                    PromedioMinutos = promedio
                });
            }
            return Ok(personasMostrar);
        }



        //Agrupas las actividades por persona, sumando el total de minutos de tods las 
        // actividades y el total de calorias quemadas de todas las actividades
        [HttpPost("InformeActividadXPersonaCantidad")]
        public async Task<ActionResult<IEnumerable<PersonasDTO>>> InformeActividadXPersonaCantidad([FromBody] FiltroActividad filtro)
        {
            //Obtengo el ID del usuario logueado
            var userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            //Trae todas las personas
            var personas = await _context.Personas
                //.Where(p => p.UsuarioID == userId)
                .ToListAsync();
            // Lista final que tendra los resultados finales
            List<PersonasDTO> personasMostrar = new List<PersonasDTO>();

            foreach (var persona in personas)
            {
                var email = await _context.Users
                    .Where(u => u.Id == persona.UsuarioID)
                    .Select(u => u.Email)
                    .FirstOrDefaultAsync();

                //Busco las actividades que le pertenecenn a esa persona
                var actividades = _context.Actividades
                .Where(a => a.PersonaID == persona.PersonaID)
                .OrderByDescending(a => a.Fecha)
                    .Include(a => a.TipoActividad).AsQueryable();

                //FILTROS
                if (filtro.PersonaID > 0)
                    actividades = actividades.Where(a => a.PersonaID == filtro.PersonaID);

                if (filtro.TipoActividadID > 0)
                    actividades = actividades.Where(a => a.TipoActividadID == filtro.TipoActividadID);

                // Filtrar por rango de fechas de actividades
                DateTime fechaDesde = new DateTime();
                bool fechaDesdeValido = !string.IsNullOrEmpty(filtro.FechaDesde) && DateTime.TryParse(filtro.FechaDesde, out fechaDesde);
                DateTime fechaHasta = new DateTime();
                bool fechaHastaValido = !string.IsNullOrEmpty(filtro.FechaHasta) && DateTime.TryParse(filtro.FechaHasta, out fechaHasta);
                // Si ambas fechas son válidas, filtrar actividades dentro del rango
                if (fechaDesdeValido && fechaHastaValido)
                    actividades = actividades.Where(a => a.Fecha >= fechaDesde && a.Fecha <= fechaHasta);
                else if (fechaDesdeValido) // Solo desde
                    actividades = actividades.Where(a => a.Fecha >= fechaDesde);
                else if (fechaHastaValido) // Solo hasta
                    actividades = actividades.Where(a => a.Fecha <= fechaHasta);

                var actividadesFiltradas = await actividades.ToListAsync();

                if (!actividadesFiltradas.Any())
                    continue;
                //Agrupo actividades por TipoActividad
                var tipoActividadAgrupado = actividadesFiltradas
                .GroupBy(a => a.TipoActividadID);

                List<TipoActividadDTO> tiposActividadMostrar = new List<TipoActividadDTO>();

                foreach (var actividad in tipoActividadAgrupado)
                {
                    var tiposActividad = actividad.Select(a => a.TipoActividad).First();
                    tiposActividadMostrar.Add(new TipoActividadDTO
                    {
                        NombreTipo = tiposActividad.Nombre,
                        CaloriasPorMinuto = tiposActividad.CaloriasPorMinuto,
                        Actividades = actividad
                        .Select(a => new ActividadDTO
                        {
                            Fecha = a.Fecha,
                            DuracionMinutos = (int)a.DuracionMinutos.TotalMinutes,
                            CaloriasQuemadas = (int)a.DuracionMinutos.TotalMinutes * a.TipoActividad.CaloriasPorMinuto
                        })
                    .ToList()
                    });
                }
                personasMostrar.Add(new PersonasDTO
                {
                    Nombre = persona.Nombre,
                    Email = email,
                    TiposActividad = tiposActividadMostrar,
                    TotalMinutos = actividadesFiltradas.Sum(a => (int)a.DuracionMinutos.TotalMinutes),
                    TotalCalorias = (int)actividadesFiltradas.Sum(a => (int)a.DuracionMinutos.TotalMinutes * a.TipoActividad.CaloriasPorMinuto)
                });
            }
            return Ok(personasMostrar);
        }




        //Contar el número de actividades realizadas por cada persona y sumando los minutos y calorias totales.
        [HttpPost("InformeTotalAactividadesXPersona")]
        public async Task<ActionResult<IEnumerable<PersonasDTO>>> InformeTotalAactividadesXPersona([FromBody] FiltroActividad filtro)
        {
            var personas = await _context.Personas.ToListAsync();

            List<PersonasDTO> personasMostrar = new List<PersonasDTO>();
            foreach (var persona in personas)
            {
                var email = await _context.Users
                .Where(u => u.Id == persona.UsuarioID)
                .Select(u => u.Email)
                .FirstOrDefaultAsync();

                var actividades = _context.Actividades
                .Where(a => a.PersonaID == persona.PersonaID)
                .Include(a => a.TipoActividad).AsQueryable();

                //FILTRO POR PERSONA
                if (filtro.PersonaID > 0)
                    actividades = actividades.Where(a => a.PersonaID == filtro.PersonaID);

                var listaActividades = await actividades.ToListAsync();

                var totalMinutos = listaActividades.Any()
                    ? listaActividades.Sum(a => (int)a.DuracionMinutos.TotalMinutes)
                    : 0;

                var cantidadActividades = listaActividades.Count;

                var caloriasTotales = listaActividades.Any()
                    ? listaActividades.Sum(a =>
                        (decimal)a.DuracionMinutos.TotalMinutes * a.TipoActividad.CaloriasPorMinuto)
                    : 0;
                if (!actividades.Any())
                    continue;
                personasMostrar.Add(new PersonasDTO
                {
                    Nombre = persona.Nombre,
                    Email = email,
                    TotalMinutos = totalMinutos,
                    CantidadActividades = cantidadActividades,
                    TotalCalorias = caloriasTotales
                });
            }
            return Ok(personasMostrar);
        }


        //Calcular el promedio de duración de las actividades de cada persona.
        [HttpPost("DuracionPromedioActividadPorPersona")]
        public async Task<ActionResult<IEnumerable<PersonasDTO>>> DuracionPromedioActividadPorPersona()
        {
            //Obtengo el ID del usuario logueado
            var userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            //Trae todas las personas
            var personas = await _context.Personas.ToListAsync();
            //.Where(p => p.UsuarioID == userId)

            List<PersonasDTO> personasMostrar = new List<PersonasDTO>();

            foreach (var persona in personas)
            {
                var email = await _context.Users
                    .Where(u => u.Id == persona.UsuarioID)
                    .Select(u => u.Email)
                    .FirstOrDefaultAsync();

                //Busco las actividades que le pertenecenn a esa persona
                var actividades = await _context.Actividades
                .Where(a => a.PersonaID == persona.PersonaID && a.TipoActividad.Eliminado == false)
                .OrderByDescending(a => a.Fecha)
                    .Include(a => a.TipoActividad).ToListAsync();
                //Suma los minutos de todas las actividades realizadas, sin dividir por tipo
                var totalMinutos = actividades.Sum(a => (int)a.DuracionMinutos.TotalMinutes);
                var totalCalorias = (int)Math.Round(actividades.Sum(a =>
                        (int)a.DuracionMinutos.TotalMinutes * a.TipoActividad.CaloriasPorMinuto));
                // Cantidad de actividades registradas de este tipo
                var cantidadActividades = actividades.Count();
                //Obtiene el promedio del total de minutos
                var promedioMinutos = Math.Round(cantidadActividades > 0 ? totalMinutos / (double)cantidadActividades : 0);
                var promedioCalorias = Math.Round(cantidadActividades > 0 ? totalCalorias / (double)cantidadActividades : 0);
                if (!actividades.Any())
                    continue;

                personasMostrar.Add(new PersonasDTO
                {
                    Nombre = persona.Nombre,
                    Email = email,
                    PromedioMinutos = promedioMinutos,
                    PromedioCalorias = promedioCalorias
                });
            }
            return Ok(personasMostrar);
        }



        //Informe de actividades agrupadas por tipo de actividad
        //Ver cuántas actividades de cada tipo se han realizado.
        [HttpPost("ActividadesAgrupadasXTipoActividad")]
        public async Task<ActionResult<IEnumerable<PersonasDTO>>> ActividadesAgrupadasXTipoActividad()
        {
            var tipoActividad = await _context.TipoActividades.ToListAsync();
            var actividades = await _context.Actividades
            .Include(a => a.TipoActividad)
            .ToListAsync();

            List<TipoActividadDTO> tipoActividadMostrar = new List<TipoActividadDTO>();
            foreach (var tiposActividades in tipoActividad)
            {
                var actTipos = actividades
                .Where(a => a.TipoActividadID == tiposActividades.TipoActividadID)
                .ToList();

                if (!actTipos.Any())
                    continue;

                var totalActividades = actTipos.Count();
                var totalMinutos = actTipos.Sum(a => (int)a.DuracionMinutos.TotalMinutes);
                var totalCalorias = actTipos.Sum(a => (decimal)a.DuracionMinutos.TotalMinutes * a.TipoActividad.CaloriasPorMinuto);


                tipoActividadMostrar.Add(new TipoActividadDTO
                {
                    NombreTipo = tiposActividades.Nombre,
                    CantidadActividades = totalActividades,
                    TotalMinutos = totalMinutos,
                    TotalCalorias = totalCalorias
                });
            }
            return Ok(tipoActividadMostrar);
        }


        [HttpPost("DetalleActividades")]
        public async Task<ActionResult<IEnumerable<PersonasDTO>>> DetalleActividades([FromBody] FiltroActividad filtro)
        {
            var usuarios = await _context.Users.ToListAsync();
            var personas = await _context.Personas.ToListAsync();
            var actividades = await _context.Actividades.Include(a => a.TipoActividad).ToListAsync();
            List<PersonasDTO> personasMostrar = new List<PersonasDTO>();

            foreach (var persona in personas)
            {
                var email = usuarios
                .Where(u => u.Id == persona.UsuarioID)
                .Select(u => u.Email)
                .FirstOrDefault();

                var actividadesPersona = actividades
                .Where(a => a.PersonaID == persona.PersonaID).AsQueryable();

                if (filtro.PersonaID > 0)
                    actividadesPersona = actividadesPersona.Where(a => a.PersonaID == filtro.PersonaID);
                //FILTRO POR TIPO ACTIVIDAD
                if (filtro.TipoActividadID > 0)
                    actividadesPersona = actividadesPersona.Where(a => a.TipoActividadID == filtro.TipoActividadID);

                if (!actividadesPersona.Any())
                    continue;

                var tipoActividadAgrupado = actividadesPersona
                    .GroupBy(a => a.TipoActividadID);

                List<TipoActividadDTO> tiposActividadMostrar = new List<TipoActividadDTO>();
                // Recorre cada grupo de actividades 
                foreach (var actividad in tipoActividadAgrupado)
                {
                    var tiposActividad = actividad.Select(a => a.TipoActividad).FirstOrDefault();

                    tiposActividadMostrar.Add(new TipoActividadDTO
                    {
                        NombreTipo = tiposActividad.Nombre,
                        CaloriasPorMinuto = tiposActividad.CaloriasPorMinuto,
                        Actividades = actividad
                        .Select(a => new ActividadDTO
                        {
                            Fecha = a.Fecha,
                            DuracionMinutos = (int)a.DuracionMinutos.TotalMinutes, // Convierte TimeSpan a minutos
                            CaloriasQuemadas = (int)a.DuracionMinutos.TotalMinutes * a.TipoActividad.CaloriasPorMinuto
                        })
                    .ToList()
                    });
                }
                personasMostrar.Add(new PersonasDTO
                {
                    Nombre = persona.Nombre,
                    Email = email,
                    TiposActividad = tiposActividadMostrar
                });
            }
            return Ok(personasMostrar);
        }






        [HttpPost("EstadisticasActividades")]
        public async Task<ActionResult<List<AgruparXPersona>>> EstadisticasActividades([FromBody] FiltroActividad filtro)
        {
            var hoy = DateTime.Today;

            var personasConActividades = await _context.Personas
                .Include(p => p.Actividades)
                .ThenInclude(a => a.TipoActividad)
                .Where(p => p.Actividades.Any(a =>
                    a.Fecha.Date <= hoy &&
                    (!filtro.FechaDesdeFiltro.HasValue || a.Fecha.Date >= filtro.FechaDesdeFiltro.Value.Date) &&
                    (!filtro.FechaHastaFiltro.HasValue || a.Fecha.Date <= filtro.FechaHastaFiltro.Value.Date)
                ))
                .ToListAsync();

            var resultados = personasConActividades.Select(grupoPersona =>
            {
                var actividades = grupoPersona.Actividades?
                    .Where(a => a.Fecha.Date <= hoy &&
                                (!filtro.FechaDesdeFiltro.HasValue || a.Fecha.Date >= filtro.FechaDesdeFiltro.Value.Date) &&
                                (!filtro.FechaHastaFiltro.HasValue || a.Fecha.Date <= filtro.FechaHastaFiltro.Value.Date))
                    .ToList() ?? new List<Actividad>();

            var totalMinutos = actividades.Sum(a => (int)a.DuracionMinutos.TotalMinutes);
            var totalCalorias = actividades.Sum(a => a.TipoActividad.CaloriasPorMinuto * (int)a.DuracionMinutos.TotalMinutes);
            var pesoFinal = grupoPersona.Peso - (totalCalorias / 7700m); 

            var tipoActividad = actividades.GroupBy(a => a.TipoActividad.Nombre)
            .Select(grupoTipo => new AgruparXTipo
            {
                NombreActividad = grupoTipo.Key,
                TotalActividades = grupoTipo.Count(),
                TiempoTotalMinutos = grupoTipo.Sum(a => (int)a.DuracionMinutos.TotalMinutes),
                TotalCalorias = grupoTipo.Sum(a => a.TipoActividad.CaloriasPorMinuto * (int)a.DuracionMinutos.TotalMinutes),
                PesoFinal = grupoPersona.Peso - (grupoTipo.Sum( a => (int)a.DuracionMinutos.TotalMinutes * (int)a.TipoActividad.CaloriasPorMinuto)/7700m),
                VariacionPeso = (grupoPersona.Peso - (grupoTipo.Sum( a => (int)a.DuracionMinutos.TotalMinutes * (int)a.TipoActividad.CaloriasPorMinuto)/7700m)),
            }).ToList();
            
            return new AgruparXPersona
            {
                Nombre = grupoPersona.Nombre,
                PesoInicial = grupoPersona.Peso,
                PesoFinal = pesoFinal,
                VariacionPeso = pesoFinal - grupoPersona.Peso,
                TotalActividades = actividades.Count(),
                TiempoTotalMinutos = totalMinutos,
                PromedioDuracion = actividades.Count > 0? totalMinutos / actividades.Count: 0,
                TotalCalorias = totalCalorias,
                TiposAgrupados = tipoActividad
            };
            }).ToList();

            return Ok(resultados);
        }

        
        private bool ActividadExists(int id)
        {
            return _context.Actividades.Any(e => e.ActividadID == id);
        }
    }
}

