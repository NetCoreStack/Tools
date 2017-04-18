using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NetCoreStack.Hisar.WebCli.Tools.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace NetCoreStack.Hisar.WebCli.Tools
{
    public class Program
    {
        private readonly IConsole _console;
        private readonly string _workingDir;
        private List<string> _urls;

        public Program(IConsole console, string workingDir)
        {
            if (console == null)
            {
                throw new ArgumentNullException(nameof(console));
            }

            if (string.IsNullOrEmpty(workingDir))
            {
                throw new ArgumentNullException(nameof(workingDir));
            }

            _console = console;
            _workingDir = workingDir;
        }

        private void ApplicationStarted()
        {
            Task.Run(() =>
            {
                var url = _urls.FirstOrDefault().Replace("*", "localhost");
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    Process.Start("CMD.exe", "/C start \"\" " + url);
                }
                if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                {
                    Process.Start("open", url);
                }
            });
        }

        private async Task MainInternalAsync(string[] args)
        {
            var cmdOptions = CommandLineOptions.Parse(args, _console);
            var appdir = cmdOptions.MainAppDirectory.Value();
            var staticServe = cmdOptions.StaticServe.Value();

            if (!string.IsNullOrEmpty(appdir))
            {
                if (Directory.Exists(appdir))
                {
                    _console.Out.WriteLine("Main application directory is: " + appdir);
                    HostingHelper.MainAppDirectory = appdir;
                }
            }

            if (!string.IsNullOrEmpty(staticServe))
            {
                if (Directory.Exists(staticServe))
                {
                    _console.Out.WriteLine("Static files directory is: " + appdir);
                    HostingHelper.StaticServe = staticServe;
                }
            }

            _urls = new List<string>()
            {
                $"http://*:{HostingHelper.GetHostingPort()}"
            };

            await Task.Run(() =>
            {
                var configuration = new ConfigurationBuilder()
                    .AddCommandLine(args).Build();

                var contentRoot = Directory.GetCurrentDirectory();

#if RELEASE
                contentRoot = PathUtility.GetRootPath(true);
#endif
                var hostBuilder = new WebHostBuilder()
                    .UseConfiguration(configuration)
                    .UseContentRoot(contentRoot)
                    .UseUrls(_urls.ToArray())
                    .UseKestrel(options => options.AddServerHeader = false)
                    .UseIISIntegration()
                    .UseStartup<Startup>();
#if RELEASE
                hostBuilder.UseWebRoot(PathUtility.GetRootPath());
#endif

                bool serveStatic = !string.IsNullOrEmpty(HostingHelper.StaticServe);
                if (serveStatic)
                {
                    hostBuilder.UseWebRoot(HostingHelper.StaticServe);
                }

                var host = hostBuilder.Build();

                if (!serveStatic)
                {
                    var applicationLifetime = host.Services.GetService<IApplicationLifetime>();
                    applicationLifetime.ApplicationStarted.Register(ApplicationStarted);
                }

                host.Run();
            });
        }

        public static int Main(string[] args)
        {
            return new Program(PhysicalConsole.Singleton, Directory.GetCurrentDirectory())
                .RunAsync(args)
                .GetAwaiter()
                .GetResult();
        }

        public async Task<int> RunAsync(string[] args)
        {
            try
            {
                await MainInternalAsync(args);
                return 0;
            }
            catch (Exception ex)
            {
                await _console.Out.WriteLineAsync("Exception: " + ex?.Message);

                if (ex is TaskCanceledException || ex is OperationCanceledException)
                {
                    // swallow when only exception is the CTRL+C forced an exit
                    return 0;
                }

                return 1;
            }
        }
    }
}
