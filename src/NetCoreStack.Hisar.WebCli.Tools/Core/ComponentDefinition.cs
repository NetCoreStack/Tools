using System;
using System.Collections.Generic;
using System.Linq;

namespace NetCoreStack.Hisar.WebCli.Tools.Core
{
    public class ComponentDefinition
    {
        public string Namespace { get; }
        public string ProjectDirectory { get; }
        public string ComponentId { get; }
        public IDictionary<string, string> ComponentDependencies { get; }

        public ComponentDefinition(string assemblyName, 
            string projectDirectory, 
            IDictionary<string, string> componentDependencies)
        {
            if (string.IsNullOrEmpty(assemblyName))
            {
                throw new ArgumentNullException(nameof(assemblyName));
            }

            if (string.IsNullOrEmpty(projectDirectory))
            {
                throw new ArgumentNullException(nameof(assemblyName));
            }

            if (componentDependencies == null)
            {
                throw new ArgumentNullException(nameof(componentDependencies));
            }

            Namespace = assemblyName;
            ComponentId = assemblyName.Split('.').Last();
            ProjectDirectory = projectDirectory;
            ComponentDependencies = componentDependencies;
        }
    }
}
