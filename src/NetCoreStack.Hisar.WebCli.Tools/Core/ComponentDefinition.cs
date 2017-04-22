using System;
using System.Linq;

namespace NetCoreStack.Hisar.WebCli.Tools.Core
{
    public class ComponentDefinition
    {
        public string Namespace { get; }
        public string ProjectDirectory { get; }
        public string ComponentId { get; }

        public ComponentDefinition(string assemblyName, string projectDirectory)
        {
            if (string.IsNullOrEmpty(assemblyName))
            {
                throw new ArgumentNullException(nameof(assemblyName));
            }

            if (string.IsNullOrEmpty(projectDirectory))
            {
                throw new ArgumentNullException(nameof(assemblyName));
            }

            Namespace = assemblyName;
            ComponentId = assemblyName.Split('.').Last();
            ProjectDirectory = projectDirectory;
        }
    }
}
