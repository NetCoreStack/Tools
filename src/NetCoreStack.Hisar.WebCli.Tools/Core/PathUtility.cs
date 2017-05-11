using NetCoreStack.Hisar.WebCli.Tools.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;

namespace NetCoreStack.Hisar.WebCli.Tools.Core
{
    public static class PathUtility
    {
        private const string ContentFolderName = "content";

        public static string NormalizeToWebPath(string filter)
        {
            // OS forward slash check.
            if(filter.StartsWith("/"))
            {
                return filter;
            }
            
            if (!filter.StartsWith("\\"))
            {
                filter = "\\" + filter;
            }

            return filter.Replace('\\', '/');
        }

        public static string GetTempDirectory() => Path.Combine(Path.GetTempPath(), HostingConstants.PackageName.Replace(".", ""));

        public static void CleanTempDirectory()
        {
            var tempDir = GetTempDirectory();
            if (Directory.Exists(tempDir))
            {
                Directory.Delete(tempDir, true);
            }
        }

        public static string NormalizeToOSPath(string filter, bool replaceStartsWithSlash = false)
        {
            if (replaceStartsWithSlash && (filter.StartsWith("\\") || filter.StartsWith("/")))
            {
                filter = filter.Substring(1);
            }

            return filter.Replace('/', '\\');
        }

        public static string NormalizeRelavitePath(string rootPath, string path)
        {
            if (string.IsNullOrEmpty(rootPath))
            {
                throw new ArgumentNullException(nameof(rootPath));
            }

            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentNullException(nameof(path));
            }

            if (!Path.IsPathRooted(path))
            {
                return Path.Combine(rootPath, path);
            }

            return path;
        }

        public static string GetRootPath(bool isContentRoot = false)
        {
            var assemblyPath = Assembly.GetEntryAssembly().Location;
            var directory = Path.GetDirectoryName(assemblyPath);
            var directoryInfo = new DirectoryInfo(directory);
            do
            {
                var path = string.Empty;
                if (isContentRoot)
                    path = Path.Combine(directoryInfo.FullName, "content");
                else
                    path = Path.Combine(directoryInfo.FullName, "content", "wwwroot");
                
                var root = new DirectoryInfo(path);
                if (root.Exists)
                {
                    return root.FullName;
                }

                directoryInfo = directoryInfo.Parent;
            }
            while (directoryInfo.Parent != null);

            var message = $"Ex: wwwroot could not be located using package root {assemblyPath}";
            throw new Exception(message);
        }

        public static string GetWebRootDirectory(string directory)
        {
            var directoryInfo = new DirectoryInfo(directory);
            do
            {
                var path = Path.Combine(directoryInfo.FullName, "wwwroot");
                var root = new DirectoryInfo(path);
                if (root.Exists)
                {
                    return root.FullName;
                }

                directoryInfo = directoryInfo.Parent;
            }
            while (directoryInfo.Parent != null);

            var message = $"Ex: wwwroot could not be located in {directory}";
            throw new Exception(message);
        }

        public static string GetLayoutPagePath(string directory)
        {
            var name = Path.GetFileName(HostingConstants.LayoutPageFullName);
            var files = Directory.GetFiles(directory, name, SearchOption.AllDirectories);
            if (files.Any())
            {
                var layoutPagePath = files.FirstOrDefault();
                return layoutPagePath;
            }

            var message = $"Ex: {HostingConstants.LayoutPageFullName} could not be located in {directory}";
            throw new Exception(message);
        }

        public static ComponentDefinition GetComponentInfo(string directory)
        {
            if (string.IsNullOrEmpty(directory))
            {
                throw new ArgumentNullException(nameof(directory));
            }
            
            var files = Directory.GetFiles(directory, "*.csproj", SearchOption.AllDirectories);
            if (files.Any())
            {
                var csprojFile = files.FirstOrDefault();
                using (var fs = new FileStream(csprojFile, FileMode.Open))
                {
                    XDocument document = XDocument.Load(fs);
                    var candidatePropertyGroup = document.Element("Project")?.Descendants("PropertyGroup")
                        .Where(e => e.HasElements && e.Element("AssemblyName") != null).FirstOrDefault();

                    var elementValue = candidatePropertyGroup?.Element("AssemblyName")?.Value;
                    if (string.IsNullOrEmpty(elementValue))
                    {
                        elementValue = Path.GetFileNameWithoutExtension(csprojFile);
                    }

                    var dependencyTree = document.Element("Project")?.Descendants("PackageReference").ToList();
                    var dependencies = dependencyTree.ToDictionary(x => x.Attribute("Include").Value, x => x.Attribute("Version").Value);
                    return new ComponentDefinition(elementValue, directory, dependencies);
                }
            }

            return null;
        }

        private static List<string> _excludeDirs = new List<string> { "bin", "obj", "node_modules" };
        public static List<JsTreeDataModel> TreeList = new List<JsTreeDataModel>();
        // TODO Cache
        public static List<JsTreeDataModel> WalkDirectoryTree(DirectoryInfo directory, JsTreeDataModel tree)
        {
            FileInfo[] files = null;
            DirectoryInfo[] subDirs = null;

            files = directory.GetFiles("*.*");

            if (files != null)
            {
                foreach (FileInfo fi in files)
                {
                    var extension = fi.Extension.Replace(".", "-");
                    tree.Children.Add(new JsTreeDataModel
                    {
                        Text = fi.Name,
                        Id = fi.FullName,
                        Icon = "file file" + extension,
                        Type = "file"
                    });
                }                
            }

            subDirs = directory.GetDirectories();

            foreach (DirectoryInfo dirInfo in subDirs)
            {
                var subDirectory = new JsTreeDataModel
                {
                    Text = dirInfo.Name,
                    Id = dirInfo.FullName,
                    Opened = "false",
                    Type = "root"
                };

                if (!_excludeDirs.Contains(dirInfo.Name))
                {
                    tree.Children.Add(subDirectory);
                    // Resursive call for subdirectory.
                    WalkDirectoryTree(dirInfo, subDirectory);
                }                
            }
            
            return TreeList;
        }
    }
}
