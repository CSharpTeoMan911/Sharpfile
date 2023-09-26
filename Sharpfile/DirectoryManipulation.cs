using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Sharpfile
{
    internal class DirectoryManipulation:File_Sub_Operations
    {
        public static async void CopyDirectory(string source_path, string destination_path)
        {
            DirectoryContents source_directory_contents = new DirectoryContents();
            await GetDirectoryContents(source_directory_contents, source_path);

            DirectoryInfo directoryInfo = new DirectoryInfo(source_directory_contents.DirectoryPath);

            StringBuilder path_builder = new StringBuilder(destination_path);
            path_builder.Append(await Sub_Operations_Controller(Sub_Operations.Path_Separator_Generator, String.Empty));
            path_builder.Append(directoryInfo.Name);

            Stopwatch s = new Stopwatch();

            s.Start();
            await SetDirectoryContents(source_directory_contents, Null_Check((await Sub_Operations_Controller(Sub_Operations.Random_Directory_Name_Generator, path_builder.ToString()) as string)));
            s.Stop();

            path_builder.Clear();
            System.Diagnostics.Debug.WriteLine("\n\n!!! FINISHED !!!");
            System.Diagnostics.Debug.WriteLine("Elapsed milliseconds: " + s.ElapsedMilliseconds);
            System.Diagnostics.Debug.WriteLine("\n\n!!! FINISHED !!!");
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

        private static async Task<bool> SetDirectoryContents(DirectoryContents contents, string current_path)
        {
            Directory.CreateDirectory(current_path);

            StringBuilder path_builder = new StringBuilder();



            IEnumerator files_enumerator = contents.FilePaths.GetEnumerator();

            while (files_enumerator.MoveNext() == true)
            {
                FileInfo fileInfo = new FileInfo((string)files_enumerator.Current);

                path_builder.Append(current_path);
                path_builder.Append(await Sub_Operations_Controller(Sub_Operations.Path_Separator_Generator, String.Empty));
                path_builder.Append(fileInfo.Name);

                File.Copy((string)files_enumerator.Current, path_builder.ToString());

                path_builder.Clear();
            }

            contents.FilePaths.Clear();


            IEnumerator directories_enumerator = contents.DirectoriesPaths.GetEnumerator();

            while (directories_enumerator.MoveNext() == true)
            {
                DirectoryContents inner_contents = (DirectoryContents)(directories_enumerator.Current);

                DirectoryInfo directoryInfo = new DirectoryInfo(inner_contents.DirectoryPath);

                path_builder.Append(current_path);
                path_builder.Append(await Sub_Operations_Controller(Sub_Operations.Path_Separator_Generator, String.Empty));
                path_builder.Append(directoryInfo.Name);

                await SetDirectoryContents(inner_contents, path_builder.ToString());

                path_builder.Clear();
            }

            return true;
        }
    }
}
