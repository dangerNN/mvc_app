using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace mvc_app.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AccountController : ControllerBase
    {
        private readonly IUserService _userService;

        public AccountController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> ApiLogin([FromBody] LoginDto loginData)
        {
            var users = await _userService.GetUsersAsync();
            var user = users.FirstOrDefault(u => u.Email == loginData.Email && u.Password == loginData.Password);

            if (user == null)
                return Unauthorized(new { message = "Неверный логин или пароль" });

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Name ?? "Студент")
            };

            var identity = new ClaimsIdentity(claims, "CookieAuth");
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync("CookieAuth", principal);

            return Ok(new { message = "Успешный вход", userName = user.Name });
        }

        [HttpGet("logout")]
        public async Task<IActionResult> ApiLogout()
        {
            await HttpContext.SignOutAsync("CookieAuth");
            return Redirect("/login.html");
        }
    }

    public class LoginDto
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}