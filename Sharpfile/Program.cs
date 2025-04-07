using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using TextCopy;

namespace Sharpfile
{
    class Program:Extra_Functions
    {
        private static DateTime last_change = DateTime.Now;


        private static bool enable_execute = false;

        public static List<Func<Task>> controller_gui_operations = new List<Func<Task>>();
        public static ConcurrentQueue<Task> controller_operations = new ConcurrentQueue<Task>();


        public static ConcurrentBag<Tuple<string, string, string, ConsoleColor>> current_directory = new ConcurrentBag<Tuple<string, string, string, ConsoleColor>>();
        public static string current_directory_permissions = "__";


        public static int current_index = 0;
        public static int cursor_location = 0;
        public static int start_index = 0;


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
        public static bool File_Copy_Mode = false;
        public static bool Delete_Mode = false;

        public static string Error = String.Empty;


        public static void Main()
        {
            Load_Application_Modules();

            Directories_Browser.Push(Directory.GetCurrentDirectory());
            Console.TreatControlCAsInput = true;
            Console.CursorVisible = false;

            Current_Buffer_Width = Console.BufferWidth;
            Default_Console_Color = Console.ForegroundColor;

            Read_Input().Wait();
        }

        private static async Task Read_Input()
        {
            Action? action;

            string? location_path = String.Empty;
            ConsoleKeyInfo cki = new ConsoleKeyInfo();

            await Application_Operational_Controller.Controller(Application_Operational_Controller.Application_Operations.Redraw_Window_And_Load_Window);

            System.Timers.Timer gui_management = new System.Timers.Timer();
            gui_management.Interval = 1;
            gui_management.Elapsed += Gui_management_Elapsed;
            gui_management.Start();

            System.Timers.Timer size_management = new System.Timers.Timer();
            size_management.Interval = 1;
            size_management.Elapsed += Size_change_detection;
            size_management.Start();

            while (true)
            {
                Console.CursorVisible = false;
                cki = Console.ReadKey(true);

                if (enable_execute)
                {
                    try
                    {
                        if (Selection_Mode == false)
                        {
                            switch (cki.Key)
                            {
                                case ConsoleKey.UpArrow:
                                    if (current_input.ToString() != String.Empty)
                                    {
                                        lock (Error)
                                        {
                                            Error = String.Empty;
                                            current_input.Clear();
                                            current_input.Append(String.Empty);
                                        }

                                        action = async () => await Application_Operational_Controller.Controller(Application_Operational_Controller.Application_Operations.Change_Location);
                                        action.Invoke();
                                    }

                                    if (current_index > 0)
                                    {
                                        current_index--;
                                        cursor_location--;
                                    }

                                    lock (controller_gui_operations)
                                    {
                                        switch (Cursor_Position_Calculator())
                                        {
                                            case true:
                                                controller_gui_operations.Add(() => Application_Operational_Controller.Controller(Application_Operational_Controller.Application_Operations.Redraw_Window));
                                                break;
                                            case false:
                                                controller_gui_operations.Add(() => Application_Operational_Controller.Controller(Application_Operational_Controller.Application_Operations.Redraw_File_Unit));
                                                break;
                                        }
                                    }
                                    break;
                                case ConsoleKey.DownArrow:
                                    if (current_input.ToString() != String.Empty)
                                    {
                                        lock (Error)
                                        {
                                            Error = String.Empty;
                                            current_input.Clear();
                                            current_input.Append(String.Empty);
                                        }

                                        action = async () => await Application_Operational_Controller.Controller(Application_Operational_Controller.Application_Operations.Change_Location);
                                        action.Invoke();
                                    }

                                    if (current_index < current_directory.Count - 1)
                                    {
                                        current_index++;
                                        cursor_location++;
                                    }

                                    lock (controller_gui_operations)
                                    {
                                        switch (Cursor_Position_Calculator())
                                        {
                                            case true:

                                                controller_gui_operations.Add(() => Application_Operational_Controller.Controller(Application_Operational_Controller.Application_Operations.Redraw_Window));
                                                break;
                                            case false:
                                                controller_gui_operations.Add(() => Application_Operational_Controller.Controller(Application_Operational_Controller.Application_Operations.Redraw_File_Unit));
                                                break;
                                        }
                                    }
                                    break;
                                case ConsoleKey.O:
                                    lock (Error)
                                    {
                                        Error = String.Empty;
                                        current_input.Clear();
                                        current_input.Append(" OPEN FILE");
                                    }

                                    if (current_directory.ElementAt(current_index).Item1[0] == 'r')
                                    {
                                        action = async () => await Application_Operational_Controller.Controller(Application_Operational_Controller.Application_Operations.Open_Files);
                                        action.Invoke();
                                    }
                                    else
                                    {
                                        Error = "[ Error: No permissions to open the file ]";
                                        action = async () => await Application_Operational_Controller.Controller(Application_Operational_Controller.Application_Operations.Change_Location);
                                        action.Invoke();
                                    }
                                    break;

                                case ConsoleKey.RightArrow:
                                    lock (Error)
                                    {
                                        Error = String.Empty;
                                        current_input.Clear();
                                        current_input.Append(" OPEN FILE");
                                    }

                                    if (current_directory.ElementAt(current_index).Item1[0] == 'r')
                                    {
                                        action = async () => await Application_Operational_Controller.Controller(Application_Operational_Controller.Application_Operations.Open_Files);
                                        action.Invoke();
                                    }
                                    else
                                    {
                                        Error = "[ Error: No permissions to open the file ]";
                                        action = async () => await Application_Operational_Controller.Controller(Application_Operational_Controller.Application_Operations.Change_Location);
                                        action.Invoke();
                                    }
                                    break;

                                case ConsoleKey.B:
                                    lock (Error)
                                    {
                                        Error = String.Empty;
                                        current_input.Clear();
                                        current_input.Append(" GO BACK");
                                    }

                                    current_index = 0;
                                    cursor_location = 0;
                                    start_index = 0;

                                    action = async () => await Application_Operational_Controller.Controller(Application_Operational_Controller.Application_Operations.Go_Back);
                                    action.Invoke();

                                    break;

                                case ConsoleKey.LeftArrow:
                                    lock (Error)
                                    {
                                        Error = String.Empty;
                                        current_input.Clear();
                                        current_input.Append(" GO BACK");
                                    }

                                    current_index = 0;
                                    cursor_location = 0;
                                    start_index = 0;

                                    action = async () => await Application_Operational_Controller.Controller(Application_Operational_Controller.Application_Operations.Go_Back);
                                    action.Invoke();

                                    break;

                                case ConsoleKey.C:
                                    lock (Error)
                                    {
                                        Error = String.Empty;
                                        if (cki.Modifiers == ConsoleModifiers.Control)
                                        {
                                            Console.Clear();
                                            Console.CursorVisible = true;
                                            System.Environment.Exit(0);
                                        }
                                        else
                                        {
                                            Error = String.Empty;

                                            current_input.Clear();
                                            current_input.Append(" FILE COPY ( PRESS 'Esc' TO EXIT )");

                                            Selection_Mode = true;
                                            File_Copy_Mode = true;

                                            location_path = String.Empty;
                                            Directories_Browser.TryPeek(out location_path);

                                            if (selection_buffer != null)
                                            {
                                                lock (selection_buffer)
                                                {
                                                    selection_buffer = location_path;
                                                }
                                            }
                                        }
                                    }

                                    action = async () => await Application_Operational_Controller.Controller(Application_Operational_Controller.Application_Operations.Redraw_Window);
                                    action.Invoke();

                                    break;
                                case ConsoleKey.R:

                                    if (cki.Modifiers == (ConsoleModifiers.Control))
                                    {
                                        lock (Error)
                                        {
                                            Console.CursorVisible = true;

                                            Error = String.Empty;

                                            if (current_directory.ElementAt(current_index).Item1[0] == 'r' && current_directory.ElementAt(current_index).Item1[1] == 'w')
                                            {
                                                Selection_Mode = true;
                                                File_Rename_Mode = true;

                                                if (selection_buffer != null)
                                                {
                                                    lock (selection_buffer)
                                                    {
                                                        selection_buffer = current_directory.ElementAt(current_index).Item2;
                                                    }
                                                }

                                                current_input.Clear();
                                                current_input.Append(" FILE RENAME ( PRESS 'Esc' TO EXIT )");
                                            }
                                            else
                                            {
                                                current_input.Append(" FILE RENAME ");
                                                Error = "[ Error: Insufficient permissions ]";
                                            }
                                        }

                                        action = async () => await Application_Operational_Controller.Controller(Application_Operational_Controller.Application_Operations.Change_Location);
                                        action.Invoke();
                                    }
                                    else
                                    {
                                        current_input.Clear();
                                        current_input.Append(" REFRESH");

                                        lock (controller_gui_operations)
                                            controller_gui_operations.Add(() => Application_Operational_Controller.Controller(Application_Operational_Controller.Application_Operations.Redraw_Window_And_Load_Window));
                                    }
                                    break;
                                case ConsoleKey.L:
                                    lock (Error)
                                    {
                                        Error = String.Empty;

                                        Selection_Mode = true;
                                        Location_Selection_Mode = true;

                                        location_path = String.Empty;
                                        Directories_Browser.TryPeek(out location_path);

                                        if (selection_buffer != null)
                                        {
                                            lock (selection_buffer)
                                            {
                                                selection_buffer = location_path;
                                            }
                                        }

                                        current_input.Clear();
                                        current_input.Append(" LOCATION SELECTION ( PRESS 'Esc' TO EXIT )");
                                    }

                                    action = async () => await Application_Operational_Controller.Controller(Application_Operational_Controller.Application_Operations.Change_Location);
                                    action.Invoke();

                                    break;

                                case ConsoleKey.D:
                                    lock (Error)
                                    {
                                        if (cki.Modifiers == ConsoleModifiers.Control)
                                        {
                                            current_input.Clear();
                                            current_input.Append(" FILE DELETION");

                                            if (current_directory.ElementAt(current_index).Item1[0] == 'r' && current_directory.ElementAt(current_index).Item1[1] == 'w')
                                            {
                                                Error = String.Empty;
                                                Selection_Mode = true;
                                                Delete_Mode = true;
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

                                            if (selection_buffer != null)
                                            {
                                                lock (selection_buffer)
                                                {
                                                    selection_buffer = String.Empty;
                                                }
                                            }

                                            current_input.Clear();
                                            current_input.Append(" DIRECTORY CREATION ( PRESS 'Esc' TO EXIT )");
                                        }
                                    }

                                    action = async () => await Application_Operational_Controller.Controller(Application_Operational_Controller.Application_Operations.Change_Location);
                                    action.Invoke();

                                    break;

                                case ConsoleKey.Delete:
                                    current_input.Clear();
                                    current_input.Append(" FILE DELETION");

                                    if (current_directory.ElementAt(current_index).Item1[0] == 'r' && current_directory.ElementAt(current_index).Item1[1] == 'w')
                                    {
                                        lock (Error)
                                        {
                                            Error = String.Empty;
                                            Selection_Mode = true;
                                            Delete_Mode = true;
                                        }

                                        action = async () => await Application_Operational_Controller.Controller(Application_Operational_Controller.Application_Operations.Delete_File_Or_Directory);
                                        action.Invoke();
                                    }
                                    else
                                    {
                                        lock (Error)
                                        {
                                            Error = "[ Error: Insufficient permissions ]";
                                        }
                                    }

                                    action = async () => await Application_Operational_Controller.Controller(Application_Operational_Controller.Application_Operations.Change_Location);
                                    action.Invoke();
                                    break;




                                //////////////////////////////////////////
                                //          !!!! TO DO !!!!             //
                                //////////////////////////////////////////
                                // Implement the file search operation  //
                                //////////////////////////////////////////
                                case ConsoleKey.S:
                                    lock (Error)
                                    {
                                        Error = String.Empty;

                                        Selection_Mode = true;
                                        Item_Search_Mode = true;

                                        if (selection_buffer != null)
                                        {
                                            lock (selection_buffer)
                                            {
                                                selection_buffer = String.Empty;
                                            }
                                        }

                                        current_input.Clear();
                                        current_input.Append(" ITEM SEARCH ( 'Esc' TO EXIT )");
                                    }
                                    action = async () => await Application_Operational_Controller.Controller(Application_Operational_Controller.Application_Operations.Change_Location);
                                    action.Invoke();
                                    break;

                                case ConsoleKey.T:
                                    lock (Error)
                                    {
                                        Error = String.Empty;

                                        current_input.Clear();
                                        current_input.Append(" TERMINAL");
                                    }

                                    action = async () =>
                                    {
                                        await Application_Operational_Controller.Controller(Application_Operational_Controller.Application_Operations.Terminal);
                                        await Application_Operational_Controller.Controller(Application_Operational_Controller.Application_Operations.Change_Location);
                                    };
                                    action.Invoke();
                                    break;

                                case ConsoleKey.M:
                                    lock (Error)
                                    {
                                        if (current_directory.ElementAt(current_index).Item1[0] == 'r' && current_directory.ElementAt(current_index).Item1[1] == 'w')
                                        {
                                            Error = String.Empty;


                                            Selection_Mode = true;
                                            File_Relocation_Mode = true;

                                            location_path = String.Empty;
                                            Directories_Browser.TryPeek(out location_path);

                                            if (selection_buffer != null)
                                            {
                                                lock (selection_buffer)
                                                {
                                                    selection_buffer = location_path;
                                                }
                                            }

                                            current_input.Clear();
                                            current_input.Append(" FILE RELOCATION ( PRESS 'Esc' TO EXIT )");
                                        }
                                        else
                                        {
                                            current_input.Clear();
                                            current_input.Append(" FILE RELOCATION");
                                            Error = "[ Error: Insufficient permissions ]";
                                        }
                                    }

                                    action = async () => await Application_Operational_Controller.Controller(Application_Operational_Controller.Application_Operations.Change_Location);
                                    action.Invoke();

                                    break;

                                default:
                                    lock (Error)
                                    {
                                        Error = String.Empty;

                                        current_input.Clear();
                                        current_input.Append(String.Empty);
                                    }

                                    lock (controller_gui_operations)
                                        controller_gui_operations.Add(() => Application_Operational_Controller.Controller(Application_Operational_Controller.Application_Operations.Redraw_Window_And_Load_Window));
                                    break;
                            }
                        }
                        else
                        {
                            switch (cki.Key)
                            {
                                case ConsoleKey.Escape:
                                    current_input.Clear();
                                    

                                    lock (controller_gui_operations)
                                        controller_gui_operations.Add(() => Application_Operational_Controller.Controller(Application_Operational_Controller.Application_Operations.Redraw_Window_And_Load_Window));

                                    Location_Selection_Mode = false;
                                    Directory_Creation_Mode = false;
                                    Item_Search_Mode = false;
                                    File_Rename_Mode = false;
                                    File_Relocation_Mode = false;
                                    File_Copy_Mode = false;
                                    Selection_Mode = false;
                                    Delete_Mode = true;
                                    break;

                                case ConsoleKey.Backspace:

                                    lock (Error)
                                    {
                                        Error = String.Empty;

                                        if (selection_buffer != null)
                                        {
                                            lock (selection_buffer)
                                            {
                                                if (selection_buffer.Length > 0)
                                                {
                                                    selection_buffer = selection_buffer.Substring(0, selection_buffer.Length - 1);
                                                }
                                            }
                                        }
                                    }

                                    action = async () => await Application_Operational_Controller.Controller(Application_Operational_Controller.Application_Operations.Change_Location);
                                    action.Invoke();
                                    break;

                                case ConsoleKey.Enter:

                                    if (Location_Selection_Mode == true)
                                    {
                                        lock (Error)
                                        {
                                            Error = String.Empty;
                                        }

                                        if (System.IO.Directory.Exists(selection_buffer) == true)
                                        {
                                            current_input.Clear();
                                            

                                            Directories_Browser.Push(selection_buffer);

                                            action = async () => await Application_Operational_Controller.Controller(Application_Operational_Controller.Application_Operations.Navigate_To_Directory);
                                            action.Invoke();

                                            lock (controller_gui_operations)
                                                controller_gui_operations.Add(() => Application_Operational_Controller.Controller(Application_Operational_Controller.Application_Operations.Redraw_Window_And_Load_Window));
                                        }
                                        else
                                        {
                                            lock (Error)
                                            {
                                                Error = "[ Error: Invalid path ]";
                                            }

                                            action = async () => await Application_Operational_Controller.Controller(Application_Operational_Controller.Application_Operations.Change_Location);
                                            action.Invoke();
                                        }
                                    }
                                    else if (Directory_Creation_Mode == true)
                                    {
                                        lock (Error)
                                        {
                                            Error = String.Empty;
                                        }

                                        location_path = String.Empty;
                                        Directories_Browser.TryPeek(out location_path);

                                        if (current_directory_permissions[1] == 'w')
                                        {
                                            current_input.Clear();
                                            

                                            action = async () => await Application_Operational_Controller.Controller(Application_Operational_Controller.Application_Operations.Create_Directory);
                                            action.Invoke();

                                            lock (controller_gui_operations)
                                                controller_gui_operations.Add(() => Application_Operational_Controller.Controller(Application_Operational_Controller.Application_Operations.Redraw_Window_And_Load_Window));
                                        }
                                        else
                                        {
                                            lock (Error)
                                            {
                                                Error = "[ Error: Insufficient permissions ]";
                                            }

                                            action = async () => await Application_Operational_Controller.Controller(Application_Operational_Controller.Application_Operations.Change_Location);
                                            action.Invoke();

                                        }
                                    }
                                    else if (Item_Search_Mode == true)
                                    {
                                        lock (Error)
                                        {
                                            Error = String.Empty;
                                            current_input.Clear();
                                            
                                        }

                                        action = async () => await Application_Operational_Controller.Controller(Application_Operational_Controller.Application_Operations.Item_Search);
                                        action.Invoke();
                                    }
                                    else if (File_Rename_Mode == true)
                                    {
                                        lock (Error)
                                        {
                                            Error = String.Empty;

                                            current_input.Clear();
                                            
                                        }

                                        action = async () => await Application_Operational_Controller.Controller(Application_Operational_Controller.Application_Operations.Rename_File);
                                        action.Invoke();
                                    }
                                    else if (File_Relocation_Mode == true)
                                    {
                                        lock (Error)
                                        {
                                            Error = String.Empty;

                                            current_input.Clear();
                                            
                                        }

                                        action = async () => await Application_Operational_Controller.Controller(Application_Operational_Controller.Application_Operations.Move_File);
                                        action.Invoke();
                                    }
                                    else if (File_Copy_Mode == true)
                                    {
                                        lock (Error)
                                        {
                                            Error = String.Empty;
                                            current_input.Clear();  
                                        }

                                        action = async () => await Application_Operational_Controller.Controller(Application_Operational_Controller.Application_Operations.Copy_File);
                                        action.Invoke();
                                    }
                                    else if (Delete_Mode == true)
                                    {
                                        lock (Error)
                                        {
                                            Error = String.Empty;
                                            current_input.Clear();
                                        }

                                        action = async () => await Application_Operational_Controller.Controller(Application_Operational_Controller.Application_Operations.Delete_File_Or_Directory);
                                        action.Invoke();
                                    }

                                    Location_Selection_Mode = false;
                                    Directory_Creation_Mode = false;
                                    Item_Search_Mode = false;
                                    File_Rename_Mode = false;
                                    File_Relocation_Mode = false;
                                    File_Copy_Mode = false;
                                    Selection_Mode = false;
                                    Delete_Mode = true;
                                    break;

                                default:
                                    Error = String.Empty;
                                    if (selection_buffer != null)
                                    {
                                        lock (selection_buffer)
                                        {
                                            selection_buffer += cki.KeyChar;
                                        }
                                    }

                                    action = async () => await Application_Operational_Controller.Controller(Application_Operational_Controller.Application_Operations.Change_Location);
                                    action.Invoke();
                                    break;

                            }
                        }

                        Empty_STDIN_Buffered_Stream();
                    }
                    catch { }

                    enable_execute = false;
                }
            }
        }

        private static void Gui_management_Elapsed(object? sender, System.Timers.ElapsedEventArgs e)
        {
            if ((DateTime.Now - last_change).TotalMilliseconds >= 100)
            {
                lock (controller_gui_operations)
                {
                    int count = controller_gui_operations.Count;

                    if (count > 0)
                    {
                        Func<Task>? gui_operation = controller_gui_operations.ElementAt(count - 1);

                        async void Execute()
                        {
                            Cursor_Position_Calculator();

                            if (gui_operation != null)
                                await gui_operation.Invoke();
                        }
                        Execute();

                        controller_gui_operations.RemoveRange(0, count);
                    }
                }

                enable_execute = true;
            }
        }


        private static void Size_change_detection(object? sender, System.Timers.ElapsedEventArgs e)
        {
            if (Current_Buffer_Width != Console.BufferWidth)
            {
                last_change = DateTime.Now;

                Console.Clear();

                Current_Buffer_Width = Console.BufferWidth;

                lock (controller_gui_operations)
                    controller_gui_operations.Add(()=> Application_Operational_Controller.Controller(Application_Operational_Controller.Application_Operations.Redraw_Window));
            }
            else if (Current_Buffer_Height != Console.WindowHeight)
            {
                last_change = DateTime.Now;

                Console.Clear();

                Current_Buffer_Height = Console.WindowHeight;

                lock (controller_gui_operations)
                    controller_gui_operations.Add(()=> Application_Operational_Controller.Controller(Application_Operational_Controller.Application_Operations.Redraw_Window));
            }
        }

        ~Program()
        {
            current_directory.Clear();
            Directories_Browser.Clear();

            Console.CursorVisible = true;
            System.Environment.Exit(0);
        }
    }
}
    