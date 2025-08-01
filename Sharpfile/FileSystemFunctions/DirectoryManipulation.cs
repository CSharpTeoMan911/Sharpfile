﻿using Sharpfile.Shared;
using System.Collections;
using System.Text;


namespace Sharpfile.FileSystemFunctions
{
    internal class DirectoryManipulation:File_Sub_Operations
    {
        public static bool CopyDirectory(string source_path, string destination_path)
        {
            DirectoryContents source_directory_contents = new DirectoryContents();

            GetDirectoryContents(source_directory_contents, source_path);

            SetDirectoryContents(source_directory_contents, destination_path);

            return true;
        }

        

        private static bool GetDirectoryContents(DirectoryContents contents, string current_path)
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
                GetDirectoryContents(sub_directory_contents, (string)directories_enumerator.Current);
                contents.DirectoriesPaths.Add(sub_directory_contents);
            }

            return true;
        }

        private static bool SetDirectoryContents(DirectoryContents contents, string current_path)
        {
            Directory.CreateDirectory(current_path);

            StringBuilder path_builder = new StringBuilder();



            IEnumerator files_enumerator = contents.FilePaths.GetEnumerator();

            while (files_enumerator.MoveNext() == true)
            {
                FileInfo fileInfo = new FileInfo((string)files_enumerator.Current);

                path_builder.Append(current_path);
                path_builder.Append(Sub_Operations_Controller(Sub_Operations.Path_Separator_Generator, string.Empty));
                path_builder.Append(fileInfo.Name);

                File.Copy((string)files_enumerator.Current, path_builder.ToString());

                path_builder.Clear();
            }

            contents.FilePaths.Clear();


            IEnumerator directories_enumerator = contents.DirectoriesPaths.GetEnumerator();

            while (directories_enumerator.MoveNext() == true)
            {
                DirectoryContents inner_contents = (DirectoryContents)directories_enumerator.Current;

                DirectoryInfo directoryInfo = new DirectoryInfo(inner_contents.DirectoryPath);

                path_builder.Append(current_path);
                path_builder.Append(Sub_Operations_Controller(Sub_Operations.Path_Separator_Generator, string.Empty));
                path_builder.Append(directoryInfo.Name);

                SetDirectoryContents(inner_contents, path_builder.ToString());

                path_builder.Clear();
            }

            return true;
        }
    }
}
