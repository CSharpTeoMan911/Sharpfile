using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sharpfile
{
    internal interface File_System_Operations
    {
        public Task<bool> List_Files(); // DONE
        public Task<bool> Navigate_To_Directory(string directory_path); // DONE
        public Task<bool> Navigate_To_Previos_Directory(); // DONE
        public Task<bool> Create_Directory(string directory_path);
        public Task<bool> Delete_Directory(string directory_path);
        public Task<bool> Search_File(string file_name);
        public Task<bool> Create_File(string file_path);
        public Task<bool> Delete_File(string file_path);
        public Task<bool> Open_File(string file_path);// DONE
    }
}
