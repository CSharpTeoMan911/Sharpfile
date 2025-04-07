using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sharpfile
{
    internal class OS_Independent_Operations: File_System_Operations
    {
        private File_System_Operations file_system_operations { get; }

        public OS_Independent_Operations(File_System_Operations operations)
        {
            file_system_operations = operations;
        }

        public bool List_Files()
        {
            bool operation_result = false;
            operation_result = file_system_operations.List_Files();
            return operation_result;
        }

        public bool Create_Directory(string directory_path)
        {
            bool operation_result = false;
            operation_result = file_system_operations.Create_Directory(directory_path);
            return operation_result;
        }

        public bool Delete_Directory(string directory_path)
        {
            bool operation_result = false;
            operation_result = file_system_operations.Delete_Directory(directory_path);
            return operation_result;
        }

        public bool Navigate_To_Previos_Directory()
        {
            bool operation_result = false;
            operation_result = file_system_operations.Navigate_To_Previos_Directory();
            return operation_result;
        }

        public bool Search_File(string file_name)
        {
            bool operation_result = false;
            operation_result = file_system_operations.Search_File(file_name);
            return operation_result;
        }

        public bool Open_Current_Directory_In_Terminal()
        {
            bool operation_result = false;
            operation_result = file_system_operations.Open_Current_Directory_In_Terminal();
            return operation_result;
        }

        public bool Delete_File(string file_path)
        {
            bool operation_result = false;
            operation_result = file_system_operations.Delete_File(file_path);
            return operation_result;
        }

        public bool Open_File(string file_path)
        {
            bool operation_result = false;
            operation_result = file_system_operations.Open_File(file_path);
            return operation_result;
        }

        public bool Navigate_To_Directory(string directory_path)
        {
            bool operation_result = false;
            operation_result = file_system_operations.Navigate_To_Directory(directory_path);
            return operation_result;
        }

        public bool Move_Or_Rename_File(string path)
        {
            bool operation_result = false;
            operation_result = file_system_operations.Move_Or_Rename_File(path);
            return operation_result;
        }

        public bool Move_Or_Rename_Directory(string path)
        {
            bool operation_result = false;
            operation_result = file_system_operations.Move_Or_Rename_Directory(path);
            return operation_result;
        }

        public bool Copy_File(string path)
        {
            bool operation_result = false;
            operation_result = file_system_operations.Copy_File(path);
            return operation_result;
        }

        public bool Copy_Directory(string path)
        {
            bool operation_result = false;
            operation_result = file_system_operations.Copy_Directory(path);
            return operation_result;
        }
    }
}
