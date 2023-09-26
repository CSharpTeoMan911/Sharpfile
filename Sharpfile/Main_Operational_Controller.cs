using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sharpfile
{
    internal class Main_Operational_Controller:File_Sub_Operations
    {
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


        protected static async Task<bool> Initiate_Operation(Operations operation, string operation_content)
        {
            OS_Independent_Operations? current_operation = null;

            if (System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.Linux) == true)
            {
                current_operation = new OS_Independent_Operations(new Linux_Based_File_Operations());
            }
            else if(System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.Windows) == true)
            {
                current_operation = new OS_Independent_Operations(new Windows_Based_File_Operations());
            }

            if(current_operation != null)
            {
                switch(operation)
                {
                    case Operations.List_Files:
                        await current_operation.List_Files();
                        break;
                    case Operations.Navigate_To_Directory:
                        await current_operation.Navigate_To_Directory(operation_content);
                        break;
                    case Operations.Create_Directory:
                        await current_operation.Create_Directory(operation_content);
                        break;
                    case Operations.Delete_Directory:
                        await current_operation.Delete_Directory(operation_content);
                        break;
                    case Operations.Navigate_To_Previous_Directory:
                        await current_operation.Navigate_To_Previos_Directory();
                        break;
                    case Operations.Search_File:
                        await current_operation.Search_File(operation_content);
                        break;
                    case Operations.Open_Current_Directory_In_Terminal:
                        await current_operation.Open_Current_Directory_In_Terminal();
                        break;
                    case Operations.Delete_File:
                        await current_operation.Delete_File(operation_content);
                        break;
                    case Operations.Open_File:
                        await current_operation.Open_File(operation_content);
                        break;
                    case Operations.Rename_Or_Move_File:
                        await current_operation.Move_Or_Rename_File(operation_content);
                        break;
                    case Operations.Rename_Or_Move_Directory:
                        await current_operation.Move_Or_Rename_Directory(operation_content);
                        break;
                    case Operations.Copy_File:
                        await current_operation.Copy_File(operation_content);
                        break;
                    case Operations.Copy_Directory:
                        await current_operation.Copy_Directory(operation_content);
                        break;
                }
            }

            return true;
        }
    }
}
