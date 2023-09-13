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
            Navigate_To_Directory
        }
        public static async void Controller(Application_Operations operation)
        {
            string current_item = String.Empty;
            string current_item_path = String.Empty;

            switch (operation)
            {
                case Application_Operations.Redraw_Window_And_Load_Window:
                    await Initiate_Operation(Operations.List_Files, "");
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
                    current_item = Program.current_directory[Program.current_index].Item2;
                    current_item_path = File_Path_Generator(current_item);

                    switch (Get_If_Path_Is_File_Or_Directory(current_item_path))
                    {
                        case true:
                            await Initiate_Operation(Operations.Navigate_To_Directory, current_item_path);
                            break;
                        case false:
                            await Initiate_Operation(Operations.Open_File, current_item_path);
                            break;
                    }

                    await GUI_Contents.Redraw_Screen();

                    switch (Get_If_Path_Is_File_Or_Directory(current_item_path))
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
                    bool directory_valid = false;

                    lock(Program.Current_Directory)
                    {
                        if (Get_If_Path_Is_File_Or_Directory(Program.Current_Directory))
                        {
                            directory_valid = true;
                        }
                    }

                    if(directory_valid == true)
                    {
                        await Initiate_Operation(Operations.Navigate_To_Directory, Program.location_buffer);
                    }

                    break;
                case Application_Operations.Change_Location:
                    lock(Program.location_buffer)
                    {
                        GUI_Contents.Location_Selection_Menu(Program.location_buffer);
                    }
                    break;
                case Application_Operations.Go_Back:
                    await Initiate_Operation(Operations.Navigate_To_Previous_Directory, null);
                    await GUI_Contents.Redraw_Screen();
                    Thread.Sleep(100);
                    break;
            }
        }


        private static string File_Path_Generator(string file)
        {
            string current_item_path = String.Empty;

            if (System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.Linux))
            {
                current_item_path = Program.Current_Directory + "/" + file;
            }
            else if (System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.Windows))
            {
                current_item_path = Program.Current_Directory + "\\" + file;
            }

            return current_item_path;
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
