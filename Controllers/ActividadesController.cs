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

            if (fecha > fechaDeHoy)
            {
                return BadRequest("La fecha no puede ser futura a la fecha de hoy");
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

















        ///////////////////////////////////////////////INFORMES////////////////////////////////////////////////
        /// Por cada persona, muestra todas sus actividades, ordenadas por fecha, mostrando la duración y 
        /// calculando las calorías (duración × calorías por minuto)
        // [HttpPost("InformeActividadXPersona")]
        // public async Task<ActionResult<IEnumerable<PersonasDTO>>> InformeActividadXPersona()
        // {
        //     //Obtengo el ID del usuario logueado
        //     var userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
        //     //Trae todas las personas
        //     var personas = await _context.Personas
        //         //.Where(p => p.UsuarioID == userId)
        //         .ToListAsync();

        //     // Lista final que tendra los resultados finales
        //     List<PersonasDTO> personasMostrar = new List<PersonasDTO>();

        //     foreach (var persona in personas)
        //     {
        //         var email = await _context.Users
        //             .Where(u => u.Id == persona.UsuarioID)
        //             .Select(u => u.Email)
        //             .FirstOrDefaultAsync();

        //         //Busco las actividades que le pertenecenn a esa persona
        //         var actividades = await _context.Actividades
        //         .Where(a => a.PersonaID == persona.PersonaID)
        //         .OrderByDescending(a => a.Fecha)
        //             .Include(a => a.TipoActividad).ToListAsync();

        //         if (!actividades.Any())
        //             continue;

        //         //Agrupo actividades por TipoActividad
        //         var tipoActividadAgrupado = actividades
        //         .GroupBy(a => a.TipoActividadID);

        //         List<TipoActividadDTO> tiposActividadMostrar = new List<TipoActividadDTO>();

        //         foreach (var actividad in tipoActividadAgrupado)
        //         {
        //             var tiposActividad = actividad.Select(a => a.TipoActividad).First();
        //             tiposActividadMostrar.Add(new TipoActividadDTO
        //             {
        //                 NombreTipo = tiposActividad.Nombre,
        //                 CaloriasPorMinuto = tiposActividad.CaloriasPorMinuto,
        //                 Actividades = actividad
        //                 .Select(a => new ActividadDTO
        //                 {
        //                     Fecha = a.Fecha,
        //                     DuracionMinutos = (int)a.DuracionMinutos.TotalMinutes,
        //                     CaloriasQuemadas = (int)a.DuracionMinutos.TotalMinutes * a.TipoActividad.CaloriasPorMinuto
        //                 })
        //             .ToList()
        //             });
        //         }
        //         personasMostrar.Add(new PersonasDTO
        //         {
        //             Nombre = persona.Nombre,
        //             Email = email,
        //             TiposActividad = tiposActividadMostrar
        //         });
        //     }

        //     return Ok(personasMostrar);
        // }



        // [HttpPost("FiltrarPersona")]
        // public async Task<ActionResult<IEnumerable<Actividad>>> FiltroPersonaActividad([FromBody] FiltroPersonaActividad filtro)
        // {
        //     //List<Actividad> vista = new List<Actividad>();

        //     var persona = await _context.Personas.Where(p => p.PersonaID == filtro.PersonaID)
        //     .FirstOrDefaultAsync();

        //     // Obtener todas las actividades incluidos los tipos de actividad
        //     var actividades = await _context.Actividades
        //     .Where(a => a.PersonaID == filtro.PersonaID)
        //     .Include(a => a.TipoActividad).ToListAsync();

        //     if (filtro.PersonaID > 0) // solo filtra si se seleccionó una persona específica
        //         actividades = actividades.Where(a => a.PersonaID == filtro.PersonaID).ToList();
        //     return Ok(actividades);
        // }


        ///Muestra cada tipo de actividad y dentro de cada uno lista todas las actividades que tienen ese tipo, 
        /// mostrando duración y calorías quemadas (duración × calorías por minuto)
        // [HttpPost("InformeActividadXTipoActividad")]
        // public async Task<ActionResult<IEnumerable<TipoActividadDTO>>> InformeActividadXTipoActividad()
        // {
        //     var tipoActividad = await _context.TipoActividades.ToListAsync();
        //     var actividades = await _context.Actividades.Include(a => a.TipoActividad).ToListAsync();

        //     List<TipoActividadDTO> tiposActividadMostrar = new List<TipoActividadDTO>();


        //     foreach(var tiposActividades in tipoActividad)
        //     {
        //          var actividadesDelTipo = actividades
        //             .Where(a => a.TipoActividadID == tiposActividades.TipoActividadID)
        //         .Select(t => new ActividadDTO
        //         {
        //             Fecha = t.Fecha,
        //             DuracionMinutos = (int)t.DuracionMinutos.TotalMinutes,
        //             CaloriasQuemadas = (int)t.DuracionMinutos.TotalMinutes * t.TipoActividad.CaloriasPorMinuto
        //         })
        //         .ToList();

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
        /// hizo de cada tipo, el total de minutos acumulados y las calorías totales quemadas
        // [HttpPost("InformeActividadXPersonaYTipoAcividad")]
        // public async Task<ActionResult<IEnumerable<PersonasDTO>>> InformeActividadXPersonaYTipoAcividad()
        // {
        //     //Obtengo el ID del usuario logueado
        //     var userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
        //     //Trae todas las personas
        //     var personas = await _context.Personas
        //         //.Where(p => p.UsuarioID == userId)
        //         .ToListAsync();

        //     // Lista final que tendra los resultados finales
        //     List<PersonasDTO> personasMostrar = new List<PersonasDTO>();

        //     foreach (var persona in personas)
        //     {
        //         var email = await _context.Users
        //             .Where(u => u.Id == persona.UsuarioID)
        //             .Select(u => u.Email)
        //             .FirstOrDefaultAsync();

        //         //Busco las actividades que le pertenecenn a esa persona
        //         var actividades = await _context.Actividades
        //         .Where(a => a.PersonaID == persona.PersonaID)
        //         .OrderByDescending(a => a.Fecha)
        //             .Include(a => a.TipoActividad).ToListAsync();

        //         if (!actividades.Any())
        //             continue;

        //         //Agrupo actividades por TipoActividad
        //         var tipoActividadAgrupado = actividades
        //         .GroupBy(a => a.TipoActividadID);

        //         List<TipoActividadDTO> tiposActividadMostrar = new List<TipoActividadDTO>();

        //         foreach (var tipoActividades in tipoActividadAgrupado)
        //         {
        //             var tiposActividad = tipoActividades.Select(a => a.TipoActividad).First();

        //             var totalMinutos = tipoActividades.Sum(a => (int)a.DuracionMinutos.TotalMinutes);
        //             var totalCalorias = (int)Math.Round(tipoActividades.Sum(a =>
        //                 (int)a.DuracionMinutos.TotalMinutes * a.TipoActividad.CaloriasPorMinuto));
        //             var cantidadActividades = tipoActividades.Count();

        //             tiposActividadMostrar.Add(new TipoActividadDTO
        //             {
        //                 NombreTipo = tiposActividad.Nombre,
        //                 TotalMinutos = totalMinutos,
        //                 TotalCalorias = totalCalorias,
        //                 CantidadActividades = cantidadActividades
        //             });
        //         }
        //         personasMostrar.Add(new PersonasDTO
        //         {
        //             Nombre = persona.Nombre,
        //             Email = email,
        //             TiposActividad = tiposActividadMostrar
        //         });
        //     }

        //     return Ok(personasMostrar);
        // }


        //Nombre de la persona, total de minutos acumulados en todas las actividades, 
        // cantidad de actividades realizadas y el promedio de minutos por actividad
        // [HttpPost("DuracionTotalActividadesPersona")]
        // public async Task<ActionResult<IEnumerable<PersonasDTO>>> DuracionTotalActividadesPersona()
        // {
        //     //Obtengo el ID del usuario logueado
        //     var userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
        //     //Trae todas las personas
        //     var personas = await _context.Personas
        //         //.Where(p => p.UsuarioID == userId)
        //         .ToListAsync();
        //     List<PersonasDTO> personasMostrar = new List<PersonasDTO>();

        //     foreach (var persona in personas)
        //     {
        //         var email = await _context.Users
        //             .Where(u => u.Id == persona.UsuarioID)
        //             .Select(u => u.Email)
        //             .FirstOrDefaultAsync();

        //         //Busco las actividades que le pertenecenn a esa persona
        //         var actividades = await _context.Actividades
        //         .Where(a => a.PersonaID == persona.PersonaID && a.TipoActividad.Eliminado == false)
        //         .OrderByDescending(a => a.Fecha)
        //             .Include(a => a.TipoActividad).ToListAsync();

        //         var totalMinutos = actividades.Sum(a => (int)a.DuracionMinutos.TotalMinutes);
        //         var cantidadActividades = actividades.Count();
        //         var promedio = Math.Round(cantidadActividades > 0 ? totalMinutos / (double)cantidadActividades : 0);               


        //         personasMostrar.Add(new PersonasDTO
        //     {
        //         Nombre = persona.Nombre,
        //         Email = email,
        //         TotalMinutos = totalMinutos,
        //         CantidadActividades = cantidadActividades,
        //         PromedioMinutos = promedio
        //     });
        //     }
        //     return Ok(personasMostrar);
        // }









        // [HttpPost("InformeActividadXPersonaCantidad")]
        // public async Task<ActionResult<IEnumerable<PersonasDTO>>> InformeActividadXPersona([FromBody] FiltroActividad filtro)
        // {
        //     //Obtengo el ID del usuario logueado
        //     var userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
        //     //Trae todas las personas
        //     var personas = await _context.Personas
        //         //.Where(p => p.UsuarioID == userId)
        //         .ToListAsync();
        //     // Lista final que tendra los resultados finales
        //     List<PersonasDTO> personasMostrar = new List<PersonasDTO>();

        //     foreach (var persona in personas)
        //     {
        //         var email = await _context.Users
        //             .Where(u => u.Id == persona.UsuarioID)
        //             .Select(u => u.Email)
        //             .FirstOrDefaultAsync();

        //         //Busco las actividades que le pertenecenn a esa persona
        //         var actividades =_context.Actividades
        //         .Where(a => a.PersonaID == persona.PersonaID)
        //         .OrderByDescending(a => a.Fecha)
        //             .Include(a => a.TipoActividad).AsQueryable();

        //         //FILTROS
        //         if (filtro.PersonaID > 0) 
        //          actividades = actividades.Where(a => a.PersonaID == filtro.PersonaID);

        //         if (filtro.TipoActividadID > 0) 
        //          actividades = actividades.Where(a => a.TipoActividadID == filtro.TipoActividadID);

        //         // Filtrar por rango de fechas de actividades
        //         DateTime fechaDesde = new DateTime();
        //         bool fechaDesdeValido = !string.IsNullOrEmpty(filtro.FechaDesde) && DateTime.TryParse(filtro.FechaDesde, out fechaDesde);
            

        //         DateTime fechaHasta = new DateTime();
        //         bool fechaHastaValido = !string.IsNullOrEmpty(filtro.FechaHasta) && DateTime.TryParse(filtro.FechaHasta, out fechaHasta);

        //         // Si ambas fechas son válidas, filtrar actividades dentro del rango
        //         if (fechaDesdeValido && fechaHastaValido)
        //             actividades = actividades.Where(a => a.Fecha >= fechaDesde && a.Fecha <= fechaHasta);
        //         else if (fechaDesdeValido) // Solo desde
        //             actividades = actividades.Where(a => a.Fecha >= fechaDesde);
        //         else if (fechaHastaValido) // Solo hasta
        //             actividades = actividades.Where(a => a.Fecha <= fechaHasta);

        //         var actividadesFiltradas = await actividades.ToListAsync();

        //         if (!actividadesFiltradas.Any())
        //             continue;

        //         //Agrupo actividades por TipoActividad
        //         var tipoActividadAgrupado = actividadesFiltradas
        //         .GroupBy(a => a.TipoActividadID);

        //         List<TipoActividadDTO> tiposActividadMostrar = new List<TipoActividadDTO>();

        //         foreach (var actividad in tipoActividadAgrupado)
        //         {
        //             var tiposActividad = actividad.Select(a => a.TipoActividad).First();
        //             tiposActividadMostrar.Add(new TipoActividadDTO
        //             {
        //                 NombreTipo = tiposActividad.Nombre,
        //                 CaloriasPorMinuto = tiposActividad.CaloriasPorMinuto,
        //                 Actividades = actividad
        //                 .Select(a => new ActividadDTO
        //                 {
        //                     Fecha = a.Fecha,
        //                     DuracionMinutos = (int)a.DuracionMinutos.TotalMinutes,
        //                     CaloriasQuemadas = (int)a.DuracionMinutos.TotalMinutes * a.TipoActividad.CaloriasPorMinuto
        //                 })
        //             .ToList()
        //             });
        //         }
        //         personasMostrar.Add(new PersonasDTO
        //         {
        //             Nombre = persona.Nombre,
        //             Email = email,
        //             TiposActividad = tiposActividadMostrar,
        //             TotalMinutos = actividadesFiltradas.Sum(a => (int)a.DuracionMinutos.TotalMinutes),
        //             TotalCalorias = (int)actividadesFiltradas.Sum(a => (int)a.DuracionMinutos.TotalMinutes * a.TipoActividad.CaloriasPorMinuto)
        //         });
        //     }

        //     return Ok(personasMostrar);
        // }



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
        private bool ActividadExists(int id)
        {
            return _context.Actividades.Any(e => e.ActividadID == id);
        }
    }
}
