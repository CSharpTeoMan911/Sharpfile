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
            Go_Back
        }
        public static void Controller(Application_Operations operation)
        {
            string current_item = String.Empty;
            string current_item_path = String.Empty;

            Thread.Sleep(100);
            switch (operation)
            {
                case Application_Operations.Redraw_Window_And_Load_Window:
                    Initiate_Operation(Operations.List_Files, "");
                    GUI_Contents.Redraw_Screen();
                    break;
                case Application_Operations.Redraw_Window:
                    GUI_Contents.Redraw_Screen();
                    break;
                case Application_Operations.Redraw_File_Unit:
                    GUI_Contents.Redraw_File_Unit();
                    break;
                case Application_Operations.Open_Files:
                    current_item = Program.current_directory[Program.current_index].Item2;
                    current_item_path = new FileInfo(current_item).FullName;
                    switch (Get_If_Path_Is_File_Or_Directory(current_item_path))
                    {
                        case true:
                            Initiate_Operation(Operations.Navigate_To_Directory, current_item_path);
                            break;
                        case false:
                            Initiate_Operation(Operations.Open_File, current_item_path);
                            break;
                    }
                    GUI_Contents.Redraw_Screen();
                    break;

                case Application_Operations.Go_Back:
                    Initiate_Operation(Operations.Navigate_To_Previous_Directory, null);
                    GUI_Contents.Redraw_Screen();
                    break;
            }
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
