using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Final2025.Models.General;
using System.Security.Claims;
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

        public PersonasController(Context context, UserManager<ApplicationUser> userManager)
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
        public async Task<ActionResult<Persona>> PostPersona([FromBody] CrearUsuario persona)
        {
            var emailExiste = await _context.Users.Where(u => u.Email == persona.Email).AnyAsync();
            if (emailExiste)
            {
                // return BadRequest("El Email ya esta registrado"); 
                return BadRequest(new { codigo = 0, mensaje = $"El Email {persona.Email} ya existe." });
            }

            var fechaDeHoy = DateOnly.FromDateTime(DateTime.Now);
            if (persona.Persona.FechaNacimiento > fechaDeHoy)
            {
                return BadRequest("La fecha de naciemiento no puede ser futura");
            }

            var user = new ApplicationUser
            {
                UserName = persona.Email,
                Email = persona.Email,
                NombreCompleto = persona.Persona.Nombre
            };

            await _userManager.CreateAsync(user, "Final2025");

            persona.Persona.UsuarioID = user.Id;
            _context.Personas.Add(persona.Persona);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetPersona", new { id = persona.Persona.PersonaID }, persona);
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

        private bool PersonaExists(int id)
        {
            return _context.Personas.Any(e => e.PersonaID == id);
        }




        // [HttpPost("Filtrar")]
        // //public async Task<ActionResult<IEnumerable<TipoActividad>>> GetTipoActividad([FromBody] FiltroTipoActividad filtro)
        // public async Task<ActionResult<IEnumerable<Persona>>> Filtrar([FromBody] FiltroPersona filtro)
        // {
        //     var personas = _context.Personas.AsQueryable();

        //     if (!string.IsNullOrEmpty(filtro.Nombre))
        //     {
        //         personas = personas.Where(c => c.Nombre.ToLower().Contains(filtro.Nombre.ToLower()));
        //     }
        //     if (!string.IsNullOrEmpty(filtro.FechaNacimiento))
        //     {
        //         if (DateTime.TryParse(filtro.FechaNacimiento, out DateTime fechaFiltro))
        //         {
        //             DateOnly fechaDateOnly = DateOnly.FromDateTime(fechaFiltro);
        //             personas = personas.Where(p => p.FechaNacimiento == fechaDateOnly);
        //         }
        //     }
        //     if (filtro.Peso.HasValue)
        //     {
        //         personas = personas.Where(c => c.Peso == filtro.Peso.Value);
        //     }

        //     if (filtro.Edad.HasValue)
        //     {
        //         // Calculamos rango de fechas según la edad
        //         var hoy = DateOnly.FromDateTime(DateTime.Today);
        //         var fechaMax = hoy.AddYears(-filtro.Edad.Value);
        //         var fechaMin = fechaMax.AddYears(-1).AddDays(1);
        //         personas = personas.Where(p => p.FechaNacimiento >= fechaMin && p.FechaNacimiento <= fechaMax);
        //     }

        //     // Filtrar por rango de fechas de actividades
        //     DateTime fechaDesde = new DateTime();
        //     bool fechaDesdeValido = !string.IsNullOrEmpty(filtro.FechaNacimientoDesde)
        //                             && DateTime.TryParse(filtro.FechaNacimientoDesde, out fechaDesde);

        //     DateTime fechaHasta = new DateTime();
        //     bool fechaHastaValido = !string.IsNullOrEmpty(filtro.FechaNacimientoHasta)
        //                             && DateTime.TryParse(filtro.FechaNacimientoHasta, out fechaHasta);

        //     DateOnly fechaDesdeDateOnly = fechaDesdeValido ? DateOnly.FromDateTime(fechaDesde) : default;
        //     DateOnly fechaHastaDateOnly = fechaHastaValido ? DateOnly.FromDateTime(fechaHasta) : default;

        //     // Si ambas fechas son válidas, filtrar actividades dentro del rango
        //     if (fechaDesdeValido && fechaHastaValido)
        //         personas = personas.Where(a => a.FechaNacimiento >= fechaDesdeDateOnly
        //                                  && a.FechaNacimiento <= fechaHastaDateOnly);
        //     else if (fechaDesdeValido) // Solo desde
        //         personas = personas.Where(a => a.FechaNacimiento >= fechaDesdeDateOnly);
        //     else if (fechaHastaValido) // Solo hasta
        //         personas = personas.Where(a => a.FechaNacimiento <= fechaHastaDateOnly);

        //     return personas.ToList();
        // }




    }
}
