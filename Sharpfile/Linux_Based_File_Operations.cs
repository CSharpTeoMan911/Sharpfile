using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sharpfile
{
    internal class Linux_Based_File_Operations : File_System_Operations
    {
        public async Task<bool> Create_Directory(string directory_path)
        {
            throw new NotImplementedException();
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
            IEnumerable<string> contents = System.IO.Directory.EnumerateFileSystemEntries(Program.Current_Directory);
            IEnumerator<string> contents_enumerator = contents.GetEnumerator();


            while (contents_enumerator.MoveNext() == true)
            {
                ConsoleColor current_item_color = Program.Default_Console_Color;
                string file_name = System.IO.Path.GetFileName(contents_enumerator.Current);

                // SET PROCESS 
                System.Diagnostics.ProcessStartInfo shell_process_start_info = new System.Diagnostics.ProcessStartInfo("/bin/bash", "-c \"ls -all " + file_name + "\"");
                shell_process_start_info.RedirectStandardError = true;
                shell_process_start_info.RedirectStandardInput = true;
                shell_process_start_info.RedirectStandardOutput = true;

                System.Diagnostics.Process shell_process = new System.Diagnostics.Process();
                shell_process.StartInfo = shell_process_start_info;
                shell_process.Start();



                int space_index = 0;
                StringBuilder result = new StringBuilder(await shell_process.StandardOutput.ReadLineAsync());

                for (int i = 0; i < result.Length; i++)
                {
                    if (result[i] == ' ')
                    {
                        space_index = i;
                        break;
                    }
                }


                result.Remove(space_index, (result.Length - space_index));
                string file_permissions = result.ToString();

                string extension_type = System.IO.Path.GetExtension(contents_enumerator.Current);

                switch (extension_type == String.Empty)
                {
                    case true:

                        switch (file_permissions.Contains('d'))
                        {
                            case true:
                                current_item_color = ConsoleColor.Blue;
                                break;

                            case false:
                                switch (file_permissions.Contains('x'))
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
                        break;

                    case false:
                        current_item_color = ConsoleColor.Green;
                        break;
                }

                GUI_Contents.Print_Current_Directory_Contents(file_name, current_item_color);
            }

            if (contents_enumerator != null)
            {
                contents_enumerator.Dispose();
            }

            return true;
        }

        public async Task<bool> Navigate_To_Next_Directory()
        {
            throw new NotImplementedException();
        }

        public async Task<bool> Navigate_To_Previos_Directory()
        {
            throw new NotImplementedException();
        }

        public async Task<bool> Open_File(string file_path)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> Search_File(string file_name)
        {
            throw new NotImplementedException();
        }
    }
}
