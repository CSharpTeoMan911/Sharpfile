using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sharpfile
{
    internal class DirectoryContents
    {
        public string DirectoryPath = String.Empty;
        public ConcurrentBag<string> FilePaths = new ConcurrentBag<string>();
        public ConcurrentBag<DirectoryContents> DirectoriesPaths = new ConcurrentBag<DirectoryContents>();
    }
}
