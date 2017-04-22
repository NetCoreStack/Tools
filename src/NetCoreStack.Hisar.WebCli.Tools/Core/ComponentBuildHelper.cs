using NetCoreStack.Hisar.WebCli.Tools.Interfaces;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace NetCoreStack.Hisar.WebCli.Tools.Core
{
    public static class ComponentBuildHelper
    {
        private static List<string> excludeFiles = new List<string>
        {
            "favicon.ico",
            "_references.js"
        };

        private static List<string> excludeStartsWith = new List<string>
        {
            "glyphicons-",
            "font-awesome"
        };

        private static List<string> searchDirectories = new List<string>
        {
            "css",
            "img",
            "images",
            "js",
            "fonts"
        };

        /// <summary>
        /// 
        /// </summary>
        /// <param name="componentId"></param>
        /// <param name="directory"></param>
        /// <returns>List of original files to be replace</returns>
        private static List<string> RenameFiles(string componentId, string directory)
        {
            List<string> originalFiles = new List<string>();
            var directoryInfo = new DirectoryInfo(directory);
            var path = Path.Combine(directoryInfo.FullName, "wwwroot");
            if (Directory.Exists(path))
            {
                var directories = Directory.GetDirectories(path);
                foreach (var dic in directories)
                {
                    var dInfo = new DirectoryInfo(dic);
                    if (searchDirectories.Contains(dInfo.Name))
                    {
                        var files = dInfo.GetFiles("*.*", SearchOption.AllDirectories);
                        if (files.Any())
                        {
                            foreach (var fInfo in files)
                            {
                                var fileName = fInfo.Name;
                                if (excludeFiles.Contains(fileName))
                                    continue;

                                if (excludeStartsWith.Any(p => fileName.StartsWith(p)))
                                    continue;

                                var prefix = $"{componentId.ToLowerInvariant()}-";

                                if (fileName.StartsWith(prefix))
                                    continue;

                                originalFiles.Add(fInfo.FullName);
                                var movePath = Path.Combine(dInfo.FullName, $"{prefix}{fileName}");
                                File.Move(fInfo.FullName, movePath);
                            }
                        }
                    }
                }
            }

            return originalFiles;
        }

        private static string[] GetViews(string directory)
        {
            var directoryInfo = new DirectoryInfo(directory);
            var path = Path.Combine(directoryInfo.FullName, "Views");
            if (Directory.Exists(path))
            {
                return Directory.GetFiles(path, "*.cshtml", SearchOption.AllDirectories);
            }

            return null;
        }

        public static IEnumerable<IComponentFileReplacer> Build(ComponentDefinition definition)
        {
            RenameFiles(definition.ComponentId, definition.ProjectDirectory);
            var list = new List<IComponentFileReplacer>();
            return list;
        }
    }
}
