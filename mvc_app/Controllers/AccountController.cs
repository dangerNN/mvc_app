using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace mvc_app.Controllers
{
    public class AccountController : Controller
    {
        private readonly UsersContext _context;

        public AccountController(UsersContext context)
        {
            _context = context;
        }

        // 1. НОВАЯ СТАРТОВАЯ СТРАНИЦА (Доступна всем)
        [HttpGet]
        public IActionResult Welcome()
        {
            // Если пользователь ОУЖЕ вошел, сразу отправляем его на главную
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        // --- РЕЕСТРАЦИЯ ---
        [HttpGet]
        public IActionResult Register() => View();

        [HttpPost]
        public async Task<IActionResult> Register(User user)
        {
            // Проверяем самые важные поля
            if (string.IsNullOrEmpty(user.Email) || string.IsNullOrEmpty(user.Password))
            {
                ModelState.AddModelError("", "Email и Пароль обязательны для заполнения!");
                return View(user);
            }

            // Проверяем дубликаты по Email
            var userExists = await _context.Users.AnyAsync(u => u.Email == user.Email);
            if (userExists)
            {
                ModelState.AddModelError("", "Пользователь с таким Email уже зарегистрирован!");
                return View(user);
            }

            // Принудительно заполняем Name, если оно пустое, чтобы не было ошибок в БД
            if (string.IsNullOrEmpty(user.Name))
            {
                user.Name = user.Email.Split('@')[0];
            }

            // Добавляем в базу и ОБЯЗАТЕЛЬНО сохраняем
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // После успешной регистрации сразу перенаправляем на Вход
            return RedirectToAction("Login");
        }

        // --- ВХОД (ЛОГИН) ---
        [HttpGet]
        public IActionResult Login() => View();

        [HttpPost]
        public async Task<IActionResult> Login(string email, string password)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                ModelState.AddModelError("", "Заполните все поля!");
                return View();
            }

            // Ищем пользователя (обрезаем пробелы на случай опечаток)
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email.ToLower() == email.Trim().ToLower() && u.Password == password);

            if (user == null)
            {
                ModelState.AddModelError("", "Неверный Email или Пароль!");
                return View();
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Name ?? user.Email),
                new Claim(ClaimTypes.Email, user.Email)
            };

            var identity = new ClaimsIdentity(claims, "CookieAuth");
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync("CookieAuth", principal);

            return RedirectToAction("Index", "Home");
        }

        // --- ВЫХОД ---
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync("CookieAuth");
            return RedirectToAction("Welcome", "Account"); // После выхода — на стартовую
        }
    }
}