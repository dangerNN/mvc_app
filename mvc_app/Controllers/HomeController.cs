using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace mvc_app.Controllers
{
    [Authorize]
    public class HomeController : Controller
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
            return View(users);
        }

        // ВИПРАВЛЕНО: Прибрали ("{id}"), щоб не ламати стандартний маршрут MVC
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null)
                return NotFound();

            return View(user);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(User user)
        {
            if (!ModelState.IsValid)
                return View(user);
            await _userService.CreateUserAsync(user);
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Update(int id)
        {
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null)
                return NotFound();
            return View(user);
        }

        [HttpPost]
        public async Task<IActionResult> Update(int id, User user)
        {
            if (ModelState.IsValid)
            {
                var updatedUser = await _userService.UpdateUserAsync(id, user);
                if (updatedUser == null)
                    return NotFound();

                return RedirectToAction(nameof(Index));
            }
            return View(user);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null)
                return NotFound();

            return View(user);
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var deletedUser = await _userService.DeleteUserAsync(id);
            if (deletedUser == null)
                return NotFound();

            return RedirectToAction(nameof(Index));
        }

        // Окремий атрибут [Authorize] прибрали, бо він уже діє на весь клас
        [HttpGet]
        public IActionResult Tools()
        {
            return View();
        }
    }
}