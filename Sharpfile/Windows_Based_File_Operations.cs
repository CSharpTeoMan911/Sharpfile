﻿using System;
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
        public Task<bool> Create_Directory(string directory_path)
        {
            bool result = false;

            try
            {
                if (System.IO.Directory.Exists(directory_path) == false)
                {
                    System.IO.Directory.CreateDirectory(directory_path);
                }
            }
            catch { }

            return Task.FromResult(result);
        }

        public Task<bool> Open_Current_Directory_In_Terminal()
        {
            string path = String.Empty;
            Program.Directories_Browser.TryPeek(out path);

            Process p = new Process();

            try
            {
                p.StartInfo.UseShellExecute = true;
                p.StartInfo.FileName = "cmd";
                p.StartInfo.Arguments = "/k \"cd \"" + path + "\"\"";
                p.Start();
            }
            catch 
            {
                if(p != null)
                {
                    p.Dispose();
                }
            }

            return Task.FromResult(true);
        }

        public Task<bool> Delete_Directory(string directory_path)
        {
            bool result = false;

            try
            {
                if (System.IO.Directory.Exists(directory_path) == true)
                {
                    System.IO.Directory.Delete(directory_path, true);
                }
            }
            catch { }

            return Task.FromResult(result);
        }

        public Task<bool> Delete_File(string file_path)
        {
            bool result = false;

            try
            {
                if (System.IO.File.Exists(file_path) == true)
                {
                    System.IO.File.Delete(file_path);
                }
            }
            catch { }

            return Task.FromResult(result);
        }

        public async Task<bool> List_Files()
        {
            Program.current_directory.Clear();

            string path = String.Empty;
            Program.Directories_Browser.TryPeek(out path);
            Program.current_directory_permissions = Null_Check((await Sub_Operations_Controller(Sub_Operations.Get_File_Permissions, path) as string));

            IEnumerable<string> contents = System.IO.Directory.EnumerateFileSystemEntries(path);
            IEnumerator<string> contents_enumerator = contents.GetEnumerator();


            while (contents_enumerator.MoveNext() == true)
            {
                Tuple<string, string, string, ConsoleColor> current_file = null;

                ConsoleColor current_item_color = Program.Default_Console_Color;

                string file_name = Null_Check((await Sub_Operations_Controller(Sub_Operations.Get_File_Name, contents_enumerator.Current)) as string);

                string extension_type = Null_Check(await Sub_Operations_Controller(Sub_Operations.Get_File_Extension, contents_enumerator.Current) as string);

                string file_permissions = Null_Check(await Sub_Operations_Controller(Sub_Operations.Get_File_Permissions, contents_enumerator.Current) as string);


                switch (System.IO.Directory.Exists(contents_enumerator.Current))
                {
                    case true:
                        current_item_color = ConsoleColor.Blue;
                        break;

                    case false:
                        switch (extension_type == ".exe")
                        {
                            case true:
                                current_item_color = ConsoleColor.Yellow;
                                break;

                            case false:
                                current_item_color = ConsoleColor.Green;
                                break;
                        }
                        break;
                }

                current_file = new Tuple<string, string, string, ConsoleColor>(file_permissions, file_name, extension_type, current_item_color);
                
                Program.current_directory.Add(current_file);
            }

            if(contents_enumerator != null)
            {
                contents_enumerator.Dispose();
            }

            return true;
        }

        public async Task<bool> Navigate_To_Directory(string directory_path)
        {
            bool result = false;
            try
            {
                if(Directory.Exists(directory_path) == true)
                {
                    Program.Directories_Browser.Push(directory_path);
                    result = await List_Files();
                }
            }
            catch { }
            return result;
        }

        public async Task<bool> Navigate_To_Previos_Directory()
        {
            bool result = false;

            if(Program.Directories_Browser.Count > 1)
            {
                string? path = String.Empty;
                Program.Directories_Browser.TryPop(out path);

                Program.Directories_Browser.TryPeek(out path);

                result = await List_Files();
            }

            return result;
        }

        public Task<bool> Open_File(string file_path)
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

            return Task.FromResult(result);
        }

        public async Task<bool> Search_File(string file_name)
        {
            bool result = true;
            System.IO.FileInfo file_info = new System.IO.FileInfo(file_name);

            if(file_info.Name.Length > 0)
            {
                int found_index = -1;

                string path = String.Empty;
                Program.Directories_Browser.TryPeek(out path);

                StringBuilder formated_file_name = new StringBuilder(file_info.Name);
                formated_file_name.Remove(file_info.Name.Length - file_info.Extension.Length, file_info.Extension.Length);



                for (int i = 0; i < Program.current_directory.Count; i++)
                {
                    StringBuilder formated_current_directory_file_name = new StringBuilder(path);
                    formated_current_directory_file_name.Append("\\");
                    formated_current_directory_file_name.Append(Program.current_directory.ElementAt(i).Item2);

                    System.IO.FileInfo pre_formated_current_directory_file_name_file_info = new System.IO.FileInfo(formated_current_directory_file_name.ToString());
                    formated_current_directory_file_name.Clear();
                    formated_current_directory_file_name.Append(pre_formated_current_directory_file_name_file_info.Name);
                    formated_current_directory_file_name.Remove(pre_formated_current_directory_file_name_file_info.Name.Length - pre_formated_current_directory_file_name_file_info.Extension.Length, pre_formated_current_directory_file_name_file_info.Extension.Length);

                    if (Program.current_directory.ElementAt(Program.current_index).Item2 == file_info.Name)
                    {
                        formated_current_directory_file_name.Clear();
                        found_index = i;
                        break;
                    }
                    else if (formated_current_directory_file_name.ToString() == formated_file_name.ToString())
                    {
                        formated_current_directory_file_name.Clear();
                        found_index = i;
                        break;
                    }

                    formated_current_directory_file_name.Clear();
                }

                Program.current_index = 0;
                Program.start_index = 0;
                Program.cursor_location = 0;


                if (found_index >= 0)
                {
                    while (Program.current_index != found_index)
                    {
                        Program.current_index++;
                        Program.cursor_location++;
                        await Program.Cursor_Position_Calculator();
                    }
                }

                formated_file_name.Clear();
            }

            return result;
        }

        public Task<bool> Move_Or_Rename_File(string path, bool is_directory)
        {
            string current_path = String.Empty;
            Program.Directories_Browser.TryPeek(out current_path);

            if (current_path != null || current_path != String.Empty)
            {
                switch (is_directory)
                {
                    case true:
                        System.IO.Directory.Move(current_path, path);
                        break;

                    case false:
                        System.IO.File.Move(current_path, path);
                        break;
                }
            }

            return Task.FromResult(true);
        }

        public async Task<bool> Move_Or_Rename_File(string path)
        {
            bool result = false;
            string formated_path = Null_Check(await Sub_Operations_Controller(Sub_Operations.File_Path_Generation, Program.current_directory.ElementAt(Program.current_index).Item2) as string);

            File.Move(formated_path, path);
            return result;
        }

        public async Task<bool> Move_Or_Rename_Directory(string path)
        {
            bool result = false;
            string formated_path = Null_Check(await Sub_Operations_Controller(Sub_Operations.File_Path_Generation, Program.current_directory.ElementAt(Program.current_index).Item2) as string);

            Directory.Move(formated_path, path);
            return result;
        }

        public Task<bool> Copy_File(string path)
        {
            throw new NotImplementedException();
        }

        public Task<bool> Copy_Directory(string path)
        {
            throw new NotImplementedException();
        }
    }
}
