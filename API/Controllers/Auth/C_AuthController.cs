using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using API._Services.Interfaces;
using API.Helper.Params.Auth;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace API.Controllers.Auth
{
    [Route("api/[controller]")]
    [ApiController]
    public class C_AuthController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly I_Auth _iAuth;

        public C_AuthController(IConfiguration configuration,
                                I_Auth iAuth)
        {
            _configuration = configuration;
            _iAuth = iAuth;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(UserLoginParam param)
        {
            var userForLogin = await _iAuth.Login(param);
            if (userForLogin == null)
            {
                return Unauthorized();
            }
            var claims = new[]
           {
                new Claim(ClaimTypes.Name, userForLogin.Name)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8
                .GetBytes(_configuration.GetSection("AppSettings:Token").Value));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(7),
                SigningCredentials = creds
            };

            var tokenHandler = new JwtSecurityTokenHandler();

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return Ok(new
            {
                token = tokenHandler.WriteToken(token),
                user = userForLogin
            });
        }
    }
}