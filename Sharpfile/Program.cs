using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using TextCopy;

namespace Sharpfile
{
    class Program
    {

        public static ConcurrentBag<Tuple<string, string, string, ConsoleColor>> current_directory = new ConcurrentBag<Tuple<string, string, string, ConsoleColor>>();
        public static string current_directory_permissions = "__";

        public static string operation_started = "false";

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

        public static string Error = String.Empty;


        public static void Main()
        {
            Init();
        }

        public static async void Init()
        {
            await Load_Application_Modules();

            Directories_Browser.Push(Directory.GetCurrentDirectory());
            Console.TreatControlCAsInput = true;
            Console.CursorVisible = false;

            Current_Buffer_Width = Console.BufferWidth;
            Default_Console_Color = Console.ForegroundColor;

            Thread input_thread = new Thread(Read_Input);
            input_thread.Priority = ThreadPriority.Highest;
            input_thread.Start();
        }

        private static async void Read_Input()
        {

            string location_path = String.Empty;
            ConsoleKeyInfo cki = new ConsoleKeyInfo();
            Console.Clear();

            System.Timers.Timer size_change_detection_timer = new System.Timers.Timer();
            size_change_detection_timer.Elapsed += Size_change_detection_timer_Elapsed;
            size_change_detection_timer.Interval = 100;
            size_change_detection_timer.Start();

            Thread.Sleep(2000);

            await Application_Operational_Controller.Controller(Application_Operational_Controller.Application_Operations.Redraw_Window_And_Load_Window);

            do
            {

                try
                {
                    Console.CursorVisible = false;
                     cki = Console.ReadKey(true);

                    lock(operation_started)
                    {
                        async void Execute()
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

                                            await Application_Operational_Controller.Controller(Application_Operational_Controller.Application_Operations.Change_Location);
                                        }

                                        if (current_index > 0)
                                        {
                                            current_index--;
                                            cursor_location--;
                                        }

                                        switch(await Cursor_Position_Calculator())
                                        {
                                            case true:
                                                await Application_Operational_Controller.Controller(Application_Operational_Controller.Application_Operations.Redraw_Window);
                                                break;
                                            case false:
                                                await Application_Operational_Controller.Controller(Application_Operational_Controller.Application_Operations.Redraw_File_Unit);
                                                break;
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

                                            await Application_Operational_Controller.Controller(Application_Operational_Controller.Application_Operations.Change_Location);
                                        }

                                        if (current_index < current_directory.Count - 1)
                                        {
                                            current_index++;
                                            cursor_location++;
                                        }

                                        switch (await Cursor_Position_Calculator())
                                        {
                                            case true:
                                                await Application_Operational_Controller.Controller(Application_Operational_Controller.Application_Operations.Redraw_Window);
                                                break;
                                            case false:
                                                await Application_Operational_Controller.Controller(Application_Operational_Controller.Application_Operations.Redraw_File_Unit);
                                                break;
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
                                            await Application_Operational_Controller.Controller(Application_Operational_Controller.Application_Operations.Open_Files);
                                        }
                                        else
                                        {
                                            Error = "[ Error: No permissions to open the file ]";
                                            await Application_Operational_Controller.Controller(Application_Operational_Controller.Application_Operations.Change_Location);
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
                                        await Application_Operational_Controller.Controller(Application_Operational_Controller.Application_Operations.Go_Back);
                                        break;
                                    case ConsoleKey.C:
                                        lock (Error)
                                        {
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
                                        }
                                        break;
                                    case ConsoleKey.R:

                                        if (cki.Modifiers == (ConsoleModifiers.Control))
                                        {
                                            lock (Error)
                                            {
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

                                            await Application_Operational_Controller.Controller(Application_Operational_Controller.Application_Operations.Change_Location);
                                        }
                                        else
                                        {
                                            current_input.Clear();
                                            current_input.Append(" REFRESH");

                                            await Application_Operational_Controller.Controller(Application_Operational_Controller.Application_Operations.Redraw_Window_And_Load_Window);
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

                                        await Application_Operational_Controller.Controller(Application_Operational_Controller.Application_Operations.Change_Location);
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

                                        await Application_Operational_Controller.Controller(Application_Operational_Controller.Application_Operations.Change_Location);
                                        break;

                                    case ConsoleKey.Delete:
                                        current_input.Clear();
                                        current_input.Append(" FILE DELETION");

                                        if (current_directory.ElementAt(current_index).Item1[0] == 'r' && current_directory.ElementAt(current_index).Item1[1] == 'w')
                                        {
                                            lock (Error)
                                            {
                                                Error = String.Empty;
                                            }
                                            await Application_Operational_Controller.Controller(Application_Operational_Controller.Application_Operations.Delete_File_Or_Directory);
                                        }
                                        else
                                        {
                                            lock (Error)
                                            {
                                                Error = "[ Error: Insufficient permissions ]";
                                            }
                                        }
                                        await Application_Operational_Controller.Controller(Application_Operational_Controller.Application_Operations.Change_Location);
                                        break;

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
                                        await Application_Operational_Controller.Controller(Application_Operational_Controller.Application_Operations.Change_Location);
                                        break;

                                    case ConsoleKey.T:
                                        lock (Error)
                                        {
                                            Error = String.Empty;

                                            current_input.Clear();
                                            current_input.Append(" TERMINAL");
                                        }
                                        await Application_Operational_Controller.Controller(Application_Operational_Controller.Application_Operations.Terminal);
                                        await Application_Operational_Controller.Controller(Application_Operational_Controller.Application_Operations.Change_Location);
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

                                        await Application_Operational_Controller.Controller(Application_Operational_Controller.Application_Operations.Change_Location);
                                        break;

                                    default:
                                        lock (Error)
                                        {
                                            Error = String.Empty;

                                            current_input.Clear();
                                            current_input.Append(String.Empty);
                                        }

                                        await Application_Operational_Controller.Controller(Application_Operational_Controller.Application_Operations.Redraw_Window_And_Load_Window);
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
                                        Directory_Creation_Mode = false;
                                        Item_Search_Mode = false;
                                        File_Rename_Mode = false;
                                        File_Relocation_Mode = false;
                                        Selection_Mode = false;

                                        await Application_Operational_Controller.Controller(Application_Operational_Controller.Application_Operations.Redraw_Window_And_Load_Window);
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
                                            if (selection_buffer != null)
                                            {
                                                lock (selection_buffer)
                                                {
                                                    selection_buffer += cki.KeyChar;
                                                }
                                            }
                                            await Application_Operational_Controller.Controller(Application_Operational_Controller.Application_Operations.Change_Location);
                                        }
                                        break;

                                    case ConsoleKey.V:
                                        if ((cki.Modifiers.ToString() == "Shift, Control"))
                                        {
                                            selection_buffer += await ClipboardService.GetTextAsync();
                                        }
                                        else
                                        {
                                            if (selection_buffer != null)
                                            {
                                                lock (selection_buffer)
                                                {
                                                    selection_buffer += cki.KeyChar;
                                                }
                                            }
                                        }
                                        await Application_Operational_Controller.Controller(Application_Operational_Controller.Application_Operations.Change_Location);
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

                                        await Application_Operational_Controller.Controller(Application_Operational_Controller.Application_Operations.Change_Location);
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
                                                current_input.Append("");

                                                Location_Selection_Mode = false;
                                                Selection_Mode = false;

                                                if (selection_buffer != null)
                                                {
                                                    lock (selection_buffer)
                                                    {
                                                        Directories_Browser.Push(selection_buffer);
                                                    }
                                                }

                                                await Application_Operational_Controller.Controller(Application_Operational_Controller.Application_Operations.Navigate_To_Directory);
                                                await Application_Operational_Controller.Controller(Application_Operational_Controller.Application_Operations.Redraw_Window_And_Load_Window);
                                            }
                                            else
                                            {
                                                lock (Error)
                                                {
                                                    Error = "[ Error: Invalid path ]";
                                                }

                                                await Application_Operational_Controller.Controller(Application_Operational_Controller.Application_Operations.Change_Location);
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
                                                current_input.Append("");

                                                Directory_Creation_Mode = false;
                                                Selection_Mode = false;

                                                await Application_Operational_Controller.Controller(Application_Operational_Controller.Application_Operations.Create_Directory);
                                                await Application_Operational_Controller.Controller(Application_Operational_Controller.Application_Operations.Redraw_Window_And_Load_Window);
                                            }
                                            else
                                            {
                                                lock (Error)
                                                {
                                                    Error = "[ Error: Insufficient permissions ]";
                                                }

                                                await Application_Operational_Controller.Controller(Application_Operational_Controller.Application_Operations.Change_Location);
                                            }
                                        }
                                        else if (Item_Search_Mode == true)
                                        {
                                            lock (Error)
                                            {
                                                Error = String.Empty;
                                                current_input.Clear();
                                                current_input.Append("");

                                                Item_Search_Mode = false;
                                                Selection_Mode = false;
                                            }

                                            await Application_Operational_Controller.Controller(Application_Operational_Controller.Application_Operations.Item_Search);
                                        }
                                        else if (File_Rename_Mode == true)
                                        {
                                            lock (Error)
                                            {
                                                Error = String.Empty;

                                                current_input.Clear();
                                                current_input.Append("");

                                                File_Rename_Mode = false;
                                                Selection_Mode = false;
                                            }

                                            await Application_Operational_Controller.Controller(Application_Operational_Controller.Application_Operations.Rename_File);
                                        }
                                        else if (File_Relocation_Mode == true)
                                        {
                                            lock (Error)
                                            {
                                                Error = String.Empty;

                                                current_input.Clear();
                                                current_input.Append("");

                                                File_Relocation_Mode = false;
                                                Selection_Mode = false;
                                            }

                                            await Application_Operational_Controller.Controller(Application_Operational_Controller.Application_Operations.Move_File);
                                        }
                                        break;

                                    default:
                                        Error = String.Empty;
                                        lock (selection_buffer)
                                        {
                                            selection_buffer += cki.KeyChar;
                                        }
                                        await Application_Operational_Controller.Controller(Application_Operational_Controller.Application_Operations.Change_Location);
                                        break;

                                }
                            }
                        }
                        Execute();

                        Empty_STDIN_Buffered_Stream();
                    }
                }
                catch { }
            } 
            while (true);
        }

        private static void Size_change_detection_timer_Elapsed(object? sender, System.Timers.ElapsedEventArgs e)
        {
            lock (operation_started)
            {
                async void Execute()
                {
                    bool Empty_Buffer = true;

                    if (operation_started == "false")
                    {
                        if (Current_Buffer_Width != Console.BufferWidth)
                        {
                            Current_Buffer_Width = Console.BufferWidth;

                            Thread stdin_buffer_cleanup_thread = new Thread(() =>
                            {
                                while (Empty_Buffer == true)
                                {
                                    Empty_STDIN_Buffered_Stream();
                                }
                            });
                            stdin_buffer_cleanup_thread.Priority = ThreadPriority.BelowNormal;
                            stdin_buffer_cleanup_thread.Start();

                            await Cursor_Position_Calculator();

                            await Application_Operational_Controller.Controller(Application_Operational_Controller.Application_Operations.Redraw_Window);
                            Empty_Buffer = false;
                        }
                        else if (Current_Buffer_Height != Console.WindowHeight)
                        {
                            Current_Buffer_Height = Console.WindowHeight;

                            Thread stdin_buffer_cleanup_thread = new Thread(() =>
                            {
                                while (Empty_Buffer == true)
                                {
                                    Empty_STDIN_Buffered_Stream();
                                }
                            });
                            stdin_buffer_cleanup_thread.Priority = ThreadPriority.BelowNormal;
                            stdin_buffer_cleanup_thread.Start();

                            await Cursor_Position_Calculator();

                            await Application_Operational_Controller.Controller(Application_Operational_Controller.Application_Operations.Redraw_Window);
                            Empty_Buffer = false;
                        }
                    }
                }
                Execute();
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

        public static Task<bool> Cursor_Position_Calculator()
        {
            if(cursor_location < 0)
            {
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
                return Task.FromResult(true);
            }
            else if(cursor_location > Console.WindowHeight - 8)
            {
                cursor_location = 0;
                start_index = current_index;
                return Task.FromResult(true);
            }
            else
            {
                return Task.FromResult(false);
            }
        }


        public async static Task<bool> Recalibrate_Indexes()
        {
            int found_index = Program.current_index;

            current_index = 0;
            start_index = 0;
            cursor_location = 0;

            while (current_index != found_index)
            {
                current_index++;
                cursor_location++;
                await Cursor_Position_Calculator();
            }

            return true;
        }

        private static Task<bool> Load_Application_Modules()
        {
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();

            foreach (Assembly assembly in assemblies)
            {
                foreach (AssemblyName an in assembly.GetReferencedAssemblies())
                {
                    Assembly.Load(an);
                }
            }

            return Task.FromResult(true);
        }

        ~Program()
        {
            current_directory.Clear();
            Directories_Browser.Clear();

            Console.Clear();
            Console.CursorVisible = true;
            System.Environment.Exit(0);
        }
    }
}
    