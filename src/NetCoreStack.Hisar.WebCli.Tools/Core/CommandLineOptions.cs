using Microsoft.Extensions.CommandLineUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace NetCoreStack.Hisar.WebCli.Tools.Core
{
    internal class CommandLineOptions
    {
        public CommandLineApplication App { get; private set; }
        public string Project { get; private set; }
        public bool IsHelp { get; private set; }
        public bool IsVerbose { get; private set; }
        public CommandOption MainAppDirectory { get; private set; }

        public static CommandLineOptions Parse(string[] args, IConsole console)
        {
            if (args == null)
            {
                throw new ArgumentNullException(nameof(args));
            }

            if (console == null)
            {
                throw new ArgumentNullException(nameof(console));
            }

            var app = new CommandLineApplication(throwOnUnexpectedArg: false)
            {
                Name = "dotnet hisar",
                FullName = "NetCoreStack Hisar WebCLI",
                Out = console.Out,
                Error = console.Error,
                AllowArgumentSeparator = true,
                ExtendedHelpText = @"
Remarks:
  Hisar WebCLI provides modular component development without dependencies.
  NetCoreStack.Hisar supports any standalone MVC app that it can be part of 
  hosting application package as a component. Please check out NetCoreStack.Hisar on Github for more details.
  Each component-module should know what layout.cshtml looks like and then this tool can manage it for you.
  If you don't specify the main application directory it will create the default _Layout.cshtml page.

  For example: dotnet hisar --appdir <the-full-path-of-your-main-app>

Examples:
  dotnet hisar
  dotnet hisar --appdir C:/users/codes/project/src/WebApp.Hosting
"
            };

            app.HelpOption("-?|-h|--help");
            var appdir = app.Option("-l|--appdir", "Main application directory that contains _Layout.cshtml page (Main Templating)", 
                CommandOptionType.SingleValue, inherited: true);

            var optVerbose = app.Option("-v|--verbose", "Show verbose output", CommandOptionType.NoValue);

            if (app.Execute(args) != 0)
            {
                console.Out.WriteLine("invalid args syntax");
            }

            app.ShowHelp();

            return new CommandLineOptions
            {
                App = app,
                IsVerbose = optVerbose.HasValue(),
                IsHelp = app.IsShowingInformation,
                MainAppDirectory = appdir
            };
        }
    }
}
