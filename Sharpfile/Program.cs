using System.Diagnostics;
using System.Text;

namespace Sharpfile
{
    class Program:Main_Operational_Controller
    {
        public static List<Tuple<string, string, string, ConsoleColor>> previous_directory = new List<Tuple<string, string, string, ConsoleColor>>();
        public static List<Tuple<string, string, string, ConsoleColor>> current_directory = new List<Tuple<string, string, string, ConsoleColor>>();
        public static int current_index;

        public static ConsoleColor Default_Console_Color = Console.ForegroundColor;
        public static ConsoleColor Default_Console_Background_Color = Console.BackgroundColor;

        public static string Current_Directory = Directory.GetCurrentDirectory();
        //public static string Current_Directory = "C:\\";
        public static int Current_Buffer_Width = 0;
        public static int Current_Buffer_Height = 0;
        public static StringBuilder current_input = new StringBuilder();

        public static void Main()
        {
            Current_Buffer_Width = Console.BufferWidth;
            Default_Console_Color = Console.ForegroundColor;

            System.Timers.Timer size_change_detection_timer = new System.Timers.Timer();
            size_change_detection_timer.Elapsed += Size_change_detection_timer_Elapsed;
            size_change_detection_timer.Interval = 500;
            size_change_detection_timer.Start();

            Thread input_thread = new Thread(Read_Input);
            input_thread.Priority = ThreadPriority.Highest;
            input_thread.Start();
        }   

        private static void Read_Input()
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
            Main_Operation(true);
            do
            {
                try
                {
                    
                    cki = Console.ReadKey(true);

                    bool load_files = false;
                    switch(cki.Key)
                    {
                        case ConsoleKey.UpArrow:
                            if(current_index > 0)
                            {
                                current_index--;
                            }
                            break;
                        case ConsoleKey.DownArrow:
                            if (current_index < current_directory.Count - 1)
                            {
                                current_index++;
                            }
                            break;
                        default:
                            if ((cki.Modifiers & ConsoleModifiers.Control) != 0)
                            {

                            }
                            break;
                    }

                    if(current_index != previous_index)
                    {
                        previous_index = current_index;
                        Main_Operation(load_files);
                    }
                }
                catch { }
            } 
            while (cki.Key != ConsoleKey.Escape);
        }

        private static async void Size_change_detection_timer_Elapsed(object? sender, System.Timers.ElapsedEventArgs e)
        {
            if(Current_Buffer_Width != Console.BufferWidth)
            {
                await GUI_Contents.Print_Current_Directory_Contents();
                Current_Buffer_Width = Console.BufferWidth;
            }
            else if(Current_Buffer_Height != Console.WindowHeight)
            {
                await GUI_Contents.Print_Current_Directory_Contents();
                Current_Buffer_Height = Console.WindowHeight;
            }
        }

        public static async void Main_Operation(bool load_files)
        {
            if(load_files == true)
            await Initiate_Operation(Operations.List_Files, "");

            await GUI_Contents.Print_Current_Directory_Contents();
        }
    }
}
    