using System.Diagnostics;
using System.Text;
using TextCopy;

namespace Sharpfile
{
    class Program
    {
        public static List<Tuple<string, string, string, ConsoleColor>> current_directory = new List<Tuple<string, string, string, ConsoleColor>>();
        public static int current_index = 0;
        public static int cursor_location = 0;
        public static int start_index = 0;

        public static ConsoleColor Default_Console_Color = Console.ForegroundColor;
        public static ConsoleColor Default_Console_Background_Color = Console.BackgroundColor;

        public static string Current_Directory = Directory.GetCurrentDirectory();
        public static string Previous_Directory = Directory.GetCurrentDirectory();
        //public static string Current_Directory = "/mnt/c/Users/Teodor Mihail/source/repos/Sharpfile/Sharpfile/bin/Debug/net6.0";
        //public static string Current_Directory = "C:\\";
        public static int Current_Buffer_Width = 0;
        public static int Current_Buffer_Height = 0;
        public static StringBuilder current_input = new StringBuilder();
        public static string location_buffer = String.Empty;
        public static bool Location_Selection_Mode = false;
        private static bool Location_Selection_Initiated = false;

        public static void Main()
        {
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

            List<ConsoleModifiers> modifiers_list = new List<ConsoleModifiers>();

            ConsoleKeyInfo cki = new ConsoleKeyInfo();
            int previous_index = current_index;
            Console.Clear();
            Application_Operational_Controller.Controller(Application_Operational_Controller.Application_Operations.Redraw_Window_And_Load_Window);
            do
            {
                try
                {
                    Console.CursorVisible = false;
                    cki = Console.ReadKey(true);


                    if (Location_Selection_Mode == false)
                    {
                        switch (cki.Key)
                        {
                            case ConsoleKey.UpArrow:
                                current_input.Clear();
                                if (current_index > 0)
                                {
                                    current_index--;
                                    cursor_location--;
                                }

                                switch (cursor_location < 0)
                                {
                                    case true:
                                        cursor_location = Console.WindowHeight - 7;
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
                                current_input.Clear();
                                if (current_index < current_directory.Count - 1)
                                {
                                    current_index++;
                                    cursor_location++;
                                }

                                if (current_index != previous_index)
                                {
                                    previous_index = current_index;

                                    switch (cursor_location > Console.WindowHeight - 7)
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
                                }
                                break;
                            case ConsoleKey.O:
                                current_input.Clear();
                                current_input.Append(" OPEN FILE");
                                Application_Operational_Controller.Controller(Application_Operational_Controller.Application_Operations.Open_Files);
                                break;
                            case ConsoleKey.B:
                                current_input.Clear();
                                current_input.Append(" GO BACK");
                                current_index = 0;
                                cursor_location = 0;
                                start_index = 0;
                                Application_Operational_Controller.Controller(Application_Operational_Controller.Application_Operations.Go_Back);
                                break;
                            case ConsoleKey.R:
                                current_input.Clear();
                                current_input.Append(" REFRESH");
                                Application_Operational_Controller.Controller(Application_Operational_Controller.Application_Operations.Redraw_Window);
                                break;
                            case ConsoleKey.L:
                                Location_Selection_Initiated = true;
                                Location_Selection_Mode = true;
                                current_input.Append(" LOCATION SELECTION ( PRESS 'CTR + E' TO EXIT )");
                                Application_Operational_Controller.Controller(Application_Operational_Controller.Application_Operations.Change_Location);
                                break;
                            default:
                                current_input.Clear();
                                current_input.Append(" N/A");
                                Application_Operational_Controller.Controller(Application_Operational_Controller.Application_Operations.Redraw_Window_And_Load_Window);
                                break;
                        }
                    }
                    else
                    {

                        if (Location_Selection_Initiated == true)
                        {
                            Location_Selection_Initiated = false;
                            location_buffer = String.Empty;
                        }


                        switch(cki.Key)
                        {
                            case ConsoleKey.E:
                                if (cki.Modifiers == ConsoleModifiers.Control)
                                {
                                    Application_Operational_Controller.Controller(Application_Operational_Controller.Application_Operations.Redraw_Window_And_Load_Window);
                                    current_input.Clear();
                                    current_input.Append("");
                                    Location_Selection_Mode = false;
                                }
                                break;

                            case ConsoleKey.P:
                                if (cki.Modifiers == ConsoleModifiers.Control)
                                {
                                    location_buffer = ClipboardService.GetText();
                                    System.Diagnostics.Debug.WriteLine("Paste: " + location_buffer);
                                    Application_Operational_Controller.Controller(Application_Operational_Controller.Application_Operations.Change_Location);
                                }
                                break;

                            case ConsoleKey.Backspace:
                                if(location_buffer.Length > 0)
                                {
                                    location_buffer = location_buffer.Substring(0, location_buffer.Length - 1);
                                }
                                Application_Operational_Controller.Controller(Application_Operational_Controller.Application_Operations.Change_Location);
                                break;

                            case ConsoleKey.Enter:

                                if(System.IO.Directory.Exists(location_buffer) == true)
                                {
                                    Current_Directory = location_buffer;
                                    Application_Operational_Controller.Controller(Application_Operational_Controller.Application_Operations.Navigate_To_Directory);
                                    Application_Operational_Controller.Controller(Application_Operational_Controller.Application_Operations.Redraw_Window_And_Load_Window);
                                    current_input.Clear();
                                    current_input.Append("");
                                    Location_Selection_Mode = false;
                                }

                                break;

                            default:
                                location_buffer += cki.KeyChar;
                                Application_Operational_Controller.Controller(Application_Operational_Controller.Application_Operations.Change_Location);
                                break;
                        }

                        System.Diagnostics.Debug.WriteLine("Keys: "+ location_buffer.ToString());
                    }

                    while (Console.KeyAvailable)
                    {
                        Console.ReadKey(true);
                    }

                }
                catch { }
            } 
            while (cki.Key != ConsoleKey.Escape);
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
    }
}
    