using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sharpfile
{
    internal class Application_Operational_Controller:Main_Operational_Controller
    {
        public enum Application_Operations
        {
            Redraw_Window_And_Load_Window,
            Redraw_Window,
            Redraw_File_Unit,
            Open_Files,
            Go_Back,
            Change_Location,
            Navigate_To_Directory,
            Create_Directory,
            Delete_File_Or_Directory,
            Item_Search,
            Terminal
        }
        public static async void Controller(Application_Operations operation)
        {
            string formated_path = String.Empty;

            switch (operation)
            {
                case Application_Operations.Redraw_Window_And_Load_Window:
                    await Initiate_Operation(Operations.List_Files, String.Empty);
                    await GUI_Contents.Redraw_Screen();
                    Thread.Sleep(1000);
                    break;
                case Application_Operations.Redraw_Window:
                    await GUI_Contents.Redraw_Screen();
                    Thread.Sleep(100);
                    break;
                case Application_Operations.Redraw_File_Unit:
                    GUI_Contents.Redraw_File_Unit();
                    break;
                case Application_Operations.Open_Files:
                    formated_path = File_Path_Generator(Program.current_directory[Program.current_index].Item2);

                    switch (Get_If_Path_Is_File_Or_Directory(formated_path))
                    {
                        case true:
                            Program.current_index= 0;
                            Program.cursor_location = 0;
                            await Initiate_Operation(Operations.Navigate_To_Directory, formated_path);
                            break;
                        case false:
                            await Initiate_Operation(Operations.Open_File, formated_path);
                            break;
                    }

                    await GUI_Contents.Redraw_Screen();

                    switch (Get_If_Path_Is_File_Or_Directory(formated_path))
                    {
                        case true:
                            Thread.Sleep(1000);
                            break;
                        case false:
                            Thread.Sleep(100);
                            break;
                    }
                    break;
                case Application_Operations.Navigate_To_Directory:
                    string path = String.Empty;
                    Program.Directories_Browser.TryPeek(out path);

                    if (Get_If_Path_Is_File_Or_Directory(path))
                    {
                        Program.current_index = 0;
                        Program.cursor_location = 0;
                        await Initiate_Operation(Operations.Navigate_To_Directory, Program.selection_buffer);
                    }
                    break;
                case Application_Operations.Change_Location:
                    lock(Program.selection_buffer)
                    {
                        GUI_Contents.Location_Selection_Menu(Program.selection_buffer);
                    }
                    break;
                case Application_Operations.Go_Back:
                    await Initiate_Operation(Operations.Navigate_To_Previous_Directory, null);
                    await GUI_Contents.Redraw_Screen();
                    Thread.Sleep(100);
                    break;
                case Application_Operations.Create_Directory:
                    formated_path = File_Path_Generator(Program.selection_buffer);
                    await Initiate_Operation(Operations.Create_Directory, formated_path);
                    await Initiate_Operation(Operations.List_Files, String.Empty);
                    await GUI_Contents.Redraw_Screen();
                    Thread.Sleep(1000);
                    break;
                case Application_Operations.Delete_File_Or_Directory:

                    formated_path = File_Path_Generator(Program.current_directory[Program.current_index].Item2);

                    switch (Get_If_Path_Is_File_Or_Directory(formated_path))
                    {
                        case true:
                            await Initiate_Operation(Operations.Delete_Directory, formated_path);
                            break;
                        case false:
                            await Initiate_Operation(Operations.Delete_File, formated_path);
                            break;
                    }

                    await Initiate_Operation(Operations.List_Files, String.Empty);
                    await GUI_Contents.Redraw_Screen();
                    Thread.Sleep(1000);
                    break;
                case Application_Operations.Item_Search:
                    formated_path = File_Path_Generator(Program.selection_buffer);

                    await Initiate_Operation(Operations.Search_File, formated_path);
                    await GUI_Contents.Redraw_Screen();
                    break;
                case Application_Operations.Terminal:
                    await Initiate_Operation(Operations.Open_Current_Directory_In_Terminal, String.Empty);
                    break;
            }
        }


        private static string File_Path_Generator(string file)
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

            return formated_path.ToString();
        }

        private static bool Get_If_Path_Is_File_Or_Directory(string current_item_path)
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

            return is_directory;
        }

    }
}
