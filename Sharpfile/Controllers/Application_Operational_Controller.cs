using Sharpfile.Shared;
using System.Diagnostics;

namespace Sharpfile.Controllers
{
    internal class Application_Operational_Controller:Main_Operational_Controller
    {
        ///////////////////////////////////////////////////////////////////
        //!!!!!!!!!!!!!!!!!!!!!!!!!!!! TO DO !!!!!!!!!!!!!!!!!!!!!!!!!!!!//
        ///////////////////////////////////////////////////////////////////
        //                                                               //
        // - CREATE A STATE MACHINE THAT PERMITS OR RESTRICTS OPERATIONS //
        //   ON A FILE IF OPERATIONS ON THAT FILE WERE INITIATED         //
        //                                                               //
        // - MEMORY AND CPU OPTIMISATIONS                                //
        //                                                               //
        // - "QUALITY OF LIFE IMPROVEMENTS" SUCH AS CARRET NAVIGATION IN //
        //   THE SELECTION SECTION                                       //
        //                                                               //
        ///////////////////////////////////////////////////////////////////

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
            Terminal,
            Rename_File,
            Move_File,
            Copy_File
        }


        public static async Task Controller(Application_Operations operation)
        {
            string formated_path = string.Empty;
            switch (operation)
            {
                case Application_Operations.Redraw_Window_And_Load_Window:
                    await Initiate_Operation(Operations.List_Files, string.Empty);
                    await GUI_Contents.Redraw_Screen();
                    break;
                case Application_Operations.Redraw_Window:
                     await GUI_Contents.Redraw_Screen();
                    break;
                case Application_Operations.Redraw_File_Unit:
                     await GUI_Contents.Redraw_File_Unit();
                    break;
                case Application_Operations.Open_Files:
                    formated_path = Null_Check(Sub_Operations_Controller(Sub_Operations.File_Path_Generation, current_directory.ElementAt(current_index).Item2) as string);

                    switch (current_directory.ElementAt(current_index).Item3 == "dir")
                    {
                        case true:
                            current_index = 0;
                            cursor_location = 0;
                            await Initiate_Operation(Operations.Navigate_To_Directory, formated_path);
                            await GUI_Contents.Redraw_Screen();
                            break;
                        case false:
                            await Initiate_Operation(Operations.Open_File, formated_path);
                            await GUI_Contents.Redraw_Screen();
                            break;
                    }
                    break;
                case Application_Operations.Navigate_To_Directory:
                    string? path = string.Empty;
                    Directories_Browser.TryPeek(out path);

                    if (Convert.ToBoolean( Sub_Operations_Controller(Sub_Operations.Get_If_File_Is_Directory, Null_Check(path))))
                    {
                        current_index = 0;
                        cursor_location = 0;

                        await Initiate_Operation(Operations.Navigate_To_Directory, Path.GetFullPath(Null_Check(selection_buffer)));
                    }
                    break;
                case Application_Operations.Change_Location:
                    await GUI_Contents.Location_Selection(Null_Check(selection_buffer));
                    break;
                case Application_Operations.Go_Back:
                    await Initiate_Operation(Operations.Navigate_To_Previous_Directory, string.Empty);
                    await GUI_Contents.Redraw_Screen();
                    break;
                case Application_Operations.Create_Directory:
                    formated_path = Null_Check( Sub_Operations_Controller(Sub_Operations.File_Path_Generation, Null_Check(selection_buffer)) as string);
                    await Initiate_Operation(Operations.Create_Directory, formated_path);
                    await Initiate_Operation(Operations.List_Files, string.Empty);
                    await GUI_Contents.Redraw_Screen();
                    break;
                case Application_Operations.Delete_File_Or_Directory:
                    formated_path = Null_Check(Sub_Operations_Controller(Sub_Operations.File_Path_Generation, current_directory.ElementAt(current_index).Item2) as string);

                    Thread file_deletion_thread = new Thread(async() =>
                    {
                        switch (current_directory.ElementAt(current_index).Item3 == "dir")
                        {
                            case true:
                                 await Initiate_Operation(Operations.Delete_Directory, formated_path);
                                break;
                            case false:
                                 await Initiate_Operation(Operations.Delete_File, formated_path);
                                break;
                        }

                         await Initiate_Operation(Operations.List_Files, string.Empty);

                        controller_gui_operations.Add(()=> GUI_Contents.Redraw_Screen());
                    });
                    file_deletion_thread.Priority = ThreadPriority.Highest;
                    file_deletion_thread.IsBackground = true;
                    file_deletion_thread.Start();
                    break;
                case Application_Operations.Item_Search:
                    formated_path = Null_Check( Sub_Operations_Controller(Sub_Operations.File_Path_Generation, Null_Check(selection_buffer)) as string);
                    await Initiate_Operation(Operations.Search_File, formated_path);
                    await GUI_Contents.Redraw_Screen();
                    break;
                case Application_Operations.Terminal:
                    await Initiate_Operation(Operations.Open_Current_Directory_In_Terminal, string.Empty);
                    break;
                case Application_Operations.Rename_File:
                    switch (current_directory.ElementAt(current_index).Item3 == "dir")
                    {
                        case true:
                            formated_path = Null_Check( Sub_Operations_Controller(Sub_Operations.File_Path_Generation, Null_Check(selection_buffer)) as string);
                            await Initiate_Operation(Operations.Rename_Or_Move_Directory, formated_path);
                            break;
                        case false:
                            formated_path = Null_Check( Sub_Operations_Controller(Sub_Operations.File_Path_Generation, Null_Check(selection_buffer)) as string);
                            await Initiate_Operation(Operations.Rename_Or_Move_File, formated_path);
                            break;
                    }

                     await Initiate_Operation(Operations.List_Files, string.Empty);
                     await GUI_Contents.Redraw_Screen();
                    break;
                case Application_Operations.Move_File:

                    Thread file_move_thread = new Thread(async() =>
                    {
                        switch (current_directory.ElementAt(current_index).Item3 == "dir")
                        {
                            case true:
                                formated_path = Null_Check( Sub_Operations_Controller(Sub_Operations.Custom_File_Path_Generation, Null_Check(selection_buffer), current_directory.ElementAt(current_index).Item2) as string);
                                await Initiate_Operation(Operations.Rename_Or_Move_Directory, formated_path);
                                break;
                            case false:
                                formated_path = Null_Check( Sub_Operations_Controller(Sub_Operations.Custom_File_Path_Generation, Null_Check(selection_buffer), current_directory.ElementAt(current_index).Item2) as string);
                                await Initiate_Operation(Operations.Rename_Or_Move_File, formated_path);
                                break;
                        }

                         await Initiate_Operation(Operations.List_Files, string.Empty);
                        controller_gui_operations.Add(() => GUI_Contents.Redraw_Screen());
                    });
                    file_move_thread.Priority = ThreadPriority.Highest;
                    file_move_thread.IsBackground = true;
                    file_move_thread.Start();
                    break;
                case Application_Operations.Copy_File:
                    Thread copy_file_thread = new Thread(async () =>
                    {
                        if (selection_buffer != null)
                            formated_path = Path.GetFullPath(selection_buffer);

                        if (formated_path != string.Empty)
                        {
                            switch (current_directory.ElementAt(current_index).Item3 == "dir")
                            {
                                case true:
                                    await Initiate_Operation(Operations.Copy_Directory, formated_path);
                                    break;
                                case false:
                                    await Initiate_Operation(Operations.Copy_File, formated_path);
                                    break;
                            }

                            await Initiate_Operation(Operations.List_Files, string.Empty);
                            
                            
                            
                            
                            
                            
                            
                            
                            
                            
                            
                            
                            
                            
                            
                            
                            
                            
                            controller_gui_operations.Add(() => GUI_Contents.Redraw_Screen());
                        }
                    });
                    copy_file_thread.Priority = ThreadPriority.Highest;
                    copy_file_thread.IsBackground = false;
                    copy_file_thread.Start();
                    break;
            }
        }

    }
}
