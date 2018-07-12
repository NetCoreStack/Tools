using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NetCoreStack.Hisar;

namespace Hisar.Component.Guideline
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();

            Configuration = builder.Build();
        }

        public void ConfigureServices(IServiceCollection services)
        {
#if !RELEASE
            services.AddCliSocket<Startup>();
#endif
            services.AddMvc();
        }

        public void Configure(IApplicationBuilder app)
        {
#if !RELEASE
            app.UseCliProxy();
#endif
            app.UseMvc(ConfigureRoutes);
        }

        public static void ConfigureRoutes(IRouteBuilder routes)
        {
            routes.MapRoute(
                name: "req",
                template: "registration",
                defaults: new { controller = "Home", action = "Registration" });

            routes.MapRoute(
                name: "default",
                template: "{controller=Home}/{action=Index}/{id?}");
        }

        public static void Main(string[] args)
        {
            BuildWebHost(args).Run();
        }

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<DefaultHisarStartup<Startup>>()
                .Build();
    }
}