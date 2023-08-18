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
            throw new NotImplementedException();
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
