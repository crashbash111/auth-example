using Lab4API.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Lab4API.Models;
using Lab4API.Services;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly IConfiguration _configuration;
    private readonly IJwtTokenService _tokenService;

    public AuthController(ApplicationDbContext context, IConfiguration configuration, IJwtTokenService jwtTokenService)
    {
        _context = context;
        _configuration = configuration;
        // Inject the JWT token service we created
        _tokenService = jwtTokenService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] UserRegisterDto dto)
    {
        var user = new User
        {
            Username = dto.Username,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password)
        };

        _context.Users.Add(user);
        //adds user to db first so id can be generated, this can then be used for the userRole table itself
        await _context.SaveChangesAsync();

        //add user to standard user role

        var standardUserRole = await _context.Roles.SingleOrDefaultAsync(r => r.Name == "User");
        if(standardUserRole == null) return BadRequest(new { message = "Standard user role not found" });

        var userRole = new UserRole
        {
            UserId = user.Id,
            RoleId = standardUserRole.Id
        };
        _context.UserRoles.Add(userRole);
        await _context.SaveChangesAsync();

        return Ok(new { message = "User registered successfully" });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] UserLoginDto dto)
    {
        var user = await _context.Users.SingleOrDefaultAsync(u => u.Username == dto.Username);

        if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
        {
            return Unauthorized(new { message = "Invalid credentials" });
        }

        var token = _tokenService.GenerateToken(user);

        return Ok(new { token });
    }


}
