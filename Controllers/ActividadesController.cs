using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Final2025.Models.General;
using System.Security.Claims;

namespace Final2025.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ActividadesController : ControllerBase
    {
        private readonly Context _context;

        public ActividadesController(Context context)
        {
            _context = context;
        }

        // GET: api/Actividades
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Actividad>>> GetActividades()
        {
            var userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var personaID = await _context.Personas.Where(p => p.UsuarioID == userId)
            .Select(p => p.PersonaID).FirstOrDefaultAsync();

            var actividades = await _context.Actividades.Where(a => a.PersonaID == personaID)
            .OrderByDescending(a => a.Fecha)
            .Include(a => a.TipoActividad)
            .ToListAsync();
            return actividades;
        }

        // GET: api/Actividades/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Actividad>> GetActividad(int id)
        {
            var actividad = await _context.Actividades.FindAsync(id);

            if (actividad == null)
            {
                return NotFound();
            }

            return actividad;
        }

        // PUT: api/Actividades/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutActividad(int id, Actividad actividad)
        {
            var fecha = DateOnly.FromDateTime(actividad.Fecha);
            var fechaDeHoy = DateOnly.FromDateTime(DateTime.Now);

            if (fecha > fechaDeHoy)
            {
                return BadRequest("La fecha no puede ser futura a la fecha de hoy");
            }

            var userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var personaID = await _context.Personas.Where(p => p.UsuarioID == userId)
            .Select(p => p.PersonaID).FirstOrDefaultAsync();

            actividad.PersonaID = personaID;

            if (id != actividad.ActividadID)
            {
                return BadRequest();
            }

            _context.Entry(actividad).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ActividadExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Actividades
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Actividad>> PostActividad(Actividad actividad)
        {

            var fecha = DateOnly.FromDateTime(actividad.Fecha);
            var fechaDeHoy = DateOnly.FromDateTime(DateTime.Now);
            var limiteFecha = fechaDeHoy.AddMonths(-1);

            if (fecha > fechaDeHoy)
            {
                return BadRequest("La fecha no puede ser futura a la fecha de hoy");
            }

            if (fecha < limiteFecha)
            {
                return BadRequest("La fecha no puede ser anterior a un mes atrás");
            }

            var userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var personaID = await _context.Personas.Where(p => p.UsuarioID == userId)
            .Select(p => p.PersonaID).FirstOrDefaultAsync();

            actividad.PersonaID = personaID;
            _context.Actividades.Add(actividad);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetActividad", new { id = actividad.ActividadID }, actividad);
        }

        // DELETE: api/Actividades/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteActividad(int id)
        {
            var actividad = await _context.Actividades.FindAsync(id);
            if (actividad == null)
            {
                return NotFound();
            }

            _context.Actividades.Remove(actividad);
            await _context.SaveChangesAsync();

            return NoContent();
        }


        //Metodo que permite filtrar por Tipo Actividad, por rango de fechas, por duracion de minutos
        // [HttpPost("Filtrar")]
        // //public async Task<ActionResult<IEnumerable<TipoActividad>>> GetTipoActividad([FromBody] FiltroTipoActividad filtro)
        // public async Task<ActionResult<IEnumerable<Actividad>>> Filtrar([FromBody] FiltroActividad filtro)
        // {
        //     var actividadFiltrada = _context.Actividades
        //     .Include(a => a.TipoActividad)
        //     .AsQueryable();

        //     if (filtro.TipoActividadID > 0) 
        //         actividadFiltrada = actividadFiltrada.Where(a => a.TipoActividadID == filtro.TipoActividadID);

        //     // Filtrar por rango de fechas de actividades
        //     DateTime fechaDesde = new DateTime();
        //     bool fechaDesdeValido = !string.IsNullOrEmpty(filtro.FechaDesde) && DateTime.TryParse(filtro.FechaDesde, out fechaDesde);

        //     DateTime fechaHasta = new DateTime();
        //     bool fechaHastaValido = !string.IsNullOrEmpty(filtro.FechaHasta) && DateTime.TryParse(filtro.FechaHasta, out fechaHasta);

        //     // Si ambas fechas son válidas, filtrar actividades dentro del rango
        //     if (fechaDesdeValido && fechaHastaValido)
        //         actividadFiltrada = actividadFiltrada.Where(a => a.Fecha >= fechaDesde && a.Fecha <= fechaHasta);
        //     else if (fechaDesdeValido) // Solo desde
        //         actividadFiltrada = actividadFiltrada.Where(a => a.Fecha >= fechaDesde);
        //     else if (fechaHastaValido) // Solo hasta
        //         actividadFiltrada = actividadFiltrada.Where(a => a.Fecha <= fechaHasta);

        //     if (filtro.DuracionMinutos.HasValue)
        //     {
        //         actividadFiltrada = actividadFiltrada.Where(c => c.DuracionMinutos == filtro.DuracionMinutos.Value);
        //     }

        //     return actividadFiltrada.ToList();
        // } 




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

    









        /// Muestra cada tipo de actividad y dentro de cada uno lista todas las actividades 
        /// que pertenecen a ese tipo, indicando su duración y las calorías quemadas 
        /// (duración × calorías por minuto)
        // [HttpPost("InformeActividadXTipoActividad")]
        // public async Task<ActionResult<IEnumerable<TipoActividadDTO>>> InformeActividadXTipoActividad()
        // {
        //     var tipoActividad = await _context.TipoActividades.ToListAsync();
        //     var actividades = await _context.Actividades.Include(a => a.TipoActividad).ToListAsync();
        //     List<TipoActividadDTO> tiposActividadMostrar = new List<TipoActividadDTO>();
        //     foreach (var tiposActividades in tipoActividad)
        //     {
        //         var actividadesDelTipo = actividades
        //         .Where(a => a.TipoActividadID == tiposActividades.TipoActividadID)
        //        .Select(t => new ActividadDTO
        //        {
        //            Fecha = t.Fecha,
        //            DuracionMinutos = (int)t.DuracionMinutos.TotalMinutes,
        //            CaloriasQuemadas = (int)t.DuracionMinutos.TotalMinutes * t.TipoActividad.CaloriasPorMinuto
        //        })
        //        .ToList();
        //         if (!actividadesDelTipo.Any())
        //             continue;
        //         tiposActividadMostrar.Add(new TipoActividadDTO
        //         {
        //             NombreTipo = tiposActividades.Nombre,
        //             CaloriasPorMinuto = tiposActividades.CaloriasPorMinuto,
        //             Actividades = actividadesDelTipo
        //         });
        //     }
        //     return Ok(tiposActividadMostrar);
        // }










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



        private bool ActividadExists(int id)
        {
            return _context.Actividades.Any(e => e.ActividadID == id);
        }
    }
}
