using System.IO;

namespace TDL.Test.Specs.Utils.Extensions
{
    public static class DirectoryExtension
    {
        public static void Empty(this DirectoryInfo directory)
        {
            foreach (var file in directory.GetFiles())
            {
                file.Delete();
            }

            foreach (var dir in directory.GetDirectories())
            {
                dir.Delete(true);
            }
        }
    }
}
