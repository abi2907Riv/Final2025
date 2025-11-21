using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Final2025.Models.General;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace Final2025.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PersonasController : ControllerBase
    {
        private readonly Context _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public PersonasController(
            Context context,
            UserManager<ApplicationUser> userManager
        )
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: api/Personas
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Persona>>> GetPersonas()
        {
            return await _context.Personas.ToListAsync();
        }

        // GET: api/Personas/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Persona>> GetPersona(int id)
        {
            var persona = await _context.Personas.FindAsync(id);

            if (persona == null)
            {
                return NotFound();
            }

            return persona;
        }

        // PUT: api/Personas/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPersona(int id, Persona persona)
        {
            if (id != persona.PersonaID)
            {
                return BadRequest();
            }

            //Validar que no exista una categoria con el mismo nombre - sin importar mayúsculas/minúsculas
            var emailExistente = await _context.Personas
            .Where(p => persona.Email.ToLower().ToUpper() == p.Email.ToLower().ToUpper() && p.PersonaID != id)
            .AnyAsync();

            //Hace una condicion de que si la categoria ya existe, se devuelva un error
            if (emailExistente)
            {
                return BadRequest(new { codigo = 0, mensaje = $"El Email {persona.Email} ya existe."});
            }


            //SOLO PERMITIR EDITAR
            persona.Nombre = persona.Nombre;
            persona.FechaNacimiento = persona.FechaNacimiento;
            persona.Peso = persona.Peso;


            _context.Entry(persona).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PersonaExists(id))
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

        // POST: api/Personas
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Persona>> PostPersona(Persona persona)
        {
            // Validar email duplicado
            var emailExistente = await _context.Personas
                .FirstOrDefaultAsync(c => c.Email.ToLower() == persona.Email.ToLower());

            if (emailExistente != null)
            {
                return BadRequest(new { codigo = 0, mensaje = $"El Email {persona.Email} ya existe." });
            }

            try
            {
                _context.Personas.Add(persona);
                await _context.SaveChangesAsync();

                // Crear usuario de Identity
                var user = new ApplicationUser
                {
                    NombreCompleto = persona.Nombre,
                    UserName = persona.Email,
                    Email = persona.Email
                };

                var result = await _userManager.CreateAsync(user, "Final2025");


                return CreatedAtAction("GetPersona", new { id = persona.PersonaID }, persona);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = ex.Message });
            }
        }

        // DELETE: api/Personas/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePersona(int id)
        {
            var persona = await _context.Personas.FindAsync(id);
            if (persona == null)
            {
                return NotFound();
            }

            _context.Personas.Remove(persona);
            await _context.SaveChangesAsync();

            return NoContent();
        }


    //     [HttpPost("Filtrar")]
    //     public async Task<ActionResult<IEnumerable<Persona>>> GetPersona([FromBody] FiltroPersona filtro)
    //     {
    //         var personas = _context.Personas.AsQueryable();

    //         if (!string.IsNullOrEmpty(filtro.Nombre))
    //         {
    //             personas = personas.Where(c => c.Nombre.ToLower().Contains(filtro.Nombre.ToLower()));
    //         }
    //         if (filtro.FechaNacimiento.HasValue)
    //         {
    //             var fechaInicio = filtro.FechaNacimiento.Value.Date;
    //             var fechaFin = fechaInicio.AddDays(1);
    //             personas = personas.Where(a => a.FechaNacimiento >= fechaInicio && a.FechaNacimiento < fechaFin);
    //         }

    //         if (filtro.Peso.HasValue)
    //         {
    //             personas = personas.Where(c => c.Peso == filtro.Peso.Value);
    //         }
            
    //         return personas.ToList();
    //    }

        private bool PersonaExists(int id)
        {
            return _context.Personas.Any(e => e.PersonaID == id);
        }
    }
}
