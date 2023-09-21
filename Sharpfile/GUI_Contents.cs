﻿using System;
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
        private static string item_search_label = "   Item name: ";
        private static string file_name_label = "   File name: ";
        private static int current_row_count = 0;
        private static int current_unit;
        private static string Input = "Input: ";

        private static int test_counter = 0;
        private static int test_total_counter = 0;

        private static int previous_input_end_index;
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
            lock(Program.operation_started)
            {
                Program.operation_started = "true";
            }

            current_row_count = 0;

            await Clear_Console();

            int small_width = (Console.BufferWidth / 10) * 3;
            int large_width = (Console.BufferWidth / 10) * 4;
            int end_index = Program.start_index + (Console.WindowHeight - 7);

            string dir = String.Empty;
            Program.Directories_Browser.TryPeek(out dir);

            await Print_Location_Section(dir);
            await Print_Current_Directory_Contents(Program.current_index, end_index, small_width, large_width);
            await Print_Command_Section(Program.current_directory.Count);

            Console.SetCursorPosition(0, 0);
            Console.CursorVisible = false;


            lock (Program.operation_started)
            {
                Program.operation_started = "false";
            }
            return true;
        }

        public static async Task<bool> Redraw_File_Unit()
        {
            lock (Program.operation_started)
            {
                Program.operation_started = "true";
            }

            current_row_count = 0;

            if (Console.GetCursorPosition().Left == 0 && Console.GetCursorPosition().Top == 0)
            {
                if (location_line.Length > Console.WindowWidth)
                {
                    Console.SetCursorPosition(0, 0);

                    string dir = String.Empty;
                    Program.Directories_Browser.TryPeek(out dir);

                    await Print_Location_Section(dir);
                }

                Console.SetCursorPosition(0, 3);

                if (Console.GetCursorPosition().Left == 0 && Console.GetCursorPosition().Top == 3)
                {
                    int small_width = (Console.WindowWidth / 10) * 3;
                    int large_width = (Console.WindowWidth / 10) * 4;
                    int end_index = Program.start_index + (Console.WindowHeight - 7);

                    await Print_Current_Directory_Contents(Program.start_index, end_index, small_width, large_width);
                }
            }
            
            Console.SetCursorPosition(0, 0);
            Console.CursorVisible = false;


            lock (Program.operation_started)
            {
                Program.operation_started = "false";
            }

            return true;
        }



        private static Task<bool> Print_Location_Section(string directory)
        {
            lock (Program.operation_started)
            {
                Program.operation_started = "true";
            }

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

            lock (Program.operation_started)
            {
                Program.operation_started = "false";
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

            lock (Program.operation_started)
            {
                Program.operation_started = "true";
            }

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




                    Console.SetCursorPosition(0, Console.WindowHeight - 4);

                    for (int i = 0; i < Console.BufferWidth; i++)
                    {
                        Console.Write('_');
                    }

                    string current_input = Program.current_input.ToString();

                    Console.Write(Input + current_input);

                    if (Program.Error != String.Empty)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.Write(" " + Program.Error + " ");
                        Console.ForegroundColor = Program.Default_Console_Color;
                    }

                    for (int i = 0; i < previous_input_end_index; i++)
                    {
                        Console.Write(' ');
                    }

                    previous_input_end_index = Input.Length + current_input.Length + Program.Error.Length;
                    Console.Write("\n");

                    for (int i = 0; i < Console.BufferWidth; i++)
                    {
                        Console.Write('_');
                    }
                }
            }

            lock (Program.operation_started)
            {
                Program.operation_started = "false";
            }

            return Task.FromResult(true);
        }



        public static Task<bool> Draw_Line(int i, int small_width, int large_width)
        {
            lock (Program.operation_started)
            {
                Program.operation_started = "true";
            }


            current_row_count++;
            int limiter = 0;

            ConsoleColor default_background = Console.BackgroundColor;

            lock (Program.current_directory)
            {
                Tuple<string, string, string, ConsoleColor> tuple = Program.current_directory.ElementAt(i);
                test_counter++;

                Console.ForegroundColor = Program.Default_Console_Color;

                if (i == Program.current_index)
                {
                    Console.BackgroundColor = ConsoleColor.DarkYellow;
                    Console.ForegroundColor = ConsoleColor.Black;
                    Console.Write(pointer);
                    limiter = pointer.Length;
                }
                else
                {
                    Console.ForegroundColor = tuple.Item4;
                }

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

                if (i == Program.current_index)
                {
                    Console.ForegroundColor = ConsoleColor.Black;
                }
                else
                {
                    Console.ForegroundColor = Program.Default_Console_Color;
                }

                Console.Write(separator);


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

                if (i == Program.current_index)
                {
                    Console.ForegroundColor = ConsoleColor.Black;
                }
                else
                {
                    Console.ForegroundColor = Program.Default_Console_Color;
                }
                
                Console.Write(separator);


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
                            }
                        }
                        else
                        {
                            if(dot_count < 3)
                            {
                                Console.Write('.');
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
                        }
                    }
                }

                Console.ForegroundColor = Program.Default_Console_Color;
                Console.BackgroundColor = default_background;

                Console.Write(new String(' ', 8));

                Console.Write("\n");
            }

            lock (Program.operation_started)
            {
                Program.operation_started = "false";
            }

            return Task.FromResult(true);
        }


        public static async Task<bool> Print_Current_Directory_Contents(int start_index, int end_index, int small_width, int large_width)
        {
            lock (Program.operation_started)
            {
                Program.operation_started = "true";
            }

            for (int i = start_index; i < end_index; i++)
            {
                if (i < Program.current_directory.Count)
                {
                    test_total_counter++;
                    await Draw_Line(i, small_width, large_width);
                }
                else
                {
                    break;
                }
            }
            System.Diagnostics.Debug.WriteLine("t: " + test_counter);
            System.Diagnostics.Debug.WriteLine("T: " + test_counter);
            System.Diagnostics.Debug.WriteLine("Total: " + test_total_counter * test_counter);
            test_total_counter = 0;
            test_counter = 0;

            lock (Program.operation_started)
            {
                Program.operation_started = "false";
            }

            return true;
        }


        public static async void Location_Selection_Menu(string current_location)
        {
            lock (Program.operation_started)
            {
                Program.operation_started = "true";
            }

            await Print_Location_Section(current_location);
            await Print_Command_Section(current_unit);
            Console.SetCursorPosition(0, 0);


            lock (Program.operation_started)
            {
                Program.operation_started = "false";
            }
        }
    }
}
