using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NetCoreStack.Hisar.WebCli.Tools.Context;
using NetCoreStack.Hisar.WebCli.Tools.Core;
using NetCoreStack.WebSockets;
using System.IO;

namespace NetCoreStack.Hisar.WebCli.Tools
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddOptions();
            services.AddMemoryCache();

            ComponentDefinition componentInfo = PathUtility.GetComponentInfo(Directory.GetCurrentDirectory());
            services.AddSingleton<ComponentDefinition>(componentInfo);
            services.AddSingleton<EnvironmentContext>();

            var appDirectory = PathUtility.GetTempDirectory();
            Directory.CreateDirectory(appDirectory);
            var databaseFullPath = Path.Combine(appDirectory, HostingConstants.DatabaseName);

            var mainAppWebRoot = string.Empty;
            if (!string.IsNullOrEmpty(HostingHelper.MainAppDirectory))
            {
                mainAppWebRoot = PathUtility.GetWebRootDirectory(HostingHelper.MainAppDirectory);
            }

            services.AddSingleton(new CliEnvironment(appDirectory, databaseFullPath, mainAppWebRoot));

            services.AddEntityFrameworkSqlite()
                .AddDbContext<HisarCliContext>(options =>
                {
                    options.UseSqlite($"Data Source={databaseFullPath}");
                });

            services.AddNativeWebSockets<ServerWebSocketCommandInvocator>();

            services.AddMvc(options => {
                options.CacheProfiles.Add("Never", new CacheProfile()
                {
                    Location = ResponseCacheLocation.None,
                    NoStore = true
                });
            });
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseNativeWebSockets();

            app.UseDefaultFiles();

            var environmentContext = app.ApplicationServices.GetService<EnvironmentContext>();

            app.UseStaticFiles(new StaticFileOptions()
            {
                FileProvider = new CustomFileProvider(environmentContext)
            });

            app.UseStaticFiles();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });

            DataInitializer.InitializeDb(app.ApplicationServices, environmentContext);
        }
    }
}
