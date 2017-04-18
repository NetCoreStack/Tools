namespace NetCoreStack.Hisar.WebCli.Tools.Core
{
    public class CliEnvironment
    {
        public string AppDirectory { get; }
        public string DatabaseFullPath { get; }
        public string MainAppDirectoryWebRoot { get; }

        public CliEnvironment(string appDirectory, string databaseFullPath, string mainAppWebRoot = "")
        {
            AppDirectory = appDirectory;
            DatabaseFullPath = databaseFullPath;
            MainAppDirectoryWebRoot = mainAppWebRoot;
        }
    }
}
