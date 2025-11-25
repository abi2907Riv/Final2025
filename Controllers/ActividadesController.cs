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
        [HttpPost("InformeActividadXPersona")]
        public async Task<ActionResult<IEnumerable<PersonasDTO>>> InformeActividadXPersona()
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
                var actividades = await _context.Actividades
                .Where(a => a.PersonaID == persona.PersonaID)
                .OrderByDescending(a => a.Fecha)
                    .Include(a => a.TipoActividad).ToListAsync();

                if (!actividades.Any())
                    continue;

                //Agrupo actividades por TipoActividad
                var tipoActividadAgrupado = actividades
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
                    TiposActividad = tiposActividadMostrar
                });
            }

            return Ok(personasMostrar);
        }



        [HttpPost("FiltrarPersona")]
        public async Task<ActionResult<IEnumerable<Actividad>>> FiltroPersonaActividad([FromBody] FiltroPersonaActividad filtro)
        {
            //List<Actividad> vista = new List<Actividad>();

            var persona = await _context.Personas.Where(p => p.PersonaID == filtro.PersonaID)
            .FirstOrDefaultAsync();

            // Obtener todas las actividades incluidos los tipos de actividad
            var actividades = await _context.Actividades
            .Where(a => a.PersonaID == filtro.PersonaID)
            .Include(a => a.TipoActividad).ToListAsync();

            if (filtro.PersonaID > 0) // solo filtra si se seleccionó una persona específica
                actividades = actividades.Where(a => a.PersonaID == filtro.PersonaID).ToList();


            return Ok(actividades);

        }

        [HttpPost("InformeActividadXTipoActividad")]
        public async Task<ActionResult<IEnumerable<TipoActividadDTO>>> InformeActividadXTipoActividad()
        {
            var tipoActividad = await _context.TipoActividades.ToListAsync();
            var actividades = await _context.Actividades.Include(a => a.TipoActividad).ToListAsync();

            List<TipoActividadDTO> tiposActividadMostrar = new List<TipoActividadDTO>();
            
            
            foreach(var tiposActividades in tipoActividad)
            {
                 var actividadesDelTipo = actividades
                    .Where(a => a.TipoActividadID == tiposActividades.TipoActividadID)
                .Select(t => new ActividadDTO
                {
                    Fecha = t.Fecha,
                    DuracionMinutos = (int)t.DuracionMinutos.TotalMinutes,
                    CaloriasQuemadas = (int)t.DuracionMinutos.TotalMinutes * t.TipoActividad.CaloriasPorMinuto
                })
                .ToList();

                if (!actividadesDelTipo.Any())
                    continue;
                tiposActividadMostrar.Add(new TipoActividadDTO
                {
                    NombreTipo = tiposActividades.Nombre,
                    CaloriasPorMinuto = tiposActividades.CaloriasPorMinuto,
                    Actividades = actividadesDelTipo
                });
            }
            return Ok(tiposActividadMostrar);

        }


        private bool ActividadExists(int id)
        {
            return _context.Actividades.Any(e => e.ActividadID == id);
        }
    }
}
