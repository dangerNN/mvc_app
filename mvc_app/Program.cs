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

            builder.Services.AddAuthentication("CookieAuth")
                .AddCookie("CookieAuth", options =>
                {
                    options.LoginPath = "/login.html";
                    options.Cookie.Name = "MyMvcAppAuthCookie";
                });

            builder.Services.AddAuthorization();

            builder.Services.AddControllers();

            var app = builder.Build();

            app.UseDefaultFiles();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}