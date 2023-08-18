using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sharpfile
{
    internal class Linux_Based_File_Operations : File_System_Operations
    {
        Task<bool> File_System_Operations.Create_Directory()
        {
            throw new NotImplementedException();
        }

        Task<bool> File_System_Operations.Create_File()
        {
            throw new NotImplementedException();
        }

        Task<bool> File_System_Operations.Delete_Directory()
        {
            throw new NotImplementedException();
        }

        Task<bool> File_System_Operations.Delete_File()
        {
            throw new NotImplementedException();
        }

        Task<bool> File_System_Operations.List_Files()
        {
            throw new NotImplementedException();
        }

        Task<bool> File_System_Operations.Navigate_To_Next_Directory()
        {
            throw new NotImplementedException();
        }

        Task<bool> File_System_Operations.Navigate_To_Previos_Directory()
        {
            throw new NotImplementedException();
        }

        Task<bool> File_System_Operations.Open_File()
        {
            throw new NotImplementedException();
        }

        Task<bool> File_System_Operations.Search_File()
        {
            throw new NotImplementedException();
        }
    }
}
