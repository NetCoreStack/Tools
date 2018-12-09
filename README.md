### Hisar Web CLI Tools: Cross-Platform Modular Component Development Web CLI
[![NuGet](https://img.shields.io/nuget/v/NetCoreStack.Hisar.WebCli.Tools.svg?longCache=true&style=flat-square)](https://www.nuget.org/packages/NetCoreStack.Hisar.WebCli.Tools)
[![NuGet](https://img.shields.io/nuget/dt/NetCoreStack.Hisar.WebCli.Tools.svg?longCache=true&style=flat-square)](https://www.nuget.org/packages/NetCoreStack.Hisar.WebCli.Tools)


Hisar WebCli provides modular component development without dependencies.
[NetCoreStack.Hisar](https://github.com/NetCoreStack/Hisar) supports any standalone MVC app that it can be part of 
hosting application package as a component. Please check out NetCoreStack.Hisar on [Github]([NetCoreStack.Hisar](https://github.com/NetCoreStack/Hisar)) for more details.
Each component-module should know what layout.cshtml looks like and then this tool can manage it for you.
If you don't specify the main application directory it will create the default _Layout.cshtml page.


### Install
Download the .NET Core SDK 2.2.0 or newer. Once installed, run this command:

	dotnet tool install --global dotnet-hisar --version 2.2.0

### Usage

    Usage: dotnet hisar [options] [[--] <arg>...]

    Options:
    -?|-h|--help  Show help information
    -a|--appdir   Main application directory
    -b|--build    Component directory
    -c|--clean    To clean NetCoreStackHisarWebCliTools temp directory and SQLite data which stores Layout.cshtml(s)
    -s|--static   It launches a server in the current working directory and serves all files in it.
    -v|--verbose  Show verbose output

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

![](https://github.com/NetCoreStack/Tools/blob/master/NetCoreStackTools.gif)

### Build
    Run pack.ps1 or pack.sh according to the OS to create Cli Tools nuget package.
    Run dotnet restore command on root directory (where the NetCoreStack.Tools.sln file)

[Latest release on Nuget](https://www.nuget.org/packages/NetCoreStack.Hisar.WebCli.Tools/)

### Prerequisites
> [ASP.NET Core](https://github.com/aspnet/Home)
