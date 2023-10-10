using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.SymbolStore;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Sharpfile
{
    internal class GUI_Contents:Extra_Functions
    {
        private static string pointer = " >  ";
        private static string separator = "|";
        private static string File_Name_Label = "Name: ";
        private static string Permissions_Label = "Permissions ";
        private static string Type_Label = "Type: ";
        private static string current_directory_label = "   Location: ";
        private static string directory_creation_label = "   Directory name: ";
        private static string item_search_label = "   Item name: ";
        private static string file_name_label = "   File name: ";
        private static string Input = " Input: ";

        public static StringBuilder location_line = new StringBuilder();

        public static void Main_Menu_GUI()
        {
            
        }

        public static void Help_GUI()
        {

        }



        public static Task<bool> Clear_Console()
        {
            if (System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.Linux) == true)
            {
                System.Diagnostics.Process shell_process = new System.Diagnostics.Process();
                shell_process.StartInfo.FileName = "/bin/bash";
                shell_process.StartInfo.Arguments = "-c \"clear\"";
                shell_process.Start();
                shell_process.WaitForExit(500000);
            }
            else if (System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.Windows) == true)
            {
                try
                {
                    String whitespace = new String(' ', Console.WindowWidth);

                    if (Console.WindowWidth >= 1 && Console.WindowHeight >= 1)
                    {
                        Console.SetCursorPosition(0, 0);
                        while (Console.CursorLeft != 0 && Console.CursorTop != 0)
                        {
                            // !!! WAIT FOR CURSOR TO REACH THE SPECIFIED POSITION !!!
                        }
                    }

                    for (int y = 0; y < Console.WindowWidth; y++)
                    {
                        Console.Write(whitespace);
                    }

                    if (Console.WindowWidth >= 1 && Console.WindowHeight >= 1)
                    {
                        Console.SetCursorPosition(0, 0);
                        while (Console.CursorLeft != 0 && Console.CursorTop != 0)
                        {
                            // !!! WAIT FOR CURSOR TO REACH THE SPECIFIED POSITION !!!
                        }
                    }
                }
                catch 
                {
                    return Task.FromResult(false);
                }
            }

            return Task.FromResult(true);
        }


        public static Task<bool> Redraw_Screen()
        {
            lock(Program.operation_started)
            {
                Program.operation_started = "true";

                async void Execution()
                {
                    try
                    {
                        bool clear_successful = await Clear_Console();

                        if(clear_successful == true)
                        {

                            int small_width = (Console.WindowWidth / 10) * 3;
                            int large_width = (Console.WindowWidth / 10) * 4;

                            Program.Recalibrate_Indexes();

                            int end_index = Program.start_index + (Console.WindowHeight - 7);

                            string? dir = String.Empty;
                            Program.Directories_Browser.TryPeek(out dir);

                            if (Console.WindowWidth >= 1 && Console.WindowHeight >= 1)
                            {
                                Console.SetCursorPosition(0, 0);
                                while (Console.CursorLeft != 0 && Console.CursorTop != 0)
                                {
                                    // !!! WAIT FOR CURSOR TO REACH THE SPECIFIED POSITION !!!
                                }
                            }

                            await Print_Location_Section(Null_Check(dir));
                            await Print_Current_Directory_Contents(Program.start_index, end_index, small_width, large_width);
                            await Print_Command_Section();

                            if (Console.WindowWidth >= 1 && Console.WindowHeight >= 1)
                            {
                                Console.SetCursorPosition(0, 0);
                                while (Console.CursorLeft != 0 && Console.CursorTop != 0)
                                {
                                    // !!! WAIT FOR CURSOR TO REACH THE SPECIFIED POSITION !!!
                                }
                            }

                            Console.CursorVisible = false;
                        }
                    }
                    catch { }
                }

                Execution();

                Program.operation_started = "false";
            }

            return Task.FromResult(true);
        }

        public static Task<bool> Redraw_File_Unit()
        {
            lock (Program.operation_started)
            {
                Program.operation_started = "true";

                async void Execution()
                {
                    try
                    {
                        if (location_line.Length > Console.WindowWidth)
                        {
                            if(Console.WindowWidth >= 1 && Console.WindowHeight >= 1)
                            {
                                Console.SetCursorPosition(0, 0);
                                while (Console.CursorLeft != 0 && Console.CursorTop != 0)
                                {
                                    // !!! WAIT FOR CURSOR TO REACH THE SPECIFIED POSITION !!!
                                }
                            }

                            string? dir = String.Empty;
                            Program.Directories_Browser.TryPeek(out dir);

                            await Print_Location_Section(Null_Check(dir));
                        }

                        if (Console.WindowWidth >= 1 && Console.WindowHeight >= 4)
                        {
                            Console.SetCursorPosition(0, 3);
                            while (Console.CursorLeft != 0 && Console.CursorTop != 3)
                            {
                                // !!! WAIT FOR CURSOR TO REACH THE SPECIFIED POSITION !!!
                            }

                            if (Console.GetCursorPosition().Left == 0 && Console.GetCursorPosition().Top == 3)
                            {
                                int small_width = (Console.WindowWidth / 10) * 3;
                                int large_width = (Console.WindowWidth / 10) * 4;

                                Program.Recalibrate_Indexes();

                                int end_index = Program.start_index + (Console.WindowHeight - 7);

                                await Print_Current_Directory_Contents(Program.start_index, end_index, small_width, large_width);
                            }

                            if (Console.WindowWidth >= 1 && Console.WindowHeight >= 1)
                            {
                                Console.SetCursorPosition(0, 0);
                                while (Console.CursorLeft != 0 && Console.CursorTop != 0)
                                {
                                    // !!! WAIT FOR CURSOR TO REACH THE SPECIFIED POSITION !!!
                                }
                            }

                            Console.CursorVisible = false;
                        }
                    }
                    catch { }
                }

                Execution();

                Program.operation_started = "false";
            }

            return Task.FromResult(true);
        }



        private static Task<bool> Print_Location_Section(string directory)
        {

            string current_label = String.Empty;

            lock(directory)
            {
                if (Program.Directory_Creation_Mode == true)
                {
                    current_label = directory_creation_label;
                }
                else if (Program.Item_Search_Mode == true)
                {
                    current_label = item_search_label;
                }
                else if (Program.File_Rename_Mode == true)
                {
                    current_label = file_name_label;
                }
                else
                {
                    current_label = current_directory_label;
                }

                Console.ForegroundColor = ConsoleColor.Black;
                Console.BackgroundColor = ConsoleColor.DarkGreen;

                int end = Console.WindowWidth;


                Console.Write(new String(' ', Console.WindowWidth + 3));


                Console.Write(current_label);
                location_line.Append(current_label);

                int cursor_left = 0;

                for (int ii = 0; ii < Console.WindowWidth - current_label.Length - 6; ii++)
                {
                    if (ii > directory.Length - 1)
                    {
                        break;
                    }

                    if (ii < Console.WindowWidth - current_label.Length - 9)
                    {
                        cursor_left++;
                        Console.Write(directory[ii]);
                        location_line.Append(directory[ii]);
                    }
                    else
                    {
                        cursor_left++;
                        Console.Write('.');
                        location_line.Append('.');
                    }
                }

                int width = Console.WindowWidth - current_label.Length - cursor_left - 6;

                if(width > 0)
                {
                    Console.Write(new String(' ', width));
                }

                Console.Write(new String(' ', Console.WindowWidth + 3));

                Console.ForegroundColor = Program.Default_Console_Color;
                Console.BackgroundColor = Program.Default_Console_Background_Color;

            }

            return Task.FromResult(true);
        }


        private static Task<bool> Print_Command_Section()
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
            // ctrl + c = exit application

            lock (Program.current_input)
            {
                lock (Program.Error)
                {
                    int cursor_left = 0;

                    Console.BackgroundColor = ConsoleColor.DarkGreen;
                    Console.ForegroundColor = ConsoleColor.Black;

                    Console.Write(new String(' ', Console.WindowWidth));

                    string current_input = Program.current_input.ToString();


                    Console.Write(Input + current_input);
                    cursor_left += Input.Length + current_input.Length;
                    

                    if (Program.Error != String.Empty)
                    {
                        Console.ForegroundColor = ConsoleColor.DarkRed;
                        Console.Write(" ");
                        Console.Write(Program.Error);
                        Console.Write(" ");
                        Console.ForegroundColor = ConsoleColor.Green;
                        cursor_left += Program.Error.Length + 2;
                    }

                    int width = Console.WindowWidth - cursor_left;

                    if(width > 0)
                    {
                        Console.Write(new String(' ', Console.WindowWidth - cursor_left));
                    }

                    Console.Write(new String(' ', Console.WindowWidth));
                    Console.Write(new String(' ', Console.WindowWidth));

                    Console.ForegroundColor = Program.Default_Console_Color;
                    Console.BackgroundColor = Program.Default_Console_Background_Color;
                }
            }

            return Task.FromResult(true);
        }



        public static Task<bool> Draw_Line(int i, int small_width, int large_width)
        {
            int cursor_left = 0;
            int limiter = 0;

            ConsoleColor default_background = Console.BackgroundColor;

            lock (Program.current_directory)
            {
                Tuple<string, string, string, ConsoleColor> tuple = Program.current_directory.ElementAt(i);

                Console.ForegroundColor = Program.Default_Console_Color;

                if (i == Program.current_index)
                {
                    Console.BackgroundColor = ConsoleColor.Green;
                    Console.ForegroundColor = ConsoleColor.Black;
                    Console.Write(pointer);
                    cursor_left += pointer.Length;
                    limiter = pointer.Length;
                }
                else
                {
                    Console.BackgroundColor = ConsoleColor.Black;
                    Console.ForegroundColor = tuple.Item4;
                }

                Console.Write(' ');
                cursor_left += 1;


                int count = 0;
                int width = 0;

                for (int ii = 0; ii < small_width - limiter - Permissions_Label.Length - tuple.Item1.Length - 2; ii++)
                {
                    if (tuple.Item1.Length > small_width - limiter - Permissions_Label.Length - 2)
                    {
                        if (ii < small_width - limiter - Permissions_Label.Length - 5)
                        {
                            Console.Write(Permissions_Label[ii]);
                            cursor_left += 1;
                            count++;
                        }
                        else
                        {
                            Console.Write('.');
                            cursor_left += 1;
                            count++;
                        }
                    }
                    else
                    {
                        if (ii < Permissions_Label.Length)
                        {
                            Console.Write(Permissions_Label[ii]);
                            cursor_left += 1;
                            count++;
                        }
                    }
                }

                if (count == 0)
                {
                    Console.Write("P");
                    cursor_left += 1;
                }

                Console.Write(": ");
                Console.Write(tuple.Item1);
                Console.Write(" ");

                cursor_left += (3 + tuple.Item1.Length);
                width = small_width - limiter - Permissions_Label.Length - tuple.Item1.Length - 2;

                if (width > 0)
                {
                    cursor_left += width;
                    Console.Write(new String(' ', small_width - limiter - Permissions_Label.Length - tuple.Item1.Length - 2));
                }

                if (i == Program.current_index)
                {
                    Console.ForegroundColor = ConsoleColor.Black;
                }
                else
                {
                    Console.ForegroundColor = Program.Default_Console_Color;
                }

                Console.Write(separator);
                cursor_left += separator.Length;


                if (i == Program.current_index)
                {
                    Console.ForegroundColor = ConsoleColor.Black;
                }
                else
                {
                    Console.ForegroundColor = tuple.Item4;
                }
                
                Console.Write(' ');
                Console.Write(File_Name_Label);

                cursor_left += (1 + File_Name_Label.Length);


                for (int ii = 0; ii < large_width - Permissions_Label.Length - 2; ii++)
                {
                    if (tuple.Item2.Length > large_width - limiter - Permissions_Label.Length - 2)
                    {
                        if (ii < large_width - limiter - Permissions_Label.Length - 5)
                        {
                            Console.Write(tuple.Item2[ii]);
                            cursor_left += 1;
                        }
                        else
                        {
                            Console.Write('.');
                            cursor_left += 1;
                        }
                    }
                    else
                    {
                        if (ii < tuple.Item2.Length)
                        {
                            Console.Write(tuple.Item2[ii]);
                            cursor_left += 1;
                        }
                        else
                        {
                            Console.Write(' ');
                            cursor_left += 1;
                        }
                    }
                }

                if (i == Program.current_index)
                {
                    Console.ForegroundColor = ConsoleColor.Black;
                }
                else
                {
                    Console.ForegroundColor = Program.Default_Console_Color;
                }
                
                Console.Write(separator);
                cursor_left += separator.Length;


                if (i == Program.current_index)
                {
                    Console.ForegroundColor = ConsoleColor.Black;
                }
                else
                {
                    Console.ForegroundColor = tuple.Item4;
                }

                Console.Write(' ');
                Console.Write(Type_Label);

                cursor_left += (1 + Type_Label.Length);

                int dot_count = 0;

                for (int ii = 0; ii < small_width - Type_Label.Length - 2; ii++)
                {
                    if (tuple.Item3.Length > small_width - 8 - Type_Label.Length - 2)
                    {
                        if (ii < small_width - 8 - Type_Label.Length - 8)
                        {
                            if(ii < tuple.Item3.Length - 3)
                            {
                                Console.Write(tuple.Item3[ii]);
                                cursor_left += 1;
                            }
                        }
                        else
                        {
                            if(dot_count < 3)
                            {
                                Console.Write('.');
                                cursor_left += 1;
                                dot_count++;
                            }
                            else
                            {
                                break;
                            }
                        }
                    }
                    else
                    {
                        if (ii < tuple.Item3.Length)
                        {
                            Console.Write(tuple.Item3[ii]);
                            cursor_left += 1;
                        }
                    }
                }

                width = Console.WindowWidth - cursor_left;

                if (width > 0)
                {
                    Console.Write(new String(' ', width));
                }

                Console.ForegroundColor = Program.Default_Console_Color;
                Console.BackgroundColor = Program.Default_Console_Background_Color;
            }

            return Task.FromResult(true);
        }


        public static async Task<bool> Print_Current_Directory_Contents(int start_index, int end_index, int small_width, int large_width)
        {

            int cursor_top = 0;

            for (int i = start_index; i < end_index; i++)
            {
                if (i < Program.current_directory.Count)
                {
                    cursor_top++;
                    await Draw_Line(i, small_width, large_width);
                }
                else
                {
                    break;
                }
            }

            Console.BackgroundColor = ConsoleColor.Black;

            while (cursor_top < Console.WindowHeight - 7)
            {
                Console.Write(new String(' ', Console.WindowWidth));
                cursor_top++;
            }

            Console.BackgroundColor = Program.Default_Console_Background_Color;

            return true;
        }


        public static void Location_Selection_Menu(string current_location)
        {
            lock (Program.operation_started)
            {
                Program.operation_started = "true";

                async void Execution()
                {
                    Console.SetCursorPosition(0, 0);
                    while (Console.CursorLeft != 0 && Console.CursorTop != 0)
                    {
                        // !!! WAIT FOR CURSOR TO REACH THE SPECIFIED POSITION !!!
                    }

                    await Print_Location_Section(current_location);

                    Console.SetCursorPosition(0, Console.WindowHeight - 4);
                    while (Console.CursorLeft != 0 && Console.CursorTop != Console.WindowHeight - 4)
                    {
                        // !!! WAIT FOR CURSOR TO REACH THE SPECIFIED POSITION !!!
                    }

                    await Print_Command_Section();


                    Console.SetCursorPosition(0, 0);
                    while (Console.CursorLeft != 0 && Console.CursorTop != 0)
                    {
                        // !!! WAIT FOR CURSOR TO REACH THE SPECIFIED POSITION !!!
                    }
                }

                Execution();

                Program.operation_started = "false";
            }
        }
    }
}
