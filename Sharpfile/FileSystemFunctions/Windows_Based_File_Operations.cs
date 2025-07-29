using System.Diagnostics;
using System.Text;

namespace Sharpfile.FileSystemFunctions
{
    internal class Windows_Based_File_Operations:File_Sub_Operations, File_System_Operations
    {
        public bool Create_Directory(string directory_path)
        {
            try
            {
                if (Directory.Exists(directory_path) == false)
                {
                    Directory.CreateDirectory(directory_path);
                }

                return true;
            }
            catch 
            {
                return false;
            }
        }

        public bool Open_Current_Directory_In_Terminal()
        {
            string? path = string.Empty;            
            Directories_Browser.TryPeek(out path);

            Process p = new Process();
            try
            {
                p.StartInfo.UseShellExecute = true;
                p.StartInfo.FileName = "cmd";
                p.StartInfo.Arguments = "/k \"cd \"" + Null_Check(path) + "\"\"";
                p.Start();
            }
            catch 
            {
                p?.Dispose();
                return false;
            }

            return true;
        }

        public bool Delete_Directory(string directory_path)
        {
            try
            {
                if (Directory.Exists(directory_path) == true)
                {
                    Directory.Delete(directory_path, true);
                }

                return true;
            }
            catch 
            {
                return false;
            }
        }

        public bool Delete_File(string file_path)
        {
            try
            {
                if (File.Exists(file_path) == true)
                {
                    File.Delete(file_path);
                }

                return true;
            }
            catch 
            {
                return false;
            }
        }

        public bool List_Files()
        {
            try
            {
                current_directory.Clear();

                string? path = string.Empty;
                Directories_Browser.TryPeek(out path);
                current_directory_permissions = Null_Check(Sub_Operations_Controller(Sub_Operations.Get_File_Permissions, Null_Check(path)) as string);

                IEnumerable<string> contents = Directory.EnumerateFileSystemEntries(Null_Check(path));
                IEnumerator<string> contents_enumerator = contents.GetEnumerator();


                while (contents_enumerator.MoveNext() == true)
                {
                    Tuple<string, string, string, ConsoleColor>? current_file = null;

                    ConsoleColor current_item_color = Default_Console_Color;

                    string file_name = Null_Check(Sub_Operations_Controller(Sub_Operations.Get_File_Name, contents_enumerator.Current) as string);

                    string extension_type = Null_Check(Sub_Operations_Controller(Sub_Operations.Get_File_Extension, contents_enumerator.Current) as string);

                    string file_permissions = Null_Check(Sub_Operations_Controller(Sub_Operations.Get_File_Permissions, contents_enumerator.Current) as string);


                    switch (extension_type)
                    {
                        case "dir":
                            current_item_color = ConsoleColor.Blue;
                            break;
                        case "bin":
                            current_item_color = ConsoleColor.Yellow;
                            break;
                        default:
                            current_item_color = ConsoleColor.Green;
                            break;
                    }

                    current_file = new Tuple<string, string, string, ConsoleColor>(file_permissions, file_name, extension_type, current_item_color);

                    current_directory.Add(current_file);
                }

                contents_enumerator?.Dispose();

                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool Navigate_To_Directory(string directory_path)
        {
            try
            {
                if(Directory.Exists(directory_path) == true)
                {
                    Directories_Browser.Push(directory_path);
                    List_Files();
                }

                return true;
            }
            catch 
            {
                return false;
            }
        }

        public bool Navigate_To_Previos_Directory()
        {
            try
            {
                if (Directories_Browser.Count > 1)
                {
                    if (Directories_Browser.TryPop(out _))
                    {
                        List_Files();
                    }
                }

                return true;
            }
            catch 
            {
                return false;
            }
        }

        public bool Open_File(string file_path)
        {
            bool result = false;
            Process fileopener = new Process();

            try
            {
                fileopener.StartInfo.FileName = "explorer";
                fileopener.StartInfo.Arguments = "\"" + file_path + "\"";
                fileopener.Start();
            }
            catch 
            { 
            }
            finally
            {
                if(fileopener != null)
                fileopener.Dispose();
            }

            return result;
        }

        public bool Search_File(string file_name)
        {
            try
            {
                FileInfo file_info = new FileInfo(file_name);

                if (file_info.Name.Length > 0)
                {
                    string? path = string.Empty;
                    Directories_Browser.TryPeek(out path);

                    StringBuilder formated_file_name = new StringBuilder(file_info.Name);
                    formated_file_name.Remove(file_info.Name.Length - file_info.Extension.Length, file_info.Extension.Length);



                    for (int i = 0; i < current_directory.Count; i++)
                    {
                        StringBuilder formated_current_directory_file_name = new StringBuilder(Null_Check(path));
                        formated_current_directory_file_name.Append('\\');
                        formated_current_directory_file_name.Append(current_directory.ElementAt(i).Item2);

                        FileInfo pre_formated_current_directory_file_name_file_info = new FileInfo(formated_current_directory_file_name.ToString());
                        formated_current_directory_file_name.Clear();
                        formated_current_directory_file_name.Append(pre_formated_current_directory_file_name_file_info.Name);
                        formated_current_directory_file_name.Remove(pre_formated_current_directory_file_name_file_info.Name.Length - pre_formated_current_directory_file_name_file_info.Extension.Length, pre_formated_current_directory_file_name_file_info.Extension.Length);

                        if (current_directory.ElementAt(current_index).Item2 == file_info.Name)
                        {
                            formated_current_directory_file_name.Clear();
                            current_index = i;
                            break;
                        }
                        else if (formated_current_directory_file_name.ToString() == formated_file_name.ToString())
                        {
                            formated_current_directory_file_name.Clear();
                            current_index = i;
                            break;
                        }

                        formated_current_directory_file_name.Clear();
                    }

                    Recalibrate_Indexes();

                    formated_file_name.Clear();
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool Move_Or_Rename_File(string path, bool is_directory)
        {
            try
            {
                string? current_path = string.Empty;
                Directories_Browser.TryPeek(out current_path);

                if (current_path != null || current_path != string.Empty)
                {
                    switch (is_directory)
                    {
                        case true:
                            Directory.Move(Null_Check(current_path), path);
                            break;

                        case false:
                            File.Move(Null_Check(current_path), path);
                            break;
                    }
                }

                return true;
            }
            catch 
            {
                return false;
            }
        }

        public bool Move_Or_Rename_File(string path)
        {
            try
            {
                string formated_path = Null_Check(Sub_Operations_Controller(Sub_Operations.File_Path_Generation, current_directory.ElementAt(current_index).Item2) as string);
                if (File.Exists(path))
                    path = Null_Check(Sub_Operations_Controller(Sub_Operations.Random_File_Name_Generator, path) as string);
                File.Move(formated_path, path);
                return true;
            }
            catch 
            {
                return false;
            }
        }

        public bool Move_Or_Rename_Directory(string path)
        {
            try
            {
                string formated_path = Null_Check(Sub_Operations_Controller(Sub_Operations.File_Path_Generation, current_directory.ElementAt(current_index).Item2) as string);
                if (Directory.Exists(path))
                    path = Null_Check(Sub_Operations_Controller(Sub_Operations.Random_Directory_Name_Generator, path) as string);
                Directory.Move(formated_path, path);
                return true;
            }
            catch 
            {
                return false;
            }
        }

        public bool Copy_File(string path)
        {
            try
            {
                string? selected_current_directory = string.Empty;
                Directories_Browser.TryPeek(out selected_current_directory);

                if (current_directory != null)
                {
                    string current_file = current_directory.ElementAt(current_index).Item2;

                    StringBuilder path_builder = new StringBuilder();
                    path_builder.Append(selected_current_directory)
                        .Append(Sub_Operations_Controller(Sub_Operations.Path_Separator_Generator, string.Empty))
                        .Append(current_file);


                    string formated_source_path = Path.GetFullPath(path_builder.ToString());

                    path_builder.Clear();
                    path_builder.Append(path).Append(Sub_Operations_Controller(Sub_Operations.Path_Separator_Generator, string.Empty)).Append(current_file);

                    string formated_destination_path = Null_Check(Sub_Operations_Controller(Sub_Operations.Random_File_Name_Generator, path_builder.ToString()) as string);
                    
                    File.Copy(formated_source_path, formated_destination_path);
                    path_builder.Clear();
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool Copy_Directory(string path)
        {
            try
            {
                string? selected_current_directory = String.Empty;
                Directories_Browser.TryPeek(out selected_current_directory);

                StringBuilder path_builder = new StringBuilder(selected_current_directory);
                path_builder.Append(Sub_Operations_Controller(Sub_Operations.Path_Separator_Generator, string.Empty));
                path_builder.Append(current_directory.ElementAt(current_index).Item2);

                string formated_source_path = Path.GetFullPath(path_builder.ToString());

                path_builder.Clear();
                path_builder.Append(path);
                path_builder.Append(Sub_Operations_Controller(Sub_Operations.Path_Separator_Generator, string.Empty));
                path_builder.Append(current_directory.ElementAt(current_index).Item2);

                string formated_destination_path = Null_Check(Sub_Operations_Controller(Sub_Operations.Random_Directory_Name_Generator, path_builder.ToString()) as string);

                DirectoryManipulation.CopyDirectory(formated_source_path, formated_destination_path);

                path_builder.Clear();

                return true;
            }
            catch 
            {
                return false;
            }
        }
    }
}
