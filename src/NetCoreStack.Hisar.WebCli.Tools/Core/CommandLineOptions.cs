﻿using Microsoft.Extensions.CommandLineUtils;
using System;

namespace NetCoreStack.Hisar.WebCli.Tools.Core
{
    internal class CommandLineOptions
    {
        public CommandLineApplication App { get; private set; }
        public string Project { get; private set; }
        public bool IsHelp { get; private set; }
        public bool IsVerbose { get; private set; }
        public CommandOption MainAppDirectory { get; private set; }
        public CommandOption StaticServe { get; private set; }
        public CommandOption BuildComponent { get; private set; }
        public CommandOption CleanTemp { get; private set; }

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
  Hisar WebCli provides modular component development environements.
  You can manage all the files you specified with --appdir option or serve static files with --static option.
  However, NetCoreStack.Hisar rely on this tool to templating and create appropriate component packages.
  If you don't specify the main application directory it will create a default simple _Layout.cshtml page.

  For example: dotnet hisar --appdir <the-full-path-of-your-main-app>
               dotnet hisar --static <the-full-path-of-your-static-files>
               dotnet hisar --build  <the-component-project-directory> (MsBuild integrated)

Examples:
  dotnet hisar
  dotnet hisar --appdir C:/users/codes/project/src/Hisar.Hosting
  dotnet hisar --static C:/users/codes/project/wwwroot
  dotnet hisar --build C:/users/codes/project/src/Hisar.Component.Carousel
"
            };

            app.HelpOption("-?|-h|--help");

            var appdir = app.Option("-a|--appdir", "Main application directory", 
                CommandOptionType.SingleValue, inherited: true);

            var buildComponent = app.Option("-b|--build", "Component directory",
                CommandOptionType.SingleValue, inherited: true);

            var cleanTemp = app.Option("-c|--clean", "To clean NetCoreStackHisarWebCliTools temp directory and SQLite data which stores Layout.cshtml(s)",
                CommandOptionType.NoValue);

            var staticServer = app.Option("-s|--static", "It launches a server in the current working directory and serves all files in it",
                CommandOptionType.SingleValue, inherited: true);

            var optVerbose = app.Option("-v|--verbose", "Show verbose output", CommandOptionType.NoValue);

            if (app.Execute(args) != 0)
            {
                console.Out.WriteLine("invalid args syntax");
            }

            return new CommandLineOptions
            {
                App = app,
                IsVerbose = optVerbose.HasValue(),
                IsHelp = app.IsShowingInformation,
                MainAppDirectory = appdir,
                StaticServe = staticServer,
                BuildComponent = buildComponent,
                CleanTemp = cleanTemp
            };
        }
    }
}
