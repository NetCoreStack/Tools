﻿using Hisar.Component.Guideline.Filters;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NetCoreStack.Hisar;
using System.IO;

namespace Hisar.Component.Guideline
{
    public class Startup
    {
        public IConfigurationRoot Configuration { get; }

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
            services.AddSingleton<ILayoutFilter, GuidelineLayoutWebPackFilter>();
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
            var host = new WebHostBuilder()
                .UseKestrel()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseIISIntegration()
                .UseStartup<DefaultHisarStartup<Startup>>()
                .Build();

            host.Run();
        }
    }
}