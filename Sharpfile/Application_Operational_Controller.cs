using Microsoft.Win32.SafeHandles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Sharpfile
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
        public static async Task<bool> Controller(Application_Operations operation)
        {
            string formated_path = String.Empty;
            switch (operation)
            {
                case Application_Operations.Redraw_Window_And_Load_Window:
                    await Initiate_Operation(Operations.List_Files, String.Empty);
                    await GUI_Contents.Redraw_Screen();
                    break;
                case Application_Operations.Redraw_Window:
                    await GUI_Contents.Redraw_Screen();
                    break;
                case Application_Operations.Redraw_File_Unit:
                    await GUI_Contents.Redraw_File_Unit();
                    break;
                case Application_Operations.Open_Files:
                    formated_path = Null_Check(await Sub_Operations_Controller( Sub_Operations.File_Path_Generation, Program.current_directory.ElementAt(Program.current_index).Item2) as string);

                    if(Initiate_State(formated_path, Operation_States.File_Open_Initiated) == true)
                        switch (Program.current_directory.ElementAt(Program.current_index).Item3 == "dir")
                        {
                            case true:
                                Program.current_index = 0;
                                Program.cursor_location = 0;
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
                    string? path = String.Empty;
                    Program.Directories_Browser.TryPeek(out path);

                    if (Convert.ToBoolean(await Sub_Operations_Controller(Sub_Operations.Get_If_File_Is_Directory, Null_Check(path))))
                    {
                        Program.current_index = 0;
                        Program.cursor_location = 0;

                        if (Initiate_State(Null_Check(Program.selection_buffer), Operation_States.Directory_Navigation_Initiated) == true)
                            await Initiate_Operation(Operations.Navigate_To_Directory, System.IO.Path.GetFullPath(Null_Check(Program.selection_buffer)));
                    }
                    break;
                case Application_Operations.Change_Location:
                    if(Program.selection_buffer != null)
                    {
                        lock (Program.selection_buffer)
                        {
                            GUI_Contents.Location_Selection_Menu(Null_Check(Program.selection_buffer));
                        }
                    }
                    break;
                case Application_Operations.Go_Back:
                    if (Initiate_State(Program.Directories_Browser.ElementAt(Program.Directories_Browser.Count - 2), Operation_States.Directory_Navigation_Initiated) == true)
                        await Initiate_Operation(Operations.Navigate_To_Previous_Directory, String.Empty);
                        await GUI_Contents.Redraw_Screen();
                    break;
                case Application_Operations.Create_Directory:
                    formated_path = Null_Check(await Sub_Operations_Controller(Sub_Operations.File_Path_Generation, Null_Check(Program.selection_buffer)) as string);
                    await Initiate_Operation(Operations.Create_Directory, formated_path);
                    await Initiate_Operation(Operations.List_Files, String.Empty);
                    await GUI_Contents.Redraw_Screen();
                    break;
                case Application_Operations.Delete_File_Or_Directory:
                    formated_path = Null_Check(await Sub_Operations_Controller(Sub_Operations.File_Path_Generation, Program.current_directory.ElementAt(Program.current_index).Item2) as string);

                    Thread file_deletion_thread = new Thread(async() =>
                    {
                        switch (Program.current_directory.ElementAt(Program.current_index).Item3 == "dir")
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
                    });
                    file_deletion_thread.Priority = ThreadPriority.Highest;
                    file_deletion_thread.IsBackground = true;
                    file_deletion_thread.Start();
                    break;
                case Application_Operations.Item_Search:
                    formated_path = Null_Check(await Sub_Operations_Controller(Sub_Operations.File_Path_Generation, Null_Check(Program.selection_buffer)) as string);
                    await Initiate_Operation(Operations.Search_File, formated_path);
                    await GUI_Contents.Redraw_Screen();
                    break;
                case Application_Operations.Terminal:
                    await Initiate_Operation(Operations.Open_Current_Directory_In_Terminal, String.Empty);
                    break;
                case Application_Operations.Rename_File:
                    switch (Program.current_directory.ElementAt(Program.current_index).Item3 == "dir")
                    {
                        case true:
                            formated_path = Null_Check(await Sub_Operations_Controller(Sub_Operations.File_Path_Generation, Null_Check(Program.selection_buffer)) as string);
                            await Initiate_Operation(Operations.Rename_Or_Move_Directory, formated_path);
                            break;
                        case false:
                            formated_path = Null_Check(await Sub_Operations_Controller(Sub_Operations.File_Path_Generation, Null_Check(Program.selection_buffer)) as string);
                            await Initiate_Operation(Operations.Rename_Or_Move_File, formated_path);
                            break;
                    }

                    await Initiate_Operation(Operations.List_Files, String.Empty);
                    await GUI_Contents.Redraw_Screen();
                    break;
                case Application_Operations.Move_File:

                    Thread file_move_thread = new Thread(async () =>
                    {
                        switch (Program.current_directory.ElementAt(Program.current_index).Item3 == "dir")
                        {
                            case true:
                                formated_path = Null_Check(await Sub_Operations_Controller(Sub_Operations.Custom_File_Path_Generation, Null_Check(Program.selection_buffer), Program.current_directory.ElementAt(Program.current_index).Item2) as string);
                                await Initiate_Operation(Operations.Rename_Or_Move_Directory, formated_path);
                                break;
                            case false:
                                formated_path = Null_Check(await Sub_Operations_Controller(Sub_Operations.Custom_File_Path_Generation, Null_Check(Program.selection_buffer), Program.current_directory.ElementAt(Program.current_index).Item2) as string);
                                await Initiate_Operation(Operations.Rename_Or_Move_File, formated_path);
                                break;
                        }

                        await Initiate_Operation(Operations.List_Files, String.Empty);
                        await GUI_Contents.Redraw_Screen();
                    });
                    file_move_thread.Priority = ThreadPriority.Highest;
                    file_move_thread.IsBackground = true;
                    file_move_thread.Start();
                    break;
                case Application_Operations.Copy_File:
                    Thread copy_file_thread = new Thread(async () =>
                    {
                        if (Program.selection_buffer != null)
                        {
                            lock (Program.selection_buffer)
                            {
                                formated_path = Path.GetFullPath(Program.selection_buffer);
                            }
                        }

                        if (formated_path != String.Empty)
                        {
                            switch (Program.current_directory.ElementAt(Program.current_index).Item3 == "dir")
                            {
                                case true:
                                    await Initiate_Operation(Operations.Copy_Directory, formated_path);
                                    break;
                                case false:
                                    await Initiate_Operation(Operations.Copy_File, formated_path);
                                    break;
                            }

                            await Initiate_Operation(Operations.List_Files, String.Empty);
                            await GUI_Contents.Redraw_Screen();
                        }
                    });
                    copy_file_thread.Priority = ThreadPriority.Highest;
                    copy_file_thread.IsBackground = false;
                    copy_file_thread.Start();
                    break;
            }

            return true;
        }

    }
}
