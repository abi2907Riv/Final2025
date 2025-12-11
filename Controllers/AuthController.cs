using Final2025.Models.Users;
using Final2025.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Final2025.Models.General;

// Si desarrollamdos una API pura, especialmente para consumir desde frontend o apps móviles:
//Usamos a modo organizativo [Route("api/[controller]")]

// Si desarrollamos algo interno, pequeño o una app híbrida (MVC + API):
//Usamos [Route("[controller]")]

[Route("api/[controller]")]
[ApiController]

public class AuthController : ControllerBase
{
    private readonly Context _context;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly RoleManager<IdentityRole> _rolManager;
    private readonly IConfiguration _configuration;

    public AuthController(
        Context context,
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        IConfiguration configuration,
        RoleManager<IdentityRole> rolManager)
    {
        _context = context;
        _userManager = userManager;
        _signInManager = signInManager;
        _configuration = configuration;
        _rolManager = rolManager;
    }


    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] Register model)
    {
        //CREAMOS EL ROL DE ADMINISTRADOR
        // var rolAdmin = _context.Roles.Where(r => r.Name == "ADMINISTRADOR").SingleOrDefault();
        // if (rolAdmin == null)
        // {
        //     var rolResult = await _rolManager.CreateAsync(new IdentityRole("ADMINISTRADOR"));
        // }
        var emailExiste = await _userManager.FindByEmailAsync(model.Email);
        if (emailExiste != null)
        {
            return BadRequest("El E-mail ya se encuentra registrado");
        }

        // Creamos un nuevo usuario con los datos que escribió
        var user = new ApplicationUser
        {
            UserName = model.Email,
            Email = model.Email,
            NombreCompleto = model.NombreCompleto
        };

        //Hacemos uno del metodo registar usuario
        var result = await _userManager.CreateAsync(user, model.Password);

        if (result.Succeeded)
        {
            var persona = new Persona
            {
                Nombre = model.NombreCompleto,
                FechaNacimiento = model.FechaNacimiento,
                Peso = model.Peso,
                UsuarioID = user.Id
            };

            persona.UsuarioID = user.Id;
            _context.Personas.Add(persona);
            await _context.SaveChangesAsync();
            //await _userManager.AddToRoleAsync(user, "ADMINISTRADOR");
            return Ok("Registro exitoso");
        }
        return BadRequest("La contraseña debe tener un mínimo de 6 caracteres.");
    }


    // [HttpPost("register")]
    // public async Task<IActionResult> Register([FromBody] Register model)
    // {
    //     var emailExiste = await _userManager.FindByEmailAsync(model.Email);
    //     if (emailExiste != null)
    //     {
    //         return BadRequest("El E-mail ya se encuentra registrado");
    //     }
    //     if (!await _rolManager.RoleExistsAsync("ADMINISTRADOR"))
    //         await _rolManager.CreateAsync(new IdentityRole("ADMINISTRADOR"));

    //     if (!await _rolManager.RoleExistsAsync("PERSONA"))
    //         await _rolManager.CreateAsync(new IdentityRole("PERSONA"));

    //     var admins = await _userManager.GetUsersInRoleAsync("ADMINISTRADOR");
    //     bool yaExisteAdmin = admins.Any();

    //     var user = new ApplicationUser
    //     {
    //         UserName = model.Email,
    //         Email = model.Email,
    //         NombreCompleto = model.NombreCompleto
    //     };

    //     var result = await _userManager.CreateAsync(user, model.Password);

    //     if (!result.Succeeded)
    //     {
    //         return BadRequest("La contraseña debe tener un mínimo de 6 caracteres.");
    //     }

    //     var persona = new Persona
    //     {
    //         Nombre = model.NombreCompleto,
    //         FechaNacimiento = model.FechaNacimiento,
    //         Peso = model.Peso,
    //         UsuarioID = user.Id
    //     };

    //     _context.Personas.Add(persona);
    //     await _context.SaveChangesAsync();

    //     if (!yaExisteAdmin)
    //     {
    //         await _userManager.AddToRoleAsync(user, "ADMINISTRADOR");
    //         return Ok("Registro exitoso: Se creó el administrador del sistema");
    //     }
    //     else
    //     {
    //         await _userManager.AddToRoleAsync(user, "PERSONA");
    //         return Ok("Registro exitoso: Usuario registrado como persona");
    //     }
    // }


    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] Login model)
    {
        //Buscamos el usuario por medio del email en la base de datos
        var user = await _userManager.FindByEmailAsync(model.Email);
        if (user != null && await _userManager.CheckPasswordAsync(user, model.Password))
        {
            //Buscar el rol que tiene cada usuario
            // var rolUsuario = _context.UserRoles.Where(r => r.UserId == user.Id);
            // string rolNombre = "";

            // var rolAsigando = rolUsuario.FirstOrDefault();
            // if (rolAsigando != null)
            // {
            //     var rol = _context.Roles.FirstOrDefault(r => r.Id == rolAsigando.RoleId);
            //     if (rol != null)
            //         rolNombre = rol.Name;
            // }

            //Si se encuentra el usuario y la contraseña es correcta, se crea el token
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.Email, user.Email),
                // new Claim(ClaimTypes.Role, rolNombre),
                new Claim(JwtRegisteredClaimNames. Jti, Guid.NewGuid().ToString())
            };

            //Se debe recuperar la Key seteada del appsettings.json
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            //Armamos el objeto con los aributos para generar el token
            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Issuer"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(15),
                signingCredentials: creds
            );

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);

            //Generar el refresh token
            var refreshToken = GenerarRefreshToken();
            //Guardamos el refresh token en la base de datos
            await _userManager.SetAuthenticationTokenAsync(user, "MyApp", "RefreshToken", refreshToken);
            return Ok(new
            {
                token = jwt,
                refreshToken = refreshToken,
                email = user.Email,
                nombreCompleto = user.NombreCompleto,
                // rol = rolNombre
            });
        }
        return Unauthorized("Credenciales inválidas");
    }

    // Generamos un token aleatorio seguro y lo convertimos a cadena Base64
    private string GenerarRefreshToken()
    {
        var randomBytes = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomBytes);
        return Convert.ToBase64String(randomBytes);
    }

    [HttpPost("refresh-token")]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest model)
    {
        var user = await _userManager.FindByEmailAsync(model.Email);
        if (user == null)
        {
            return Unauthorized("Usuario no válido");
        }

        var savedToken = await _userManager.GetAuthenticationTokenAsync(user, "MyApp", "RefreshToken");

        // Normalización básica (por si hay espacios o saltos de línea)
        var incomingToken = model.RefreshToken?.Trim();
        var storedToken = savedToken?.Trim();

        if (string.IsNullOrEmpty(incomingToken) || string.IsNullOrEmpty(storedToken))
        {
            return Unauthorized("Token faltante");
        }

        if (incomingToken != storedToken)
            return Unauthorized("Refresh token inválido");

        // Obtener rol del usuario
        // var rolUsuario = _context.UserRoles.Where(r => r.UserId == user.Id);
        // string rolNombre = "";

        // var rolAsigando = rolUsuario.FirstOrDefault();
        // if (rolAsigando != null)
        // {
        //     var rol = _context.Roles.FirstOrDefault(r => r.Id == rolAsigando.RoleId);
        //     if (rol != null)
        //         rolNombre = rol.Name;
        // }

        // Generar nuevo token JWT
        var claims = new[]
        {
        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
        new Claim(ClaimTypes.Name, user.UserName),
        new Claim(ClaimTypes.Email, user.Email),
        // new Claim(ClaimTypes.Role, rolNombre),
        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
    };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var newToken = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Issuer"],
            claims: claims,
            expires: DateTime.Now.AddDays(3),
            signingCredentials: creds
        );

        var jwt = new JwtSecurityTokenHandler().WriteToken(newToken);

        // Generar y guardar nuevo refresh token
        var newRefreshToken = GenerarRefreshToken();
        await _userManager.SetAuthenticationTokenAsync(user, "MyApp", "RefreshToken", newRefreshToken);

        return Ok(new
        {
            token = jwt,
            refreshToken = newRefreshToken,
            email = user.Email,
            nombreCompleto = user.NombreCompleto,
            // rol = rolNombre
        });
    }


    [HttpPost("logout")]
    public async Task<IActionResult> Logout([FromBody] LogoutRequest model)
    {
        var user = await _userManager.FindByEmailAsync(model.Email);
        if (user == null)
            return Unauthorized();

        await _userManager.RemoveAuthenticationTokenAsync(user, "MyApp", "RefreshToken");
        return Ok("Sesion cerrada Correctamente");
    }

}
