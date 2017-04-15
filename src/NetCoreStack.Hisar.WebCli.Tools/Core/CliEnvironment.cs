namespace NetCoreStack.Hisar.WebCli.Tools.Core
{
    public class CliEnvironment
    {
        public string AppDirectory { get; }
        public string DatabaseFullPath { get; }

        public CliEnvironment(string appDirectory, string databaseFullPath)
        {
            AppDirectory = appDirectory;
            DatabaseFullPath = databaseFullPath;
        }
    }
}
