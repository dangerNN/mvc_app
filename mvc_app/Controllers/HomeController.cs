using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace mvc_app.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/students")]
    public class HomeController : ControllerBase
    {
        private readonly IUserService _userService;

        public HomeController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var users = await _userService.GetUsersAsync();
            return Ok(users);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Details(int id)
        {
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null)
                return NotFound();

            return Ok(user);
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] User user)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await _userService.CreateUserAsync(user);

            return StatusCode(201, user);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] User user)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var updatedUser = await _userService.UpdateUserAsync(id, user);
            if (updatedUser == null)
                return NotFound();

            return Ok(updatedUser);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var currentUserIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var deletedUser = await _userService.DeleteUserAsync(id);
            if (deletedUser == null)
                return NotFound();

            if (currentUserIdClaim != null && currentUserIdClaim == id.ToString())
            {
                await HttpContext.SignOutAsync("CookieAuth");
                return Ok(new { message = "User deleted and signed out.", selfDelete = true });
            }

            return Ok(new { message = "Студент успешно удален", selfDelete = false });
        }

        [HttpGet("tools")]
        public IActionResult Tools()
        {
            return Ok(new { info = "Это секретные инструменты API" });
        }
    }
}