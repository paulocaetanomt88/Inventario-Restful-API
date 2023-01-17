using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using InventarioRestfulAPI.Models;


namespace InventarioRestfulAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TokenController : ControllerBase
    {
        public IConfiguration _configuration;
        private readonly AppDbContext _context;

        public TokenController(IConfiguration configuration, AppDbContext context)
        {
            _configuration = configuration;
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> AutenticaUsuario(Usuario _usuario)
        {
            if(_usuario != null && _usuario.Email != null && _usuario.Senha != null)
            {
                var usuario = await GetUsuario(_usuario.Email, _usuario.Senha);

                if(usuario != null)
                {
                    // cria claims baseado nas informações do usuário
                    var claims = new[]
                    {
                        new Claim(JwtRegisteredClaimNames.Sub, _configuration["Jwt:Subject"]),
                        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                        new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString()),
                        new Claim("Id", usuario.UsuarioId.ToString()),
                        new Claim("Nome", usuario.Nome),
                        new Claim("Login", usuario.Login),
                        new Claim("Email", usuario.Email)
                    };

                    // Definimos a chave usando o aplicano do método SymmetricSecutiryKey e usando a chave secreta;
                    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));

                    // Definimos as credenciais;
                    var signIn = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                    // Geramos o token com tempo de expiração(expires) de 1 dia
                    var token = new JwtSecurityToken(_configuration["Jwt:Issuer"],
                                 _configuration["Jwt:Audience"], claims,
                                 expires: DateTime.UtcNow.AddDays(1), signingCredentials: signIn);

                    return Ok(new JwtSecurityTokenHandler().WriteToken(token));
                }
                else
                {
                    return BadRequest("Credenciais inválidas");
                }
            }

            else
            {
                return BadRequest();
            }
        }
        private async Task<Usuario> GetUsuario(string email, string password)
        {
            return await _context.Usuarios.FirstOrDefaultAsync(u => u.Email == email && u.Senha == password);
        }
    }
}
