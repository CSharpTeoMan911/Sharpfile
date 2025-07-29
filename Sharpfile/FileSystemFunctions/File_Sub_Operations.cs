using Sharpfile.Shared;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Text;


namespace Sharpfile.FileSystemFunctions
{
    internal class File_Sub_Operations:Extra_Functions
    {
#pragma warning disable CA1416 // Validate platform compatibility
        public enum Sub_Operations
        {
            Get_File_Name,
            Get_File_Extension,
            Get_File_Permissions,
            Get_If_File_Is_Directory,
            File_Path_Generation,
            Custom_File_Path_Generation,
            Terminator_Formater,
            Random_File_Name_Generator,
            Random_Directory_Name_Generator,
            Path_Separator_Generator
        }


        // CONTROLLERS
        //
        // [ BEGIN ]

        protected static object Sub_Operations_Controller(Sub_Operations sub_operation, string path)
        {
            object result = string.Empty;

            switch(sub_operation)
            {
                case Sub_Operations.Get_File_Name:
                    result = Get_File_Name(path);
                    break;
                case Sub_Operations.Get_File_Extension:
                    result = Get_File_Extension(path);
                    break;
                case Sub_Operations.Get_File_Permissions:
                    result = Get_File_Permissions(path);
                    break;
                case Sub_Operations.Get_If_File_Is_Directory:
                    result = Get_If_Path_Is_File_Or_Directory(path);
                    break;
                case Sub_Operations.File_Path_Generation:
                    result = File_Path_Generator(path);
                    break;
                case Sub_Operations.Random_File_Name_Generator:
                    result = Random_File_Name_Generator(path);
                    break;
                case Sub_Operations.Random_Directory_Name_Generator:
                    result = Random_Directory_Name_Generator(path);
                    break;
                case Sub_Operations.Path_Separator_Generator:
                    result = OS_Platform_Independent_Separator();
                    break;
            }
            return result;
        }

        protected static object Sub_Operations_Controller(Sub_Operations sub_operation, string source_path, string item)
        {
            object result = string.Empty;

            if(sub_operation == Sub_Operations.Custom_File_Path_Generation)
            {
                result = Custom_File_Path_Generator(source_path, item);
            }

            return result;
        }

        // [ END ]




        private static string Get_File_Name(string path)
        {
            string result = string.Empty;
            result = Path.GetFileName(path);
            return result;
        }

        private static string Get_File_Extension(string path)
        {
            string result = string.Empty;

            if(Directory.Exists(path) == true)
            {
                result = "dir";
            }
            else if(File.Exists(path) == true)
            {
                result = Path.GetExtension(path);

                if (current_os == System.Runtime.InteropServices.OSPlatform.Linux)
                {
                    if(result == string.Empty)
                    {
                        string file_mode = File.GetUnixFileMode(path).ToString();

                        if (file_mode.Contains("UserExecute") == true)
                        {
                            result = "bin";
                        }
                    }
                }
                else if(current_os == System.Runtime.InteropServices.OSPlatform.Windows)
                {
                    if (result == string.Empty || result == ".exe")
                    {
                        result = "bin";
                    }
                }
            }
            return result;
        }

        private static string Get_File_Permissions(string path)
        {
            string file_permissions = "__";

            try
            {
                if (Path.Exists(path))
                {
                    if (current_os == System.Runtime.InteropServices.OSPlatform.Linux)
                    {
                        string file_mode = File.GetUnixFileMode(path).ToString();

                        if(file_mode.Contains("UserRead") == true && file_mode.Contains("UserWrite") == true && file_mode.Contains("UserExecute") == true)
                        {
                            file_permissions = "rwx";
                        }
                        else if(file_mode.Contains("UserRead") == true && file_mode.Contains("UserWrite") == true)
                        {
                            file_permissions = "rw_";
                        }
                        else if (file_mode.Contains("UserRead") == true && file_mode.Contains("UserExecute") == true)
                        {
                            file_permissions = "r_x";
                        }
                        else if (file_mode.Contains("UserWrite") == true && file_mode.Contains("UserExecute") == true)
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
                    else if (current_os == System.Runtime.InteropServices.OSPlatform.Windows)
                    {
                        
                        if (Directory.Exists(path) == true)
                        {
                            DirectorySecurity directory_security = new DirectoryInfo(path).GetAccessControl();

                            AuthorizationRuleCollection rules = directory_security.GetAccessRules(true, true, typeof(NTAccount));

                            foreach (AuthorizationRule rule in rules)
                            {
                                if (rule.IdentityReference.Value.Equals(WindowsIdentity.GetCurrent().Name, StringComparison.CurrentCultureIgnoreCase))
                                {
                                    var filesystemAccessRule = (FileSystemAccessRule)rule;

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
                        else if (File.Exists(path) == true)
                        {
                            FileSecurity file_security = new FileInfo(path).GetAccessControl(AccessControlSections.All);

                            AuthorizationRuleCollection rules = file_security.GetAccessRules(true, true, typeof(NTAccount));

                            foreach (AuthorizationRule rule in rules)
                            {
                                if (rule.IdentityReference.Value.Equals(WindowsIdentity.GetCurrent().Name, StringComparison.CurrentCultureIgnoreCase))
                                {
                                    var filesystemAccessRule = (FileSystemAccessRule)rule;

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
                file_permissions =  Read_File_Permissions_Using_Stream(path);
            }
 

            return file_permissions;
        }



        private static string Read_File_Permissions_Using_Stream(string path)
        {
            string file_permissions = "__";

            try
            {
                FileStream file_stream = File.Open(path, FileMode.Open);
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
                         file_stream.DisposeAsync();
                    }
                }
            }
            catch { }

            return file_permissions;
        }


        private static bool Get_If_Path_Is_File_Or_Directory(string current_item_path)
        {
            bool is_directory = false;

            try
            {
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
            }
            catch { }

            return is_directory;
        }

        private static string File_Path_Generator(string file)
        {
            string? path = string.Empty;
            Directories_Browser.TryPeek(out path);

            StringBuilder formated_path = new StringBuilder(Null_Check(path));
            formated_path.Append(OS_Platform_Independent_Separator());
            formated_path.Append(file);

            return Path.GetFullPath(formated_path.ToString());
        }


        private static string Custom_File_Path_Generator(string source_path, string item)
        {
            StringBuilder formated_path = new StringBuilder(source_path);

            formated_path.Append(OS_Platform_Independent_Separator());

            formated_path.Append(item);

            return Path.GetFullPath(formated_path.ToString());
        }

        private static string Random_File_Name_Generator(string? path)
        {
            if (path != null)
            {
                int copy_num = 0;

                FileInfo file = new FileInfo(path);

                StringBuilder formated_path = new StringBuilder();

                while (true)
                {
                    formated_path.Clear();
                    formated_path.Append(file?.Directory?.FullName);
                    formated_path.Append(OS_Platform_Independent_Separator());
                    formated_path.Append(file?.Name);
                    formated_path.Append(" Copy(");
                    formated_path.Append(copy_num);
                    formated_path.Append(")");
                    formated_path.Append(file?.Extension);

                    copy_num++;

                    if (File.Exists(formated_path.ToString()) == false)
                        break;
                }


                return formated_path.ToString();
            }
            else
            {
                return string.Empty;
            }
        }

        private static string Random_Directory_Name_Generator(string? path)
        {
            int copy_num = 0;

            StringBuilder formated_path = new StringBuilder(path);

            while (Directory.Exists(formated_path.ToString()) == true)
            {
                formated_path.Clear();
                formated_path.Append(path);
                formated_path.Append(" Copy");
                formated_path.Append('(');
                formated_path.Append(copy_num);
                formated_path.Append(')');
                copy_num++;
            }

            return formated_path.ToString();
        }


        private static string OS_Platform_Independent_Separator()
        {
            if (current_os == System.Runtime.InteropServices.OSPlatform.Windows)
            {
                return "\\";
            }
            else
            { 
                return "/";
            }
        }
    }
}
