using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sharpfile
{
    internal interface File_System_Operations
    {
        public Task<bool> List_Files();
        public Task<bool> Create_Directory();
        public Task<bool> Delete_Directory();
        public Task<bool> Navigate_To_Next_Directory();
        public Task<bool> Navigate_To_Previos_Directory();
        public Task<bool> Search_File();
        public Task<bool> Create_File();
        public Task<bool> Delete_File();
        public Task<bool> Open_File();
    }
}
