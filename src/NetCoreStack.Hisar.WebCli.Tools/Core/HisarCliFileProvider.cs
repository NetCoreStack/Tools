// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.FileProviders.Embedded;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace NetCoreStack.Hisar.WebCli.Tools.Core
{
    public class HisarCliFileProvider : IFileProvider
    {
        private readonly string _baseNamespace;
        private readonly DateTimeOffset _lastModified;
        private readonly Assembly _assembly;
        private static readonly char[] _invalidFileNameChars = Path.GetInvalidFileNameChars()
            .Where(c => c != '/' && c != '\\').ToArray();

        private static readonly IDictionary<string, IFileInfo> _resolvedFileInfoDict = new Dictionary<string, IFileInfo>();

        public HisarCliFileProvider(Assembly assembly)
        {
            _assembly = assembly ?? throw new ArgumentNullException(nameof(assembly));
            _baseNamespace = HostingConstants.PackageName + ".wwwroot.";
            _lastModified = DateTimeOffset.UtcNow;
        }

        private static bool HasInvalidPathChars(string path)
        {
            return path.IndexOfAny(_invalidFileNameChars) != -1;
        }

        public IDirectoryContents GetDirectoryContents(string subpath)
        {
            // The file name is assumed to be the remainder of the resource name.
            if (subpath == null)
            {
                return NotFoundDirectoryContents.Singleton;
            }

            // Relative paths starting with a leading slash okay
            if (subpath.StartsWith("/", StringComparison.Ordinal))
            {
                subpath = subpath.Substring(1);
            }

            // Non-hierarchal.
            if (!subpath.Equals(string.Empty))
            {
                return NotFoundDirectoryContents.Singleton;
            }

            var entries = new List<IFileInfo>();
            
            var resources = _assembly.GetManifestResourceNames();
            for (var i = 0; i < resources.Length; i++)
            {
                var resourceName = resources[i];
                if (resourceName.StartsWith(_baseNamespace))
                {
                    entries.Add(new EmbeddedResourceFileInfo(
                        _assembly,
                        resourceName,
                        resourceName.Substring(_baseNamespace.Length),
                        _lastModified));
                }
            }

            return new EnumerableDirectoryContents(entries);
        }

        public IFileInfo GetFileInfo(string subpath)
        {
            if (string.IsNullOrEmpty(subpath))
            {
                return new NotFoundFileInfo(subpath);
            }

            IFileInfo fileInfo = null;
            if (_resolvedFileInfoDict.TryGetValue(subpath, out fileInfo))
            {
                return fileInfo;
            }

            var resolvedPath = subpath;
            var name = Path.GetFileName(subpath);
            var directory = Path.GetDirectoryName(subpath);

            if (directory != "\\")
            {
                // embed directory dash - underscore checker;
                var directoryBuilder = new StringBuilder(directory);
                for (int i = 0; i < directoryBuilder.Length; i++)
                {
                    if (directoryBuilder[i] == '-')
                    {
                        directoryBuilder[i] = '_';
                    }
                }

                resolvedPath = directoryBuilder.ToString().Replace("\\", "/") + "/" + name;
            }
            
            var builder = new StringBuilder(_baseNamespace.Length + resolvedPath.Length);
            builder.Append(_baseNamespace);

            // Relative paths starting with a leading slash okay
            if (resolvedPath.StartsWith("/", StringComparison.Ordinal))
            {
                builder.Append(resolvedPath, 1, resolvedPath.Length - 1);
            }
            else
            {
                builder.Append(resolvedPath);
            }

            for (var i = _baseNamespace.Length; i < builder.Length; i++)
            {
                if (builder[i] == '/' || builder[i] == '\\')
                {
                    builder[i] = '.';
                }
            }

            var resourcePath = builder.ToString();
            if (HasInvalidPathChars(resourcePath))
            {
                _resolvedFileInfoDict.Add(subpath, new NotFoundFileInfo(resourcePath));
                return new NotFoundFileInfo(resourcePath);
            }

            if (_assembly.GetManifestResourceInfo(resourcePath) == null)
            {
                _resolvedFileInfoDict.Add(subpath, new NotFoundFileInfo(name));
                return new NotFoundFileInfo(name);
            }

            fileInfo = new EmbeddedResourceFileInfo(_assembly, resourcePath, name, _lastModified);
            _resolvedFileInfoDict.Add(subpath, fileInfo);
            return fileInfo;
        }

        public IChangeToken Watch(string filter)
        {
            return NullChangeToken.Singleton;
        }
    }
}
