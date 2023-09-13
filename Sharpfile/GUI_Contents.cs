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
    internal class GUI_Contents
    {
        private static string pointer = " >  ";
        private static string separator = "|";
        private static string File_Name_Label = "Name: ";
        private static string Permissions_Label = "Permissions ";
        private static string Type_Label = "Type: ";
        private static string current_directory_label = "   Location: ";
        private static string directory_creation_label = "   Directory name: ";
        private static int current_row_count = 0;
        private static int current_unit;
        private static string previous_error = String.Empty;

        public static StringBuilder previous_line = new StringBuilder();
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
                String whitespace = new String(' ', Console.WindowWidth);
                Console.SetCursorPosition(0, 0);
                for (int y = 0; y < Console.BufferWidth; y++)
                {
                    Console.Write(whitespace);
                }
                Console.SetCursorPosition(0, 0);
            }

            return Task.FromResult(true);
        }


        public static async Task<bool> Redraw_Screen()
        {
            current_row_count = 0;

            await Clear_Console();

            int small_width = (Console.BufferWidth / 10) * 3;
            int large_width = (Console.BufferWidth / 10) * 4;
            int end_index = Program.start_index + (Console.WindowHeight - 7);

            switch(Program.Location_Selection_Mode)
            {
                case true:
                    await Print_Location_Section(Program.Current_Directory);
                    break;

                case false:
                    await Print_Location_Section(Program.Current_Directory);
                    break;
            }
            Print_Current_Directory_Contents(Program.start_index, end_index, small_width, large_width);
            await Print_Command_Section(Program.current_directory.Count);

            Console.SetCursorPosition(0, 0);
            Console.CursorVisible = false;
            return true;
        }

        public static async void Redraw_File_Unit()
        {
            current_row_count = 0;

            if(Console.GetCursorPosition().Left == 0 && Console.GetCursorPosition().Top == 0)
            {
                if (location_line.Length > Console.WindowWidth)
                {
                    Console.SetCursorPosition(0, 0);
                    await Print_Location_Section(Program.Current_Directory);
                }

                Console.SetCursorPosition(0, 3);

                if(Console.GetCursorPosition().Left == 0 && Console.GetCursorPosition().Top == 3)
                {
                    int small_width = (Console.WindowWidth / 10) * 3;
                    int large_width = (Console.WindowWidth / 10) * 4;
                    int end_index = Program.start_index + (Console.WindowHeight - 7);

                    for (int i = Program.start_index; i < end_index; i++)
                    {
                        if (i < Program.current_directory.Count)
                        {
                            await Draw_Line(i, small_width, large_width);
                        }
                        else
                        {
                            break;
                        }
                    }
                }
            }
            
            Console.SetCursorPosition(0, 0);
            Console.CursorVisible = false;
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
                else if (Program.Directory_Creation_Mode == true)
                {
                    current_label = directory_creation_label;
                }
                else
                {
                    current_label = current_directory_label;
                }

                Console.SetCursorPosition(0, 0);
                int end = Console.WindowWidth;

                for (int i = 0; i < end; i++)
                {
                    Console.Write('_');
                }


                for (int i = 0; i < end; i++)
                {
                    if (i < 3)
                    {
                        Console.Write(' ');
                    }
                    else if (i == 3)
                    {
                        Console.Write(current_label);
                        location_line.Append(current_label);
                        Console.ForegroundColor = ConsoleColor.DarkYellow;

                        for (int ii = 0; ii < Console.WindowWidth - current_label.Length - 6; ii++)
                        {
                            if (ii > directory.Length - 1)
                            {
                                break;
                            }

                            if (ii < Console.WindowWidth - current_label.Length - 9)
                            {
                                Console.Write(directory[ii]);
                                location_line.Append(directory[ii]);
                            }
                            else
                            {
                                Console.Write('.');
                                location_line.Append('.');
                            }
                        }
                        Console.ForegroundColor = Program.Default_Console_Color;
                    }
                    else if (i > 3)
                    {
                        end = Console.BufferWidth - current_label.Length - directory.Length;
                        Console.Write(' ');
                    }
                }

                Console.Write('\n');

                for (int i = 0; i < Console.BufferWidth; i++)
                {
                    Console.Write('_');
                }
            }
            
            return Task.FromResult(true);
        }


        private static Task<bool> Print_Command_Section(int unit)
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
                    Console.SetCursorPosition(0, current_row_count + 3);
                    current_unit = unit;

                    if (unit < Console.WindowHeight - 10)
                    {
                        int count = (Console.WindowHeight - 10 - unit) + 3;

                        for (int i = 0; i < count; i++)
                        {
                            Console.WriteLine(new String(' ', Console.WindowWidth));
                        }
                    }



                    for (int i = 0; i < Console.BufferWidth; i++)
                    {
                        Console.Write('_');
                    }

                    Console.Write("Input: " + Program.current_input.ToString());

                    if (Program.Error != String.Empty)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.Write(" " + Program.Error + " ");
                        Console.ForegroundColor = Program.Default_Console_Color;
                    }

                    for (int i = 0; i < previous_error.Length + 1; i++)
                    {
                        Console.Write(' ');
                    }

                    previous_error = Program.Error;
                    Console.Write("\n");

                    for (int i = 0; i < Console.BufferWidth; i++)
                    {
                        Console.Write('_');
                    }
                }
            }

            return Task.FromResult(true);
        }



        public static Task<bool> Draw_Line(int i, int small_width, int large_width)
        {
            current_row_count++;
            int limiter = 0;

            lock(Program.current_directory)
            {
                Tuple<string, string, string, ConsoleColor> tuple = Program.current_directory[i];

                Console.ForegroundColor = Program.Default_Console_Color;

                if (i == Program.current_index)
                {
                    Console.Write(pointer);
                    limiter = pointer.Length;
                }

                Console.ForegroundColor = tuple.Item4;
                Console.Write(' ');

                int count = 0;

                for (int ii = 0; ii < small_width - limiter - Permissions_Label.Length - tuple.Item1.Length - 2; ii++)
                {
                    if (tuple.Item1.Length > small_width - limiter - Permissions_Label.Length - 2)
                    {
                        if (ii < small_width - limiter - Permissions_Label.Length - 5)
                        {
                            Console.Write(Permissions_Label[ii]);
                            count++;
                        }
                        else
                        {
                            Console.Write('.');
                            count++;
                        }
                    }
                    else
                    {
                        if (ii < Permissions_Label.Length)
                        {
                            Console.Write(Permissions_Label[ii]);
                            count++;
                        }
                    }
                }

                if (count == 0)
                {
                    Console.Write("P");
                }

                Console.Write(": " + tuple.Item1 + " ");

                for (int ii = 0; ii < small_width - limiter - Permissions_Label.Length - tuple.Item1.Length - 2; ii++)
                {
                    Console.Write(' ');
                }

                Console.ForegroundColor = Program.Default_Console_Color;
                Console.Write(separator);



                Console.ForegroundColor = tuple.Item4;
                Console.Write(' ');
                Console.Write(File_Name_Label);


                for (int ii = 0; ii < large_width - Permissions_Label.Length - 2; ii++)
                {
                    if (tuple.Item2.Length > large_width - limiter - Permissions_Label.Length - 2)
                    {
                        if (ii < large_width - limiter - Permissions_Label.Length - 5)
                        {
                            Console.Write(tuple.Item2[ii]);
                        }
                        else
                        {
                            Console.Write('.');
                        }
                    }
                    else
                    {
                        if (ii < tuple.Item2.Length)
                        {
                            Console.Write(tuple.Item2[ii]);
                        }
                        else
                        {
                            Console.Write(' ');
                        }
                    }
                }

                Console.ForegroundColor = Program.Default_Console_Color;
                Console.Write(separator);


                Console.ForegroundColor = tuple.Item4;
                Console.Write(' ');
                Console.Write(Type_Label + tuple.Item3);


                for (int ii = 0; ii < small_width - Type_Label.Length - tuple.Item3.Length; ii++)
                {
                    Console.Write(' ');
                }

                Console.Write("    ");
                Console.ForegroundColor = Program.Default_Console_Color;
                Console.Write("\n");
            }

            return Task.FromResult(true);
        }


        public static async void Print_Current_Directory_Contents(int start_index, int end_index, int small_width, int large_width)
        {
            for (int i = start_index; i < end_index; i++)
            {
                if (i < Program.current_directory.Count)
                {
                    await Draw_Line(i, small_width, large_width);
                }
                else
                {
                    break;
                }
            }
        }


        public static async void Location_Selection_Menu(string current_location)
        {
            await Print_Location_Section(current_location);
            await Print_Command_Section(current_unit);
            Console.SetCursorPosition(0, 0);
        }
    }
}
