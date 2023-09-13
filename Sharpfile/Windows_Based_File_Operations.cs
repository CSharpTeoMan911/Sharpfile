﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Sharpfile.File_Sub_Operations;

namespace Sharpfile
{
    internal class Windows_Based_File_Operations:File_Sub_Operations, File_System_Operations
    {
        public Task<bool> Create_Directory(string directory_path)
        {
            bool result = false;

            if(System.IO.Directory.Exists(directory_path) == false)
            {
                System.IO.Directory.CreateDirectory(directory_path);
            }

            return Task.FromResult(result);
        }

        public async Task<bool> Create_File(string file_path)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> Delete_Directory(string directory_path)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> Delete_File(string file_path)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> List_Files()
        {
            Program.current_directory.Clear();

            IEnumerable<string> contents = System.IO.Directory.EnumerateFileSystemEntries(Program.Current_Directory);
            IEnumerator<string> contents_enumerator = contents.GetEnumerator();


            while (contents_enumerator.MoveNext() == true)
            {
                Tuple<string, string, string, ConsoleColor> current_file = null;

                ConsoleColor current_item_color = Program.Default_Console_Color;

                string file_name = await Sub_Operations_Controller(Sub_Operations.Get_File_Name, contents_enumerator.Current);

                string extension_type = await Sub_Operations_Controller(Sub_Operations.Get_File_Extension, contents_enumerator.Current);

                string file_permissions = await Sub_Operations_Controller(Sub_Operations.Get_File_Permissions, contents_enumerator.Current);


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
                    Program.Previous_Directory = Program.Current_Directory;
                    Program.Current_Directory = directory_path;
                    result = await List_Files();
                }
            }
            catch { }
            return result;
        }

        public async Task<bool> Navigate_To_Previos_Directory()
        {
            bool result = false;
            string current_item_path = Program.Previous_Directory;
            result = await Navigate_To_Directory(current_item_path);
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
            throw new NotImplementedException();
        }
    }
}
