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
            //Validar que no exista una categoria con el mismo nombre - sin importar mayúsculas/minúsculas
            var nombreExistente = await _context.TipoActividades
            .Where(c => tipoActividad.Nombre.ToLower().ToUpper() == c.Nombre.ToLower().ToUpper() && c.TipoActividadID != id)
            .AnyAsync();

            //Hace una condicion de que si la categoria ya existe, se devuelva un error
            if (nombreExistente)
            {
                return BadRequest(new { codigo = 0, mensaje = $"El Tipo Actividad {tipoActividad.Nombre} ya existe ." });
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
            //Validar que no exista una categoria con el mismo nombre
            var nombreExistente = await _context.TipoActividades
            .FirstOrDefaultAsync(c => tipoActividad.Nombre.ToLower().ToUpper() == c.Nombre.ToLower().ToUpper());

            if (nombreExistente != null)
            {
                return BadRequest(new { codigo = 0, mensaje = $"El Tipo de Actividad {tipoActividad.Nombre} ya existe." });
            }

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
            tipoActividad.Eliminado = !tipoActividad.Eliminado;
            _context.TipoActividades.Update(tipoActividad);
            await _context.SaveChangesAsync();

            return NoContent();
        }



        // [HttpPost("Filtrar")]
        // public async Task<ActionResult<IEnumerable<TipoActividad>>> GetTipoActividad([FromBody] FiltroTipoActividad filtro)
        // {
        //     var tipoActividadFiltrada = _context.TipoActividades.AsQueryable();
        //     if (filtro.Eliminado.HasValue)
        //     {
        //         tipoActividadFiltrada = tipoActividadFiltrada.Where(c => c.Eliminado == (filtro.Eliminado.Value == 1));
        //     }
        //     if (!string.IsNullOrEmpty(filtro.Nombre))
        //     {
        //         tipoActividadFiltrada = tipoActividadFiltrada.Where(c => c.Nombre.ToLower().Contains(filtro.Nombre.ToLower()));
        //     }
        //     if (filtro.CaloriasPorMinuto.HasValue)
        //     {
        //         tipoActividadFiltrada = tipoActividadFiltrada.Where(c => c.CaloriasPorMinuto == filtro.CaloriasPorMinuto.Value);
        //     }
            
        //     return tipoActividadFiltrada.ToList();
        // }        

        private bool TipoActividadExists(int id)
        {
            return _context.TipoActividades.Any(e => e.TipoActividadID == id);
        }
    }
}
