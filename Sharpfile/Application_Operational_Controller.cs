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
            Load_Files_And_Render_Files,
            Render_Files,
            Render_File_Navigation,
            Open_Files

        }
        public static async void Controller(Application_Operations operation)
        {
            switch(operation)
            {
                case Application_Operations.Load_Files_And_Render_Files:
                    await Initiate_Operation(Operations.List_Files, "");
                    GUI_Contents.Print_Current_Directory_Contents();
                    break;
                case Application_Operations.Render_Files:
                    GUI_Contents.Print_Current_Directory_Contents();
                    break;
                case Application_Operations.Render_File_Navigation:
                    
                    break;
                case Application_Operations.Open_Files:
                    string current_item = Program.current_directory[Program.current_index].Item2;
                    string current_item_path = new FileInfo(current_item).FullName;
                    switch (await Get_If_Path_Is_File_Or_Directory(current_item_path))
                    {
                        case true:
                            await Initiate_Operation(Operations.Navigate_To_Directory, current_item_path);
                            GUI_Contents.Print_Current_Directory_Contents();
                            break;
                        case false:
                            break;
                    }
                    break;
            }
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
    }
}
