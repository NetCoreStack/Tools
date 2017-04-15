using NetCoreStack.Hisar.WebCli.Tools.Context;
using NetCoreStack.Hisar.WebCli.Tools.Core;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NetCoreStack.WebSockets;
using Swashbuckle.Swagger.Model;
using System.IO;
using System.Reflection;

namespace NetCoreStack.Hisar.WebCli.Tools
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();

            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMemoryCache();

            services.AddOptions();

            services.AddSingleton(_ => Configuration);
            services.AddSingleton<EnvironmentContext>();

            var appDirectory = Path.Combine(Path.GetTempPath(), HostingConstants.PackageName.Replace(".",""));
            Directory.CreateDirectory(appDirectory);
            var databaseFullPath = Path.Combine(appDirectory, HostingConstants.DatabaseName);

            services.AddSingleton(new CliEnvironment(appDirectory, databaseFullPath));

            services.AddEntityFramework()
                .AddEntityFrameworkSqlite()
                .AddDbContext<HisarCliContext>(options =>
                {
                    options.UseSqlite($"Data Source={databaseFullPath}");
                });

            services.AddNativeWebSockets(options => {
                options.RegisterInvocator<ServerWebSocketCommandInvocator>(WebSocketCommands.All);
            });

            services.AddSwaggerGen(c =>
            {
                c.SingleApiVersion(new Info
                {
                    Version = "v1",
                    Title = "API V1",
                    TermsOfService = "None"
                });

                c.DescribeAllEnumsAsStrings();
            });

            services.AddMvc();
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
            app.UseStaticFiles();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });

            app.UseSwagger();
            app.UseSwaggerUi();

            DataInitializer.InitializeDb(app.ApplicationServices);
        }
    }
}
