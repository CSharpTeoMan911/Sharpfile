using System.Diagnostics;
using System.Text;

namespace Sharpfile
{
    class Program
    {
        public static List<Tuple<string, string, string, ConsoleColor>> current_directory = new List<Tuple<string, string, string, ConsoleColor>>();
        public static int current_index;

        public static List<string> current_page = new List<string>(10);
        public static int current_page_index = 0;

        public static ConsoleColor Default_Console_Color = Console.ForegroundColor;
        public static ConsoleColor Default_Console_Background_Color = Console.BackgroundColor;

        public static string Current_Directory = Directory.GetCurrentDirectory();
        public static string Previous_Directory = Directory.GetCurrentDirectory();
        //public static string Current_Directory = "/mnt/c/Users/Teodor Mihail/source/repos/Sharpfile/Sharpfile/bin/Debug/net6.0";
        //public static string Current_Directory = "C:\\";
        public static int Current_Buffer_Width = 0;
        public static int Current_Buffer_Height = 0;
        public static StringBuilder current_input = new StringBuilder();

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
            Console.Clear();
            Application_Operational_Controller.Controller(Application_Operational_Controller.Application_Operations.Load_Files_And_Render_Files);
            do
            {
                try
                {
                    cki = Console.ReadKey();

                    bool load_files = false;
                    switch(cki.Key)
                    {
                        case ConsoleKey.UpArrow:
                            if(current_index > 0)
                            {
                                current_index--;
                                
                                if(current_page_index > 0)
                                {
                                    current_page_index--;
                                }
                            }

                            if (current_index != previous_index)
                            {
                                previous_index = current_index;
                                Application_Operational_Controller.Controller(Application_Operational_Controller.Application_Operations.Render_Files);
                            }
                            break;
                        case ConsoleKey.DownArrow:
                            if (current_index < current_directory.Count - 1)
                            {
                                current_index++;

                                if (current_page_index < current_page.Count - 1)
                                {
                                    current_page_index++;
                                }
                            }

                            if (current_index != previous_index)
                            {
                                previous_index = current_index;
                                Application_Operational_Controller.Controller(Application_Operational_Controller.Application_Operations.Render_Files);
                            }
                            break;
                        case ConsoleKey.O:
                            Application_Operational_Controller.Controller(Application_Operational_Controller.Application_Operations.Open_Files);
                            break;
                        case ConsoleKey.R:
                            Application_Operational_Controller.Controller(Application_Operational_Controller.Application_Operations.Load_Files_And_Render_Files);
                            break;
                        default:
                            if ((cki.Modifiers & ConsoleModifiers.Control) != 0)
                            {

                            }
                            break;
                    }

                    System.Diagnostics.Debug.WriteLine("Key: " + cki.Key);


                }
                catch { }
            } 
            while (cki.Key != ConsoleKey.Escape);
        }

        private static void Size_change_detection_timer_Elapsed(object? sender, System.Timers.ElapsedEventArgs e)
        {
            if (Current_Buffer_Width != Console.BufferWidth)
            {
                System.Diagnostics.Debug.WriteLine("Width");
                Current_Buffer_Width = Console.BufferWidth;
                Application_Operational_Controller.Controller(Application_Operational_Controller.Application_Operations.Load_Files_And_Render_Files);
            }
            else if(Current_Buffer_Height != Console.WindowHeight)
            {
                System.Diagnostics.Debug.WriteLine("Height");
                Current_Buffer_Height = Console.WindowHeight;
                Application_Operational_Controller.Controller(Application_Operational_Controller.Application_Operations.Load_Files_And_Render_Files);
            }
        }
    }
}
    