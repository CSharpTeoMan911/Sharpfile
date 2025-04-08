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
    internal class Windows_Based_File_Operations:File_Sub_Operations, File_System_Operations
    {
        public bool Create_Directory(string directory_path)
        {
            try
            {
                if (System.IO.Directory.Exists(directory_path) == false)
                {
                    System.IO.Directory.CreateDirectory(directory_path);
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
            string? path = String.Empty;
            Program.Directories_Browser.TryPeek(out path);

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
                if (System.IO.Directory.Exists(directory_path) == true)
                {
                    System.IO.Directory.Delete(directory_path, true);
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
                if (System.IO.File.Exists(file_path) == true)
                {
                    System.IO.File.Delete(file_path);
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
                Program.current_directory.Clear();

                string? path = String.Empty;
                Program.Directories_Browser.TryPeek(out path);
                Program.current_directory_permissions = Null_Check((Sub_Operations_Controller(Sub_Operations.Get_File_Permissions, Null_Check(path)) as string));

                IEnumerable<string> contents = System.IO.Directory.EnumerateFileSystemEntries(Null_Check(path));
                IEnumerator<string> contents_enumerator = contents.GetEnumerator();


                while (contents_enumerator.MoveNext() == true)
                {
                    Tuple<string, string, string, ConsoleColor>? current_file = null;

                    ConsoleColor current_item_color = Program.Default_Console_Color;

                    string file_name = Null_Check((Sub_Operations_Controller(Sub_Operations.Get_File_Name, contents_enumerator.Current)) as string);

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

                    Program.current_directory.Add(current_file);
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
                    Program.Directories_Browser.Push(directory_path);
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
                if (Program.Directories_Browser.Count > 1)
                {
                    string? path = String.Empty;
                    Program.Directories_Browser.TryPop(out path);
                    Program.Directories_Browser.TryPop(out path);

                    if (path != null)
                    {
                        string? prev = null;
                        Program.Directories_Browser.TryPeek(out prev);

                        if (prev != null)
                        {
                            if (prev != path)
                                Program.Directories_Browser.Push(path);
                        }
                        else
                        {
                            Program.Directories_Browser.Push(path);
                        }
                    }

                    List_Files();
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
                System.IO.FileInfo file_info = new System.IO.FileInfo(file_name);

                if (file_info.Name.Length > 0)
                {
                    string? path = String.Empty;
                    Program.Directories_Browser.TryPeek(out path);

                    StringBuilder formated_file_name = new StringBuilder(file_info.Name);
                    formated_file_name.Remove(file_info.Name.Length - file_info.Extension.Length, file_info.Extension.Length);



                    for (int i = 0; i < Program.current_directory.Count; i++)
                    {
                        StringBuilder formated_current_directory_file_name = new StringBuilder(Null_Check(path));
                        formated_current_directory_file_name.Append('\\');
                        formated_current_directory_file_name.Append(Program.current_directory.ElementAt(i).Item2);

                        System.IO.FileInfo pre_formated_current_directory_file_name_file_info = new System.IO.FileInfo(formated_current_directory_file_name.ToString());
                        formated_current_directory_file_name.Clear();
                        formated_current_directory_file_name.Append(pre_formated_current_directory_file_name_file_info.Name);
                        formated_current_directory_file_name.Remove(pre_formated_current_directory_file_name_file_info.Name.Length - pre_formated_current_directory_file_name_file_info.Extension.Length, pre_formated_current_directory_file_name_file_info.Extension.Length);

                        if (Program.current_directory.ElementAt(Program.current_index).Item2 == file_info.Name)
                        {
                            formated_current_directory_file_name.Clear();
                            Program.current_index = i;
                            break;
                        }
                        else if (formated_current_directory_file_name.ToString() == formated_file_name.ToString())
                        {
                            formated_current_directory_file_name.Clear();
                            Program.current_index = i;
                            break;
                        }

                        formated_current_directory_file_name.Clear();
                    }

                    Program.Recalibrate_Indexes();

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
                string? current_path = String.Empty;
                Program.Directories_Browser.TryPeek(out current_path);

                if (current_path != null || current_path != String.Empty)
                {
                    switch (is_directory)
                    {
                        case true:
                            System.IO.Directory.Move(Null_Check(current_path), path);
                            break;

                        case false:
                            System.IO.File.Move(Null_Check(current_path), path);
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
                string formated_path = Null_Check(Sub_Operations_Controller(Sub_Operations.File_Path_Generation, Program.current_directory.ElementAt(Program.current_index).Item2) as string);
                if (File.Exists(path))
                    path = Null_Check((Sub_Operations_Controller(Sub_Operations.Random_File_Name_Generator, path) as string));
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
                string formated_path = Null_Check(Sub_Operations_Controller(Sub_Operations.File_Path_Generation, Program.current_directory.ElementAt(Program.current_index).Item2) as string);
                if (Directory.Exists(path))
                    path = Null_Check((Sub_Operations_Controller(Sub_Operations.Random_Directory_Name_Generator, path) as string));
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
                string? current_directory = String.Empty;
                Program.Directories_Browser.TryPeek(out current_directory);

                if (current_directory != null)
                {
                    string current_file = Program.current_directory.ElementAt(Program.current_index).Item2;

                    StringBuilder path_builder = new StringBuilder();
                    path_builder.Append(current_directory).Append(Sub_Operations_Controller(Sub_Operations.Path_Separator_Generator, String.Empty)).Append(current_file);


                    string formated_source_path = Path.GetFullPath(path_builder.ToString());

                    path_builder.Clear();
                    path_builder.Append(path).Append(Sub_Operations_Controller(Sub_Operations.Path_Separator_Generator, String.Empty)).Append(current_file);

                    string formated_destination_path = Null_Check((Sub_Operations_Controller(Sub_Operations.Random_File_Name_Generator, path_builder.ToString()) as string));
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
                string? current_directory = String.Empty;
                Program.Directories_Browser.TryPeek(out current_directory);

                StringBuilder path_builder = new StringBuilder(current_directory);
                path_builder.Append(Sub_Operations_Controller(Sub_Operations.Path_Separator_Generator, String.Empty));
                path_builder.Append(Program.current_directory.ElementAt(Program.current_index).Item2);

                string formated_source_path = Path.GetFullPath(path_builder.ToString());

                path_builder.Clear();
                path_builder.Append(path);
                path_builder.Append(Sub_Operations_Controller(Sub_Operations.Path_Separator_Generator, String.Empty));
                path_builder.Append(Program.current_directory.ElementAt(Program.current_index).Item2);

                string formated_destination_path = Null_Check((Sub_Operations_Controller(Sub_Operations.Random_File_Name_Generator, path_builder.ToString()) as string));

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
