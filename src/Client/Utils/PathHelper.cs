using System.IO;

namespace TDL.Client.Utils
{
    public static class PathHelper
    {
        private const string SolutionFileSearchPattern = "*.sln";

        public static string RepositoryPath { get; }

        static PathHelper()
        {
            var exeDirectoryPath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) ?? string.Empty;
            RepositoryPath = GetRepositoryPath(exeDirectoryPath) ?? exeDirectoryPath;
        }

        private static string GetRepositoryPath(string path)
        {
            var currentDirectory = new DirectoryInfo(path);
            do
            {
                if (currentDirectory.GetFiles(SolutionFileSearchPattern).Length == 1)
                {
                    return currentDirectory.FullName;
                }

                currentDirectory = currentDirectory.Parent;
            }
            while (currentDirectory != null);

            // If no valid repository path is found, return an empty string or handle appropriately.
            return string.Empty;
        }
    }
}
