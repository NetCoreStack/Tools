using System;
using System.IO;

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

            var componentId = componentDefinition.ComponentId;
            var directory = componentDefinition.ProjectDirectory;
            var nameSpace = componentDefinition.Namespace;
            var startupFile = Path.Combine(directory, "Startup.Component.cs");
            if (!File.Exists(startupFile))
            {
                var componentStartup = string.Format(Properties.Resource.ComponentInfoFileContent, nameSpace, componentId);
                File.WriteAllText(startupFile, componentStartup);
            }
        }
    }
}
