using Microsoft.AspNetCore.Authentication.Cookies;
namespace mvc_app
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddDbContext<UsersContext>();
            builder.Services.AddScoped<IUserService, UserService>();

            // ИСПРАВЛЕННЫЙ БЛОК: Явно указываем "CookieAuth" как дефолтную схему
            builder.Services.AddAuthentication("CookieAuth")
                .AddCookie("CookieAuth", options =>
                {
                    options.LoginPath = "/Account/Login";
                    options.Cookie.Name = "MyMvcAppAuthCookie";
                });

            builder.Services.AddAuthorization();
            builder.Services.AddControllersWithViews();

            var app = builder.Build();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Account}/{action=Welcome}/{id?}");

            app.Run();
        }
    }
}