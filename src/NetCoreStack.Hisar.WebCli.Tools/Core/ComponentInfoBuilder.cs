using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

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
            var startupComponentFile = Path.Combine(directory, "Startup.Component.cs");
            if (!File.Exists(startupComponentFile))
            {
                var componentStartup = string.Format(Properties.Resource.ComponentFileContent, nameSpace, componentId);
                File.WriteAllText(startupComponentFile, componentStartup);
            }

            var directoryInfo = new DirectoryInfo(directory);
            var viewImport = directoryInfo.GetFiles("_ViewImports.cshtml", SearchOption.AllDirectories).FirstOrDefault();
            if (viewImport != null)
            {
                var content = File.ReadAllText(viewImport.FullName);
                var searchFor = $"@using static {nameSpace}.ComponentHelper";

                if (!content.Contains(searchFor))
                {
                    File.AppendAllText(viewImport.FullName, Environment.NewLine + searchFor);
                }
            }

            var startupComponentDependencies = Path.Combine(directory, "Startup.ComponentDependencies.cs");
            StringBuilder sb = new StringBuilder();

            var dependencies = componentDefinition.ComponentDependencies;
            if (dependencies != null)
            {
                var firstKey = dependencies.First().Key;
                var lastKey = dependencies.Last().Key;

                foreach (KeyValuePair<string, string> entry in dependencies)
                {
                    if (entry.Key == firstKey)
                    {
                        sb.AppendLine($"[\"{entry.Key}\"] = \"{entry.Value}\",");
                    }
                    else if(entry.Key == lastKey)
                    {
                        sb.AppendLine($"\t\t\t[\"{entry.Key}\"] = \"{entry.Value}\"");
                    }
                    else
                    {
                        sb.AppendLine($"\t\t\t[\"{entry.Key}\"] = \"{entry.Value}\",");
                    }
                }
            }

            var dependenciesContent = string.Format(Properties.Resource.ComponentDependenciesFileContent, nameSpace, sb.ToString());
            File.WriteAllText(startupComponentDependencies, dependenciesContent);
        }
    }
}
