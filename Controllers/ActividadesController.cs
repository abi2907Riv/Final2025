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
            var emailPersona = HttpContext.User.FindFirst(ClaimTypes.Email)?.Value;
            Console.WriteLine("EMAIL DEL TOKEN = " + emailPersona);
            var persona = _context.Personas.FirstOrDefault(p => p.Email == emailPersona);
            Console.WriteLine("¿PERSONA ENCONTRADA? " + (persona != null));

            return await _context.Actividades
            .Where(a => a.PersonaID == persona.PersonaID)
            .Include(a => a.Persona)
            .Include(a => a.TipoActividad)
            .ToListAsync();
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
            if (id != actividad.ActividadID)
            {
                return BadRequest();
            }

            var actividades = await _context.Actividades.FindAsync(id);

            actividades.TipoActividadID = actividad.TipoActividadID;
            actividades.Fecha = actividad.Fecha;
            actividades.DuracionMinutos = actividad.DuracionMinutos;
            actividades.Observaciones = actividad.Observaciones;
            //_context.Entry(actividad).State = EntityState.Modified;

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
            var emailPersona = HttpContext.User.FindFirst(ClaimTypes.Email)?.Value;

            var persona = _context.Personas.FirstOrDefault(p => p.Email == emailPersona);

            actividad.PersonaID = persona.PersonaID;
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

        // [HttpPost("Filtrar")]
        // public async Task<ActionResult<IEnumerable<VistaActividad>>> FiltroActividad([FromBody] FiltroActividad filtro)
        // {
        //     List<VistaActividad> vista = new List<VistaActividad>();

        //     var actividades = _context.Actividades.Include(t => t.TipoActividad).AsQueryable();

        //     if (filtro.TipoActividadID > 0)
        //         actividades = actividades.Where(t => t.TipoActividadID == filtro.TipoActividadID);

        //     if (filtro.FechaActividad.HasValue)
        //     {
        //         var fechaInicio = filtro.FechaActividad.Value.Date;
        //         var fechaFin = fechaInicio.AddDays(1);
        //         actividades = actividades.Where(a => a.Fecha >= fechaInicio && a.Fecha < fechaFin);
        //     }
        //     if (filtro.DuracionMinutos.HasValue)
        //     {
        //         actividades = actividades.Where(c => c.DuracionMinutos == filtro.DuracionMinutos.Value);
        //     }
            
        //     foreach (var actividad in actividades.OrderByDescending(t => t.Fecha))
        //     {
        //         var actividadMostrar = new VistaActividad
        //         {
        //             ActividadID = actividad.ActividadID,
        //             TipoActividadID = actividad.TipoActividadID,
        //             FechaString = actividad.Fecha.ToString("dd/MM/yyyy"),
        //             DuracionMinutos = actividad.DuracionMinutos,
        //             Nombre = actividad.TipoActividad != null ? actividad.TipoActividad.Nombre : "Sin categoría"

        //         };
        //         vista.Add(actividadMostrar); 
        //     }
        //     return vista.ToList();
        // }

        private bool ActividadExists(int id)
        {
            return _context.Actividades.Any(e => e.ActividadID == id);
        }
    }
}
