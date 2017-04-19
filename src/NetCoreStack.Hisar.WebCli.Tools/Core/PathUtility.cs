using NetCoreStack.Hisar.WebCli.Tools.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace NetCoreStack.Hisar.WebCli.Tools.Core
{
    public static class PathUtility
    {
        private const string ContentFolderName = "content";
        private static string _layoutPagePath = string.Empty;

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

        public static string GetAppDirectoryWebRoot(string directory)
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
            if (string.IsNullOrEmpty(_layoutPagePath))
            {
                var name = Path.GetFileName(HostingConstants.LayoutPageFullName);
                var files = Directory.GetFiles(directory, name, SearchOption.AllDirectories);
                if (files.Any())
                {
                    _layoutPagePath = files.FirstOrDefault();
                    return _layoutPagePath;
                }

                var message = $"Ex: {HostingConstants.LayoutPageFullName} could not be located in {directory}";
                throw new Exception(message);
            }

            return _layoutPagePath;
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
