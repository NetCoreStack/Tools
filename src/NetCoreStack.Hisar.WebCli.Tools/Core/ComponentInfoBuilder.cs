using System;
using System.IO;
using System.Linq;

namespace NetCoreStack.Hisar.WebCli.Tools.Core
{
    public static class ComponentInfoBuilder
    {
        public static void Build(IConsole console, string projectDirectory)
        {
            var componentDefinition = PathUtility.GetComponentInfo(projectDirectory);
            if (componentDefinition == null)
            {
                console.Out.WriteLine("Component build directory is: " + projectDirectory + Environment.NewLine);
                console.Out.WriteLine("===Build Waring: Component Definition could not be found!");
            }

            console.Out.WriteLine("===Build: Hisar Cli - Resolved Component Id: " + componentDefinition.ComponentId);

            var directory = componentDefinition.ProjectDirectory;
            var nameSpace = componentDefinition.Namespace;
            var startupFile = Path.Combine(directory, "Startup.Component.cs");
            if (!File.Exists(startupFile))
            {
                var componentStartup = string.Format(Properties.Resource.ComponentInfoFileContent, nameSpace);
                File.WriteAllText(startupFile, componentStartup);
            }

            var directoryInfo = new DirectoryInfo(directory);
            var viewImport = directoryInfo.GetFiles("_ViewImports.cshtml", SearchOption.AllDirectories).FirstOrDefault();
            if (viewImport != null)
            {
                var content = File.ReadAllText(viewImport.FullName);
                var usingNamespace = $"@using {nameSpace}";
                var searchFor = $"@using static {nameSpace}.ComponentInfo";

                File.AppendAllText(viewImport.FullName, Environment.NewLine + usingNamespace + Environment.NewLine);

                if (!content.Contains(searchFor))
                {
                    File.AppendAllText(viewImport.FullName, Environment.NewLine + searchFor + Environment.NewLine);
                }
            }
        }
    }
}
