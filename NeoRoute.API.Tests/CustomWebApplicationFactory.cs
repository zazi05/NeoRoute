using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using NeoRoute.API.Data;

namespace NeoRoute.API.Tests
{
    public class CustomWebApplicationFactory : WebApplicationFactory<Program>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseEnvironment("Testing");

            builder.ConfigureServices(services =>
            {
                services.RemoveAll(typeof(DbContextOptions<RaceDayContext>));
                services.RemoveAll(typeof(RaceDayContext));

                var dbName = "RaceDayTestDb_" + Guid.NewGuid().ToString("N");
                services.AddDbContext<RaceDayContext>(options =>
                {
                    options.UseInMemoryDatabase(dbName);
                });

                services.PostConfigure<CookieAuthenticationOptions>(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    options =>
                    {
                        options.Cookie.SameSite = Microsoft.AspNetCore.Http.SameSiteMode.Lax;
                        options.Cookie.SecurePolicy = Microsoft.AspNetCore.Http.CookieSecurePolicy.None;
                        options.Cookie.HttpOnly = true;
                        options.Cookie.Name = "NeoRoute.Auth";
                    });
            });
        }
    }
}
