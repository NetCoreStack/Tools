using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using NetCoreStack.Hisar.WebCli.Tools.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace NetCoreStack.Hisar.WebCli.Tools
{
    public class Program
    {
        private readonly IConsole _console;
        private readonly string _workingDir;

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

        private async Task MainInternalAsync(string[] args)
        {
            var cmdOptions = CommandLineOptions.Parse(args, _console);
            var appdir = cmdOptions.MainAppDirectory.Value();
            if (!string.IsNullOrEmpty(appdir))
            {
                if (Directory.Exists(appdir))
                {
                    _console.Out.WriteLine("Main application directory is: " + appdir);
                    HostingHelper.MainAppDirectory = appdir;
                }
            }

            var urls = new List<string>()
            {
                $"http://*:{HostingHelper.GetHostingPort()}"
            };

            await Task.Run(() =>
            {
                var configuration = new ConfigurationBuilder()
                    .AddCommandLine(args).Build();

                var hostBuilder = new WebHostBuilder()
                    .UseConfiguration(configuration)
                    .UseContentRoot(Directory.GetCurrentDirectory())
                    .UseUrls(urls.ToArray())
                    .UseKestrel(options => options.AddServerHeader = false)
                    .UseIISIntegration()
                    .UseStartup<Startup>();
#if RELEASE
                hostBuilder.UseWebRoot(PathUtility.GetWebRootPath());
#endif
                var host = hostBuilder.Build();
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
