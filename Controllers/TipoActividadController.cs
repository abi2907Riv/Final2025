using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Final2025.Models.General;

namespace Final2025.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TipoActividadController : ControllerBase
    {
        private readonly Context _context;

        public TipoActividadController(Context context)
        {
            _context = context;
        }

        // GET: api/TipoActividad
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TipoActividad>>> GetTipoActividades()
        {
            return await _context.TipoActividades.ToListAsync();
        }

        // GET: api/TipoActividad/5
        [HttpGet("{id}")]
        public async Task<ActionResult<TipoActividad>> GetTipoActividad(int id)
        {
            var tipoActividad = await _context.TipoActividades.FindAsync(id);

            if (tipoActividad == null)
            {
                return NotFound();
            }

            return tipoActividad;
        }

        // PUT: api/TipoActividad/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTipoActividad(int id, TipoActividad tipoActividad)
        {
            if (id != tipoActividad.TipoActividadID)
            {
                return BadRequest();
            }

            _context.Entry(tipoActividad).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TipoActividadExists(id))
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

        // POST: api/TipoActividad
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<TipoActividad>> PostTipoActividad(TipoActividad tipoActividad)
        {
            _context.TipoActividades.Add(tipoActividad);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetTipoActividad", new { id = tipoActividad.TipoActividadID }, tipoActividad);
        }

        // DELETE: api/TipoActividad/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTipoActividad(int id)
        {
            var tipoActividad = await _context.TipoActividades.FindAsync(id);
            if (tipoActividad == null)
            {
                return NotFound();
            }

            _context.TipoActividades.Remove(tipoActividad);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool TipoActividadExists(int id)
        {
            return _context.TipoActividades.Any(e => e.TipoActividadID == id);
        }
    }
}
