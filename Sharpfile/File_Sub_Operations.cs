using System.IO;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Text;


namespace Sharpfile
{
    internal class File_Sub_Operations
    {
        public enum Sub_Operations
        {
            Get_File_Name,
            Get_File_Extension,
            Get_File_Permissions,
            Get_If_File_Is_Directory,
            File_Path_Generation
        }


        protected static async Task<object> Sub_Operations_Controller(Sub_Operations sub_operation, string path)
        {
            object result = String.Empty;

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
                case Sub_Operations.Get_If_File_Is_Directory:
                    result = await Get_If_Path_Is_File_Or_Directory(path);
                    break;
                case Sub_Operations.File_Path_Generation:
                    result = await File_Path_Generator(path);
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


        [System.Diagnostics.CodeAnalysis.SuppressMessage("Interoperability", "CA1416:Validate platform compatibility", Justification = "<Pending>")]
        private static async Task<string> Get_File_Permissions(string path)
        {
            string file_permissions = "__";

            try
            {
                if (System.IO.Path.Exists(path))
                {
                    if (System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.Linux) == true)
                    {
                        string file_mode = System.IO.File.GetUnixFileMode(path).ToString();

                        if((file_mode.Contains("UserRead") == true) && (file_mode.Contains("UserWrite") == true) && (file_mode.Contains("UserExecute") == true))
                        {
                            file_permissions = "rwx";
                        }
                        else if((file_mode.Contains("UserRead") == true) && (file_mode.Contains("UserWrite") == true))
                        {
                            file_permissions = "rw_";
                        }
                        else if ((file_mode.Contains("UserRead") == true) && (file_mode.Contains("UserExecute") == true))
                        {
                            file_permissions = "r_x";
                        }
                        else if ((file_mode.Contains("UserWrite") == true) && (file_mode.Contains("UserExecute") == true))
                        {
                            file_permissions = "_wx";
                        }
                        else if (file_mode.Contains("UserRead") == true) 
                        {
                            file_permissions = "r__";
                        }
                        else if (file_mode.Contains("UserWrite") == true)
                        {
                            file_permissions = "_w_";
                        }
                        else if (file_mode.Contains("UserExecute") == true)
                        {
                            file_permissions = "__x";
                        }
                        else
                        {
                            file_permissions = "___";
                        }
                    }
                    else if (System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.Windows) == true)
                    {
                        
                        if (System.IO.Directory.Exists(path) == true)
                        {
                            System.Security.AccessControl.DirectorySecurity directory_security = new DirectoryInfo(path).GetAccessControl();

                            AuthorizationRuleCollection rules = directory_security.GetAccessRules(true, true, typeof(NTAccount));

                            //Go through the rules returned from the DirectorySecurity
                            foreach (AuthorizationRule rule in rules)
                            {
                                //If we find one that matches the identity we are looking for
                                if (rule.IdentityReference.Value.Equals(System.Security.Principal.WindowsIdentity.GetCurrent().Name, StringComparison.CurrentCultureIgnoreCase))
                                {
                                    var filesystemAccessRule = (FileSystemAccessRule)rule;

                                    //Cast to a FileSystemAccessRule to check for access rights
                                    if ((filesystemAccessRule.FileSystemRights & FileSystemRights.ReadData) > 0 && (filesystemAccessRule.FileSystemRights & FileSystemRights.WriteData)  > 0 && filesystemAccessRule.AccessControlType != AccessControlType.Deny)
                                    {
                                        file_permissions = "rw";
                                    }
                                    else if ((filesystemAccessRule.FileSystemRights & FileSystemRights.ReadData) > 0 && filesystemAccessRule.AccessControlType != AccessControlType.Deny)
                                    {
                                        file_permissions = "r_";
                                    }
                                    else if ((filesystemAccessRule.FileSystemRights & FileSystemRights.WriteData) > 0 && filesystemAccessRule.AccessControlType != AccessControlType.Deny)
                                    {
                                        file_permissions = "_w";
                                    }
                                    else
                                    {
                                        file_permissions = "__";
                                    }
                                }
                            }
                        }
                        else if (System.IO.File.Exists(path) == true)
                        {
                            System.Security.AccessControl.FileSecurity file_security = new FileInfo(path).GetAccessControl(AccessControlSections.All);

                            AuthorizationRuleCollection rules = file_security.GetAccessRules(true, true, typeof(NTAccount));

                            //Go through the rules returned from the DirectorySecurity
                            foreach (AuthorizationRule rule in rules)
                            {
                                //If we find one that matches the identity we are looking for
                                if (rule.IdentityReference.Value.Equals(System.Security.Principal.WindowsIdentity.GetCurrent().Name, StringComparison.CurrentCultureIgnoreCase))
                                {
                                    var filesystemAccessRule = (FileSystemAccessRule)rule;

                                    //Cast to a FileSystemAccessRule to check for access rights
                                    if ((filesystemAccessRule.FileSystemRights & FileSystemRights.ReadData) > 0 && (filesystemAccessRule.FileSystemRights & FileSystemRights.WriteData)  > 0 && filesystemAccessRule.AccessControlType != AccessControlType.Deny)
                                    {
                                        file_permissions = "rw";
                                    }
                                    else if ((filesystemAccessRule.FileSystemRights & FileSystemRights.ReadData) > 0 && filesystemAccessRule.AccessControlType != AccessControlType.Deny)
                                    {
                                        file_permissions = "r_";
                                    }
                                    else if ((filesystemAccessRule.FileSystemRights & FileSystemRights.WriteData) > 0 && filesystemAccessRule.AccessControlType != AccessControlType.Deny)
                                    {
                                        file_permissions = "_w";
                                    }
                                    else
                                    {
                                        file_permissions = "__";
                                    }
                                }
                            }

                        }
                        else
                        {
                            file_permissions = "__";
                        }
                    }

                }
            }
            catch 
            {
                file_permissions = await Read_File_Permissions_Using_Stream(path);
            }
 

            return file_permissions;
        }



        private static async Task<string> Read_File_Permissions_Using_Stream(string path)
        {
            string file_permissions = "__";

            try
            {
                System.IO.FileStream file_stream = System.IO.File.Open(path, System.IO.FileMode.Open);
                try
                {
                    if (file_stream.CanRead == true && file_stream.CanWrite == true)
                    {
                        file_permissions = "rw";
                    }
                    else if (file_stream.CanRead == true)
                    {
                        file_permissions = "r_";
                    }
                    else if (file_stream.CanWrite == true)
                    {
                        file_permissions = "_w";
                    }
                    else
                    {
                        file_permissions = "__";
                    }
                }
                catch
                {
                    file_permissions = "__";
                }
                finally
                {
                    if (file_stream != null)
                    {
                        await file_stream.DisposeAsync();
                    }
                }
            }
            catch { }

            return file_permissions;
        }


        private static Task<bool> Get_If_Path_Is_File_Or_Directory(string current_item_path)
        {
            bool is_directory = false;


            try
            {
                if (Directory.Exists(current_item_path))
                {
                    is_directory = true;
                }
            }
            catch { }

            try
            {
                if (File.Exists(current_item_path))
                {
                    is_directory = false;
                }
            }
            catch { }

            return Task.FromResult(is_directory);
        }

        private static Task<string> File_Path_Generator(string file)
        {
            string path = String.Empty;
            Program.Directories_Browser.TryPeek(out path);

            StringBuilder formated_path = new StringBuilder(path);

            if (System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.Linux))
            {
                formated_path.Append("/");
            }
            else if (System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.Windows))
            {
                formated_path.Append("\\");
            }

            formated_path.Append(file);

            return Task.FromResult(formated_path.ToString());
        }

        protected static string Null_Check(string? result)
        {
            return result == null ? String.Empty : result;
        }
    }
}
