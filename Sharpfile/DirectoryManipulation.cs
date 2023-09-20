using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sharpfile
{
    internal class DirectoryManipulation
    {
        public static async void CopyDirectory(string source_path, string destination_path)
        {
            DirectoryContents source_directory_contents = new DirectoryContents();
            await GetDirectoryContents(source_directory_contents, destination_path);

            /////////////////////////////////////////
            ////  ==============>
            ////////////////////////////////////////
        }

        private static async Task<bool> GetDirectoryContents(DirectoryContents contents, string current_path)
        {
            contents.DirectoryPath = current_path;

            IEnumerator files_enumerator = Directory.GetFiles(contents.DirectoryPath).GetEnumerator();

            while(files_enumerator.MoveNext() == true)
            {
                contents.FilePaths.Add((string)files_enumerator.Current);
            }

            IEnumerator directories_enumerator = Directory.GetDirectories(contents.DirectoryPath).GetEnumerator();

            while(directories_enumerator.MoveNext() == true)
            {
                DirectoryContents sub_directory_contents = new DirectoryContents();
                await GetDirectoryContents(sub_directory_contents, (string)directories_enumerator.Current);
                contents.DirectoriesPaths.Add(sub_directory_contents);
            }

            return true;
        }
    }
}
