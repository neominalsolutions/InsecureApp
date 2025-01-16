using InsecureApp.Db;
using InsecureApp.Models;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Linq;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace InsecureApp.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  [EnableCors("AllowAll")]
  public class RestController : ControllerBase
  {
    private readonly MyDbContext _context;

    public RestController(MyDbContext context)
    {
      _context = context;
    }

    // SQL Injection açığı var çünkü kullanıcı verisi doğrudan SQL sorgusuna ekleniyor
    [HttpGet("search")]
    public IActionResult Search(string param)
    {


      string query = $"SELECT * FROM Users WHERE Name = '{param}'";
      var results = _context.Database.ExecuteSqlRaw(query); // Sensitive, the FormattableString has already been evaluated, it won't be converted to a parametrized query.

      return Ok(results);
    }

    // JWT Token için güvensiz bir yapı kullanılmış (expiration, hashing vb. eksik)
    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginModel model)
    {
      if (model.Username == "admin" && model.Password == "password")
      {
        var token = GenerateJwtToken(model.Username);  // Güvensiz JWT politikası
        return Ok(new { token });
      }
      return Unauthorized();
    }

    private string GenerateJwtToken(string username)
    {
      // JWT güvenliği eksik: expiration, signing key, vb.
      var token = new JwtSecurityToken(
          claims: new[] { new Claim(ClaimTypes.Name, username) },
          expires: DateTime.Now.AddHours(1),
          signingCredentials: new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes("qwertyuiopasdfghjklzxcvbnm123456")), SecurityAlgorithms.HmacSha256));

      return new JwtSecurityTokenHandler().WriteToken(token);
    }
  }
}
