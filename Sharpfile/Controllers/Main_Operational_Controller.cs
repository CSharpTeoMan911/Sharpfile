﻿using Sharpfile.FileSystemFunctions;
using System.Collections.Concurrent;

namespace Sharpfile.Controllers
{
    internal class Main_Operational_Controller:File_Sub_Operations
    {
        private static ConcurrentQueue<string> file_operations_queue = new ConcurrentQueue<string>();

        public enum Operations
        {
            List_Files,
            Create_Directory,
            Delete_Directory,
            Navigate_To_Directory,
            Navigate_To_Previous_Directory,
            Search_File,
            Open_Current_Directory_In_Terminal,
            Delete_File,
            Open_File,
            Rename_Or_Move_File,
            Rename_Or_Move_Directory,
            Copy_File,
            Copy_Directory
        }


        protected static Task Initiate_Operation(Operations operation, string operation_content)
        {
            OS_Independent_Operations? current_operation = null;

            if (current_os == System.Runtime.InteropServices.OSPlatform.Linux)
            {
                current_operation = new OS_Independent_Operations(new Linux_Based_File_Operations());
            }
            else if(current_os == System.Runtime.InteropServices.OSPlatform.Windows)
            {
                current_operation = new OS_Independent_Operations(new Windows_Based_File_Operations());
            }

            if(current_operation != null)
            {
                switch(operation)
                {
                    case Operations.List_Files:
                        current_operation.List_Files();
                        break;
                    case Operations.Navigate_To_Directory:
                        current_operation.Navigate_To_Directory(operation_content);
                        break;
                    case Operations.Create_Directory:
                        current_operation.Create_Directory(operation_content);
                        break;
                    case Operations.Delete_Directory:
                        current_operation.Delete_Directory(operation_content);
                        break;
                    case Operations.Navigate_To_Previous_Directory:
                        current_operation.Navigate_To_Previos_Directory();
                        break;
                    case Operations.Search_File:
                        current_operation.Search_File(operation_content);
                        break;
                    case Operations.Open_Current_Directory_In_Terminal:
                        current_operation.Open_Current_Directory_In_Terminal();
                        break;
                    case Operations.Delete_File:
                        current_operation.Delete_File(operation_content);
                        break;
                    case Operations.Open_File:
                        current_operation.Open_File(operation_content);
                        break;
                    case Operations.Rename_Or_Move_File:
                        current_operation.Move_Or_Rename_File(operation_content);
                        break;
                    case Operations.Rename_Or_Move_Directory:
                        current_operation.Move_Or_Rename_Directory(operation_content);
                        break;
                    case Operations.Copy_File:
                        current_operation.Copy_File(operation_content);
                        break;
                    case Operations.Copy_Directory:
                        current_operation.Copy_Directory(operation_content);
                        break;
                }
            }

            return Task.CompletedTask;
        }
    }
}
