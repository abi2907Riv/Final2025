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
        private bool ActividadExists(int id)
        {
            return _context.Actividades.Any(e => e.ActividadID == id);
        }
    }
}
