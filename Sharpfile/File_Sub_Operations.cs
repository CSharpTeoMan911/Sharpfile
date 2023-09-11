using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace Sharpfile
{
    internal class File_Sub_Operations
    {
        public enum Sub_Operations
        {
            Get_File_Name,
            Get_File_Extension,
            Get_File_Permissions,
        }


        protected static async Task<string> Sub_Operations_Controller(Sub_Operations sub_operation, string path)
        {
            string result = String.Empty;
            switch(sub_operation)
            {
                case Sub_Operations.Get_File_Name:
                    result = await Get_File_Name(path);
                    break;
                case Sub_Operations.Get_File_Extension:
                    result = await Get_File_Extension(path);
                    break;
                case Sub_Operations.Get_File_Permissions:
                    result = await Get_File_Permissions(path);
                    break;
            }
            return result;
        }

        private static Task<string> Get_File_Name(string path)
        {
            string result = String.Empty;
            result = System.IO.Path.GetFileName(path);
            return Task.FromResult(result);
        }

        private static Task<string> Get_File_Extension(string path)
        {
            string result = String.Empty;

            if(Directory.Exists(path) == true)
            {
                result = "dir";
            }
            else if(File.Exists(path) == true)
            {
                result = System.IO.Path.GetExtension(path);

                if(result == String.Empty)
                {
                    result = "bin";
                }
            }
            return Task.FromResult(result);
        }

        private static async Task<string> Get_File_Permissions(string path)
        {
            string file_permissions = String.Empty;

            try
            {
                if (System.IO.File.Exists(path))
                {
                    System.IO.FileStream file_stream = System.IO.File.Open(path, System.IO.FileMode.Open);
                    try
                    {
                        StringBuilder permission_builder = new StringBuilder();

                        switch (file_stream.CanRead)
                        {
                            case true:
                                permission_builder.Append('r');
                                break;

                            case false:
                                permission_builder.Append('-');
                                break;
                        }

                        switch (file_stream.CanWrite)
                        {
                            case true:
                                permission_builder.Append('w');
                                break;

                            case false:
                                permission_builder.Append('-');
                                break;
                        }

                        System.Diagnostics.Debug.WriteLine("Permissions: " + permission_builder.ToString());

                        file_permissions = permission_builder.ToString();
                        permission_builder.Clear();
                    }
                    catch
                    {

                    }
                    finally
                    {
                        if (file_stream != null)
                        {
                            await file_stream.DisposeAsync();
                        }
                    }
                }
            }
            catch { }
            /*

            if (System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.Linux) == true)
            {
                string file_name = await Get_File_Name(path);

                string argument = "-c \"cd '" + new DirectoryInfo(path).Parent + "' && ls -all '" + file_name + "'\"";

                if (System.IO.Directory.Exists(path))
                {
                    argument = "-c \"cd '" + new DirectoryInfo(path).Parent + "' && ls -ld '" + file_name + "'\"";
                }

                // SET PROCESS 
                System.Diagnostics.ProcessStartInfo shell_process_start_info = new System.Diagnostics.ProcessStartInfo("/bin/bash", argument);
                shell_process_start_info.RedirectStandardError = true;
                shell_process_start_info.RedirectStandardInput = true;
                shell_process_start_info.RedirectStandardOutput = true;

                System.Diagnostics.Process shell_process = new System.Diagnostics.Process();
                shell_process.StartInfo = shell_process_start_info;
                shell_process.Start();

                int space_index = 0;

                StringBuilder result = new StringBuilder(shell_process.StandardOutput.ReadLine());

                for (int i = 0; i < result.Length; i++)
                {
                    if (result[i] == ' ')
                    {
                        space_index = i;
                        break;
                    }
                }


                result.Remove(space_index, (result.Length - space_index));
                file_permissions = result.ToString();
                result.Clear();
            }
            else if (System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.Windows) == true)
            {
                try
                {
                    if(System.IO.File.Exists(path))
                    {
                        System.IO.FileStream file_stream = System.IO.File.Open(path, System.IO.FileMode.Open);
                        try
                        {
                            StringBuilder permission_builder = new StringBuilder();

                            switch(file_stream.CanRead)
                            {
                                case true:
                                    permission_builder.Append('r');
                                    break;

                                case false:
                                    permission_builder.Append('-');
                                    break;
                            }

                            switch(file_stream.CanWrite)
                            {
                                case true:
                                    permission_builder.Append('w');
                                    break;

                                case false:
                                    permission_builder.Append('-');
                                    break;
                            }

                            System.Diagnostics.Debug.WriteLine("Permissions: " + permission_builder.ToString());

                            file_permissions = permission_builder.ToString();
                            permission_builder.Clear();
                        }
                        catch
                        {

                        }
                        finally
                        {
                            if(file_stream != null)
                            {
                                await file_stream.DisposeAsync();
                            }
                        }
                    }
                }
                catch(Exception e)
                {
                    System.Diagnostics.Debug.WriteLine("Errors: " + e.Message);
                }



            }
            */

            if(file_permissions == String.Empty)
            {
                file_permissions = "__";
            }

            return file_permissions;
        }
    }
}
