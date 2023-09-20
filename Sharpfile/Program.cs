using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using TextCopy;

namespace Sharpfile
{
    class Program
    {

        public static List<Tuple<string, string, string, ConsoleColor>> current_directory = new List<Tuple<string, string, string, ConsoleColor>>();
        public static string current_directory_permissions = "__";
        
        public static int current_index = 0;
        public static int cursor_location = 0;
        public static int start_index = 0;

        public static string? clipboard = String.Empty;

        public static ConsoleColor Default_Console_Color = Console.ForegroundColor;
        public static ConsoleColor Default_Console_Background_Color = Console.BackgroundColor;

        public static ConcurrentStack<string> Directories_Browser = new ConcurrentStack<string>();

        public static int Current_Buffer_Width = 0;
        public static int Current_Buffer_Height = 0;

        public static StringBuilder current_input = new StringBuilder();
        public static string? selection_buffer = String.Empty;

        public static bool Selection_Mode = false;

        public static bool Location_Selection_Mode = false;
        public static bool Directory_Creation_Mode = false;
        public static bool Item_Search_Mode = false;
        public static bool File_Rename_Mode = false;
        public static bool File_Relocation_Mode = false;

        public static string Error = String.Empty;


        public static void Main()
        {
            Directories_Browser.Push(Directory.GetCurrentDirectory());
            Console.TreatControlCAsInput = true;
            Console.CursorVisible = false;


            Current_Buffer_Width = Console.BufferWidth;
            Default_Console_Color = Console.ForegroundColor;


            System.Timers.Timer size_change_detection_timer = new System.Timers.Timer();
            size_change_detection_timer.Elapsed += Size_change_detection_timer_Elapsed;
            size_change_detection_timer.Interval = 1000;
            size_change_detection_timer.Start();

            Thread input_thread = new Thread(Read_Input);
            input_thread.Priority = ThreadPriority.Highest;
            input_thread.Start();
        }

        private static async void Read_Input()
        {
            //////////////////////////////////////////////
            ///                COMMANDS                ///
            //////////////////////////////////////////////
            //
            // o = open directory or file
            // i = previous directory
            // ctrl + f = create file =====> esc = exit | enter = create file with selected file name
            // ctrl + p = change path =====> esc = exit | enter = select path
            // ctrl + s = file search =====> esc = exit | enter = select file name to search


            ConsoleKeyInfo cki = new ConsoleKeyInfo();
            Console.Clear();
            Application_Operational_Controller.Controller(Application_Operational_Controller.Application_Operations.Redraw_Window_And_Load_Window);
            do
            {
                try
                {
                    Console.CursorVisible = false;
                    cki = Console.ReadKey(true);


                    if (Selection_Mode == false)
                    {
                        switch (cki.Key)
                        {
                            case ConsoleKey.UpArrow:
                                if (current_input.ToString() != String.Empty)
                                {
                                    Error = String.Empty;
                                    current_input.Clear();
                                    current_input.Append(String.Empty);
                                    Application_Operational_Controller.Controller(Application_Operational_Controller.Application_Operations.Change_Location);
                                }

                                if (current_index > 0)
                                {
                                    current_index--;
                                    cursor_location--;
                                }

                                switch (cursor_location < 0)
                                {
                                    case true:
                                        cursor_location = Console.WindowHeight - 8;
                                        int calculated_value = current_index - cursor_location;
                                        switch (calculated_value >= 0)
                                        {
                                            case true:
                                                start_index = calculated_value;
                                                break;
                                            case false:
                                                start_index = 0;
                                                break;
                                        }
                                        Application_Operational_Controller.Controller(Application_Operational_Controller.Application_Operations.Redraw_Window);
                                        break;
                                    case false:
                                        Application_Operational_Controller.Controller(Application_Operational_Controller.Application_Operations.Redraw_File_Unit);
                                        break;
                                }
                                break;
                            case ConsoleKey.DownArrow:
                                if (current_input.ToString() != String.Empty)
                                {
                                    Error = String.Empty;
                                    current_input.Clear();
                                    current_input.Append(String.Empty);
                                    Application_Operational_Controller.Controller(Application_Operational_Controller.Application_Operations.Change_Location);
                                }

                                if (current_index < current_directory.Count - 1)
                                {
                                    current_index++;
                                    cursor_location++;
                                }

                                switch (cursor_location > Console.WindowHeight - 8)
                                {
                                    case true:
                                        cursor_location = 0;
                                        start_index = current_index;
                                        Application_Operational_Controller.Controller(Application_Operational_Controller.Application_Operations.Redraw_Window);
                                        break;
                                    case false:
                                        Application_Operational_Controller.Controller(Application_Operational_Controller.Application_Operations.Redraw_File_Unit);
                                        break;
                                }
                                break;
                            case ConsoleKey.O:
                                Error = String.Empty;
                                current_input.Clear();
                                current_input.Append(" OPEN FILE");
                                if (current_directory[current_index].Item1[0] == 'r')
                                {
                                    Application_Operational_Controller.Controller(Application_Operational_Controller.Application_Operations.Open_Files);
                                }
                                else
                                {
                                    Error = "[ Error: No permissions to open the file ]";
                                    Application_Operational_Controller.Controller(Application_Operational_Controller.Application_Operations.Change_Location);
                                }
                                break;
                            case ConsoleKey.B:
                                Error = String.Empty;

                                current_input.Clear();
                                current_input.Append(" GO BACK");
                                current_index = 0;
                                cursor_location = 0;
                                start_index = 0;
                                Application_Operational_Controller.Controller(Application_Operational_Controller.Application_Operations.Go_Back);
                                break;
                            case ConsoleKey.C:
                                Error = String.Empty;

                                if (cki.Modifiers == (ConsoleModifiers.Control))
                                {
                                    Console.Clear();
                                    Console.CursorVisible = true;
                                    System.Environment.Exit(0);
                                }
                                else
                                {
                                    current_input.Clear();
                                    current_input.Append("");
                                }
                                break;
                            case ConsoleKey.R:
                                Error = String.Empty;

                                if (cki.Modifiers == (ConsoleModifiers.Control))
                                {
                                    if (current_directory[current_index].Item1[0] == 'r' && current_directory[current_index].Item1[1] == 'w')
                                    {
                                        Selection_Mode = true;
                                        File_Rename_Mode = true;

                                        lock (selection_buffer)
                                        {
                                            selection_buffer = current_directory[current_index].Item2;
                                        }

                                        current_input.Clear();
                                        current_input.Append(" FILE RENAME ( PRESS 'Esc' TO EXIT )");
                                    }
                                    else
                                    {
                                        current_input.Append(" FILE RENAME ");
                                        Error = "[ Error: Insufficient permissions ]";
                                    }

                                    Application_Operational_Controller.Controller(Application_Operational_Controller.Application_Operations.Change_Location);
                                }
                                else
                                {
                                    current_input.Clear();
                                    current_input.Append(" REFRESH");
                                    Application_Operational_Controller.Controller(Application_Operational_Controller.Application_Operations.Redraw_Window_And_Load_Window);
                                }
                                break;
                            case ConsoleKey.L:
                                Error = String.Empty;

                                Selection_Mode = true;
                                Location_Selection_Mode = true;

                                string location_path = String.Empty;
                                Directories_Browser.TryPeek(out location_path);

                                lock (selection_buffer)
                                {
                                    selection_buffer = location_path;
                                }

                                current_input.Clear();
                                current_input.Append(" LOCATION SELECTION ( PRESS 'Esc' TO EXIT )");
                                Application_Operational_Controller.Controller(Application_Operational_Controller.Application_Operations.Change_Location);
                                break;

                            case ConsoleKey.D:
                                if(cki.Modifiers == ConsoleModifiers.Control)
                                {
                                    current_input.Clear();
                                    current_input.Append(" FILE DELETION");

                                    if (current_directory[current_index].Item1[0] == 'r' && current_directory[current_index].Item1[1] == 'w')
                                    {
                                        Error = String.Empty;
                                    }
                                    else
                                    {
                                        Error = "[ Error: Insufficient permissions ]";
                                    }
                                }
                                else
                                {
                                    Error = String.Empty;


                                    Selection_Mode = true;
                                    Directory_Creation_Mode = true;

                                    selection_buffer = String.Empty;

                                    current_input.Clear();
                                    current_input.Append(" DIRECTORY CREATION ( PRESS 'Esc' TO EXIT )");
                                }
                                Application_Operational_Controller.Controller(Application_Operational_Controller.Application_Operations.Change_Location);
                                break;

                            case ConsoleKey.Delete:
                                current_input.Clear();
                                current_input.Append(" FILE DELETION");

                                if (current_directory[current_index].Item1[0] == 'r' && current_directory[current_index].Item1[1] == 'w')
                                {
                                    Error = String.Empty;
                                    Application_Operational_Controller.Controller(Application_Operational_Controller.Application_Operations.Delete_File_Or_Directory);
                                }
                                else
                                {
                                    Error = "[ Error: Insufficient permissions ]";
                                }
                                Application_Operational_Controller.Controller(Application_Operational_Controller.Application_Operations.Change_Location);
                                break;

                            case ConsoleKey.S:
                                Error = String.Empty;

                                Selection_Mode = true;
                                Item_Search_Mode = true;

                                lock (selection_buffer)
                                {
                                    selection_buffer = String.Empty;
                                }

                                current_input.Clear();
                                current_input.Append(" ITEM SEARCH ( 'Esc' TO EXIT )");
                                Application_Operational_Controller.Controller(Application_Operational_Controller.Application_Operations.Change_Location);
                                break;

                            case ConsoleKey.T:
                                Error = String.Empty;

                                current_input.Clear();
                                current_input.Append(" TERMINAL");
                                Application_Operational_Controller.Controller(Application_Operational_Controller.Application_Operations.Terminal);
                                Application_Operational_Controller.Controller(Application_Operational_Controller.Application_Operations.Change_Location);
                                break;

                            case ConsoleKey.M:
                                if(current_directory[current_index].Item1[0] == 'r' && current_directory[current_index].Item1[1] == 'w')
                                {
                                    Error = String.Empty;


                                    Selection_Mode = true;
                                    File_Relocation_Mode = true;

                                    selection_buffer = String.Empty;

                                    current_input.Clear();
                                    current_input.Append(" FILE RELOCATION ( PRESS 'Esc' TO EXIT )");
                                }
                                else
                                {
                                    current_input.Clear();
                                    current_input.Append(" FILE RELOCATION");
                                    Error = "[ Error: Insufficient permissions ]";
                                }

                                Application_Operational_Controller.Controller(Application_Operational_Controller.Application_Operations.Change_Location);
                                break;

                            default:
                                Error = String.Empty;

                                current_input.Clear();
                                current_input.Append(String.Empty);
                                Application_Operational_Controller.Controller(Application_Operational_Controller.Application_Operations.Redraw_Window_And_Load_Window);
                                break;
                        }
                    }
                    else
                    {
                        switch (cki.Key)
                        {
                            case ConsoleKey.Escape:
                                current_input.Clear();
                                current_input.Append("");
                                Location_Selection_Mode = false;
                                Selection_Mode = false;
                                Application_Operational_Controller.Controller(Application_Operational_Controller.Application_Operations.Redraw_Window_And_Load_Window);
                                break;

                            case ConsoleKey.C:

                                if (cki.Modifiers == (ConsoleModifiers.Control))
                                {
                                    Console.Clear();
                                    Console.CursorVisible = true;
                                    System.Environment.Exit(0);
                                }
                                else
                                {
                                    lock (selection_buffer)
                                    {
                                        selection_buffer += cki.KeyChar;
                                    }
                                    Application_Operational_Controller.Controller(Application_Operational_Controller.Application_Operations.Change_Location);
                                }
                                break;

                            case ConsoleKey.V:
                                if ((cki.Modifiers.ToString() == "Shift, Control"))
                                {
                                    selection_buffer += await ClipboardService.GetTextAsync();
                                }
                                else
                                {
                                    lock (selection_buffer)
                                    {
                                        selection_buffer += cki.KeyChar;
                                    }
                                }
                                Application_Operational_Controller.Controller(Application_Operational_Controller.Application_Operations.Change_Location);
                                break;

                            case ConsoleKey.Backspace:
                            
                                Error = String.Empty;

                                lock(selection_buffer)
                                {
                                    if (selection_buffer.Length > 0)
                                    {
                                        selection_buffer = selection_buffer.Substring(0, selection_buffer.Length - 1);
                                    }
                                }
                                Application_Operational_Controller.Controller(Application_Operational_Controller.Application_Operations.Change_Location);
                                break;

                            case ConsoleKey.Enter:

                                if(Location_Selection_Mode == true)
                                {
                                    Error = String.Empty;

                                    lock (selection_buffer)
                                    {
                                        if (System.IO.Directory.Exists(selection_buffer) == true)
                                        {
                                            current_input.Clear();
                                            current_input.Append("");

                                            Location_Selection_Mode = false;
                                            Selection_Mode = false;

                                            Directories_Browser.Push(selection_buffer);
                                            Application_Operational_Controller.Controller(Application_Operational_Controller.Application_Operations.Navigate_To_Directory);
                                            Application_Operational_Controller.Controller(Application_Operational_Controller.Application_Operations.Redraw_Window_And_Load_Window);
                                        }
                                        else
                                        {
                                            Error = "[ Error: Invalid path ]";
                                            Application_Operational_Controller.Controller(Application_Operational_Controller.Application_Operations.Change_Location);
                                        }
                                    }
                                }
                                else if (Directory_Creation_Mode == true)
                                {
                                    Error = String.Empty;

                                    string location_path = String.Empty;
                                    Directories_Browser.TryPeek(out location_path);

                                    if (current_directory_permissions[1] == 'w')
                                    {
                                        lock (selection_buffer)
                                        {
                                            current_input.Clear();
                                            current_input.Append("");

                                            Directory_Creation_Mode = false;
                                            Selection_Mode = false;

                                            Application_Operational_Controller.Controller(Application_Operational_Controller.Application_Operations.Create_Directory);
                                            Application_Operational_Controller.Controller(Application_Operational_Controller.Application_Operations.Redraw_Window_And_Load_Window);
                                        }
                                    }
                                    else
                                    {
                                        Error = "[ Error: Insufficient permissions ]";
                                        Application_Operational_Controller.Controller(Application_Operational_Controller.Application_Operations.Change_Location);
                                    }
                                }
                                else if(Item_Search_Mode == true)
                                {
                                    Error = String.Empty;

                                    lock (selection_buffer)
                                    {
                                        current_input.Clear();
                                        current_input.Append("");

                                        Item_Search_Mode = false;
                                        Selection_Mode = false;

                                        Application_Operational_Controller.Controller(Application_Operational_Controller.Application_Operations.Item_Search);
                                    }
                                }
                                else if (File_Rename_Mode == true)
                                {
                                    Error = String.Empty;

                                    lock (selection_buffer)
                                    {
                                        current_input.Clear();
                                        current_input.Append("");

                                        File_Rename_Mode = false;
                                        Selection_Mode = false;

                                        Application_Operational_Controller.Controller(Application_Operational_Controller.Application_Operations.Rename_File);
                                    }
                                }
                                else if (File_Relocation_Mode == true)
                                {
                                    Error = String.Empty;

                                    lock (selection_buffer)
                                    {
                                        current_input.Clear();
                                        current_input.Append("");

                                        File_Relocation_Mode = false;
                                        Selection_Mode = false;

                                        Application_Operational_Controller.Controller(Application_Operational_Controller.Application_Operations.Move_File);
                                    }
                                }
                                break;

                            default:
                                Error = String.Empty;
                                lock(selection_buffer)
                                {
                                    selection_buffer += cki.KeyChar;
                                }
                                Application_Operational_Controller.Controller(Application_Operational_Controller.Application_Operations.Change_Location);
                                break;
                        }
                    }

                    Empty_STDIN_Buffered_Stream();

                }
                catch { }
            } 
            while (true);
        }

        private static void Size_change_detection_timer_Elapsed(object? sender, System.Timers.ElapsedEventArgs e)
        {
            if (Current_Buffer_Width != Console.BufferWidth)
            {
                Current_Buffer_Width = Console.BufferWidth;
                Application_Operational_Controller.Controller(Application_Operational_Controller.Application_Operations.Redraw_Window);
            }
            else if(Current_Buffer_Height != Console.WindowHeight)
            {
                Current_Buffer_Height = Console.WindowHeight;
                Application_Operational_Controller.Controller(Application_Operational_Controller.Application_Operations.Redraw_Window);
            }
        }

        private static void Empty_STDIN_Buffered_Stream()
        {   
            if(System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.Windows))
            {
                while (Console.KeyAvailable)
                {
                    Console.ReadKey(true);
                }
            }
            else if (System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.Linux))
            {
                if(Selection_Mode == false)
                {
                    while (Console.KeyAvailable)
                    {
                        Console.ReadKey(true);
                    }
                }
            }

        }

        ~Program()
        {
            Console.Clear();
            Console.CursorVisible = true;
            System.Environment.Exit(0);
        }
    }
}
    