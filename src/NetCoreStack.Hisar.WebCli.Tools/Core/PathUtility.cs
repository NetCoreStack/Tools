﻿using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace NetCoreStack.Hisar.WebCli.Tools.Core
{
    public static class PathUtility
    {
        private const string ContentFolderName = "content";
        private static string _layoutPagePath = string.Empty;

        public static string GetWebRootPath()
        {
            var assemblyPath = Assembly.GetEntryAssembly().Location;
            var directory = Path.GetDirectoryName(assemblyPath);
            var directoryInfo = new DirectoryInfo(directory);
            do
            {
                var path = Path.Combine(directoryInfo.FullName, "content", "wwwroot");
                var webRoot = new DirectoryInfo(path);
                if (webRoot.Exists)
                {
                    return webRoot.FullName;
                }

                directoryInfo = directoryInfo.Parent;
            }
            while (directoryInfo.Parent != null);

            var message = $"Ex: wwwroot could not be located using package root {assemblyPath}";
            throw new Exception(message);
        }

        public static string GetLayoutPagePath(string directory)
        {
            if (string.IsNullOrEmpty(_layoutPagePath))
            {
                var files = Directory.GetFiles(directory, HostingConstants.LayoutPageName, SearchOption.AllDirectories);
                if (files.Any())
                {
                    _layoutPagePath = files.FirstOrDefault();
                    return _layoutPagePath;
                }

                var message = $"Ex: {HostingConstants.LayoutPageName} could not be located in {directory}";
                throw new Exception(message);
            }

            return _layoutPagePath;
        }
    }
}
