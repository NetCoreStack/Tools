### Hisar Web CLI Tools: Cross-Platform Modular Component Development Web CLI

Hisar WebCLI provides modular component development without dependencies.
[NetCoreStack.Hisar](https://github.com/NetCoreStack/Hisar) supports any standalone MVC app that it can be part of 
hosting application package as a component. Please check out NetCoreStack.Hisar on [Github]([NetCoreStack.Hisar](https://github.com/NetCoreStack/Hisar)) for more details.
Each component-module should know what layout.cshtml looks like and then this tool can manage it for you.
If you don't specify the main application directory it will create the default _Layout.cshtml page.

For example: dotnet hisar --appdir <the-full-path-of-your-main-app>

![](https://github.com/NetCoreStack/Tools/blob/master/NetCoreStackTools.gif)

### Build
    Run pack.ps1 or pack.sh according to the OS to create Cli Tools nuget package.
    Run dotnet restore command on root directory (where the NetCoreStack.Tools.sln file)

[Latest release on Nuget](https://www.nuget.org/packages/NetCoreStack.Hisar.WebCli.Tools/)

### Prerequisites
> [ASP.NET Core](https://github.com/aspnet/Home)
