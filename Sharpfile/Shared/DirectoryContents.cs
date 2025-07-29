using System.Collections.Concurrent;

namespace Sharpfile.Shared
{
    internal class DirectoryContents
    {
        public string DirectoryPath = string.Empty;
        public ConcurrentBag<string> FilePaths = new ConcurrentBag<string>();
        public ConcurrentBag<DirectoryContents> DirectoriesPaths = new ConcurrentBag<DirectoryContents>();
    }
}
