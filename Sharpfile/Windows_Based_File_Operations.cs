using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sharpfile
{
    internal class Windows_Based_File_Operations: File_System_Operations
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
            IEnumerable<string> contents = System.IO.Directory.EnumerateFileSystemEntries("C:\\Users\\Teodor Mihail\\source\\repos\\Sharpfile\\Sharpfile\\bin\\Release\\net6.0\\publish");
            IEnumerator<string> contents_enumerator = contents.GetEnumerator();


            while(contents_enumerator.MoveNext() == true)
            {
                ConsoleColor current_item_color = ConsoleColor.White;
                string extension_type = System.IO.Path.GetExtension(contents_enumerator.Current);

                switch(extension_type == String.Empty)
                {
                    case true:
                        current_item_color = ConsoleColor.Blue;
                        break;

                    case false:
                        if(extension_type == ".exe")
                        {
                            current_item_color = ConsoleColor.Yellow;
                        }
                        else
                        {
                            current_item_color = ConsoleColor.Green;
                        }
                        break;
                }

                GUI_Contents.Print_Current_Directory_Contents(System.IO.Path.GetFileName(contents_enumerator.Current), current_item_color);
            }

            if(contents_enumerator != null)
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
