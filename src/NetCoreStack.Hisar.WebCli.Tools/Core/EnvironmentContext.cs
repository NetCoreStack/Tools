using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.PlatformAbstractions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace NetCoreStack.Hisar.WebCli.Tools.Core
{
    public class EnvironmentContext
    {
        protected IConfigurationRoot Configuration { get; }
        protected IHostingEnvironment HostingEnvironment { get; }

        protected CliEnvironment CliEnvironment { get; }
        public ComponentDefinition ComponentInfo { get; }
        public DateTime ContextDateTime { get; }
        public string EnvironmentName { get; }
        public string ContentRootPath { get; }
        public string WebRootPath { get; }
        public string ApplicationBasePath { get; }
        public string AppDirectory { get; }
        public string MainAppDirectoryWebRoot { get; }
        public string DatabasePath { get; }
        public string Version { get; }
        public string AssemblyPath { get; }
        public string ExecutionPath { get; }

        public EnvironmentContext(IConfigurationRoot configuration, 
            IHostingEnvironment env, 
            CliEnvironment cliEnv, 
            ComponentDefinition componentInfo)
        {
            HostingEnvironment = env;
            CliEnvironment = cliEnv;
            Configuration = configuration;
            ComponentInfo = componentInfo;
            ContextDateTime = DateTime.Now;
            EnvironmentName = HostingEnvironment.EnvironmentName;
            ContentRootPath = HostingEnvironment.ContentRootPath;
            WebRootPath = HostingEnvironment.WebRootPath;
            ApplicationBasePath = PlatformServices.Default.Application.ApplicationBasePath;
            AppDirectory = cliEnv.AppDirectory;
            DatabasePath = cliEnv.DatabaseFullPath;
            Version = PlatformServices.Default.Application.ApplicationVersion;
            AssemblyPath = Assembly.GetEntryAssembly().Location;
            MainAppDirectoryWebRoot = cliEnv.MainAppDirectoryWebRoot;
            ExecutionPath = Directory.GetCurrentDirectory();
        }

        public IDictionary<string, object> ToJson()
        {
            Dictionary<string, object> dictionary = new Dictionary<string, object>();
            dictionary.Add(nameof(EnvironmentName), EnvironmentName);

            // Windows supports forward slashes to easy copy paste and check the path
            dictionary.Add(nameof(ContentRootPath), ContentRootPath.Replace("\\", "/"));
            dictionary.Add(nameof(WebRootPath), WebRootPath.Replace("\\", "/"));
            dictionary.Add(nameof(ApplicationBasePath), ApplicationBasePath.Replace("\\", "/"));
            dictionary.Add(nameof(AppDirectory), AppDirectory.Replace("\\", "/"));
            dictionary.Add(nameof(DatabasePath), DatabasePath.Replace("\\", "/"));
            dictionary.Add(nameof(MainAppDirectoryWebRoot), MainAppDirectoryWebRoot.Replace("\\", "/"));
            dictionary.Add(nameof(ExecutionPath), ExecutionPath.Replace("\\", "/"));
            dictionary.Add(nameof(Version), Version);
            dictionary.Add(nameof(AssemblyPath), AssemblyPath.Replace("\\", "/"));
            dictionary.Add(nameof(ContextDateTime), ContextDateTime);
            dictionary.Add(nameof(ComponentDefinition.ComponentId), ComponentInfo?.ComponentId);

            return dictionary;
        }
    }
}
