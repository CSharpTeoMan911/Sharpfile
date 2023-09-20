using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sharpfile
{
    internal class DirectoryContents
    {
        public string DirectoryPath = String.Empty;
        public List<string> FilePaths = new List<string>();
        public List<DirectoryContents> DirectoriesPaths = new List<DirectoryContents>();
    }
}
