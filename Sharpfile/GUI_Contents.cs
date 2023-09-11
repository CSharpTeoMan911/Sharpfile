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
        private static string pointer = " -->  ";
        private static string File_Name_Label = "File name: ";
        private static string Permissions_Label = "Permissions: ";
        private static string Type_Label = "Type: ";
        private static string current_directory_label = "   Location: ";
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


        public static void Clear_Line(int cursor_height)
        {
            Console.SetCursorPosition(0, cursor_height);

            for (int i = 0; i < Console.BufferWidth; i++)
            {
                Console.Write(" ");
            }
        }


        public static async void Redraw_Screen()
        {
            await Clear_Console();

            int small_width = (Console.BufferWidth / 10) * 3;
            int large_width = (Console.BufferWidth / 10) * 4;
            int end_index = Program.start_index + (Console.WindowHeight - 10);

            Print_Location_Section();
            Print_Current_Directory_Contents(Program.start_index, end_index, small_width, large_width);
            Print_Command_Section(Program.current_directory.Count);

        }


        public static void Redraw_File_Unit()
        {
            if (location_line.Length > Console.WindowWidth)
            {
                Console.SetCursorPosition(0, 0);
                Print_Location_Section();
            }

            Console.SetCursorPosition(0, 3);

            int small_width = (Console.WindowWidth / 10) * 3;
            int large_width = (Console.WindowWidth / 10) * 4;
            int end_index = Program.start_index + (Console.WindowHeight - 10);

            for (int i = Program.start_index; i < end_index; i++)
            {
                if (i < Program.current_directory.Count)
                {
                    Draw_Line(i, small_width, large_width);
                }
                else
                {
                    break;
                }
            }
            
            Console.SetCursorPosition(0, Console.WindowHeight);

        }



        private static void Print_Location_Section()
        {

            int middle_width_point = Program.Current_Buffer_Width / 2;
            int end = Console.WindowWidth;

            for (int i = 0; i < end; i++)
            {
                Console.Write('_');
            }


            for (int i = 0; i < end; i++)
            {
                if(i < 3)
                {
                    Console.Write(' ');
                }
                else if (i == 3)
                {
                    Console.Write(current_directory_label);
                    location_line.Append(current_directory_label);
                    Console.ForegroundColor = ConsoleColor.DarkYellow;

                    for(int ii = 0; ii < Console.WindowWidth - current_directory_label.Length - 6; ii++)
                    {
                        if(ii > Program.Current_Directory.Length - 1)
                        {
                            break;
                        }

                        if(ii < Console.WindowWidth - current_directory_label.Length - 9)
                        {
                            Console.Write(Program.Current_Directory[ii]);
                            location_line.Append(Program.Current_Directory[ii]);
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
                    end = Console.BufferWidth - current_directory_label.Length - Program.Current_Directory.Length;
                    Console.Write(' ');
                }
            }

            Console.Write('\n');

            for (int i = 0; i < Console.BufferWidth; i++)
            {
                Console.Write('_');
            }

        }


        private static void Print_Command_Section(int unit)
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

            if (unit < Console.WindowHeight - 10)
            {
                int count = (Console.WindowHeight - 10 - unit) + 3;

                for (int i = 0; i < count; i++)
                {
                    Console.WriteLine("");
                }
            }



            for (int i = 0; i < Console.BufferWidth; i++)
            {
                Console.Write('_');
            }

            Console.WriteLine("Input: " + Program.current_input.ToString());

            for (int i = 0; i < Console.BufferWidth; i++)
            {
                Console.Write('_');
            }

        }



        public static void Draw_Line(int i, int small_width, int large_width)
        {


            Tuple<string, string, string, ConsoleColor> tuple = Program.current_directory[i];

            Console.ForegroundColor = Program.Default_Console_Color;

            if (i == Program.current_index)
            {
                Console.Write(pointer);
            }

            int count = 0;

            Console.ForegroundColor = tuple.Item4;
            Console.Write(Permissions_Label + tuple.Item1);

            Console.ForegroundColor = Program.Default_Console_Color;

            while (count < small_width - Permissions_Label.Length - tuple.Item1.Length)
            {
                Console.Write(' ');
                count++;
            }
            Console.Write(" | ");
            count = 0;

            Console.ForegroundColor = tuple.Item4;
            Console.Write(File_Name_Label);

            int file_name_content_length = tuple.Item2.Length;

            switch (File_Name_Label.Length + tuple.Item2.Length > large_width)
            {
                case true:
                    int limit = large_width - File_Name_Label.Length - pointer.Length;

                    if(i == Program.current_index)
                    {
                        limit = large_width - File_Name_Label.Length - pointer.Length;
                    }

                    file_name_content_length = limit;

                    for (int ii = 0; ii < limit; ii++)
                    {
                        switch (ii >= limit - 3)
                        {
                            case true:
                                Console.Write(".");
                                break;
                            case false:
                                Console.Write(tuple.Item2[ii]);
                                break;
                        }
                    }
                    break;
                case false:
                    Console.Write(tuple.Item2);

                    for(int ii = 0; ii < large_width - File_Name_Label.Length - file_name_content_length; ii++)
                    {
                        Console.Write(' ');
                    }
                    break;
            }

            if(i == Program.current_index)
            {
                System.Diagnostics.Debug.WriteLine("L: " + (file_name_content_length));
                System.Diagnostics.Debug.WriteLine("Limit: " + (large_width - File_Name_Label.Length - file_name_content_length));
            }

            Console.ForegroundColor = Program.Default_Console_Color;
            Console.Write(" | ");

            count = 0;

            Console.ForegroundColor = tuple.Item4;

            Console.Write(Type_Label + tuple.Item3);

            while (count < pointer.Length)
            {
                Console.Write(' ');
                count++;
            }

            Console.Write("\n");

            count = 0;

            Console.ForegroundColor = Program.Default_Console_Color;

        }




        public static void Print_Current_Directory_Contents(int start_index, int end_index, int small_width, int large_width)
        {

            for (int i = start_index; i < end_index; i++)
            {
                if (i < Program.current_directory.Count)
                {
                    Draw_Line(i, small_width, large_width);
                }
                else
                {
                    break;
                }
            }
        }
    }
}
