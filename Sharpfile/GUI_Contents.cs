using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.SymbolStore;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Sharpfile
{
    internal class GUI_Contents
    {
        private static string current_directory_label = "   Location: ";
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
                Console.Clear();
            }
            return Task.FromResult(true);
        }

        private static Task<bool> Print_Location_Section()
        {

            int middle_width_point = Program.Current_Buffer_Width / 2;

            for (int i = 0; i < Console.BufferWidth; i++)
            {
                Console.Write('_');
            }

            
            for (int i = 0; i < Console.BufferWidth; i++)
            {
                if (i == 10)
                {
                    Console.Write(current_directory_label);
                    Console.ForegroundColor = ConsoleColor.DarkYellow;
                    Console.Write(Program.Current_Directory);
                    Console.ForegroundColor = Program.Default_Console_Color;
                }
            }

            Console.Write('\n');

            for (int i = 0; i < Console.BufferWidth; i++)
            {
                Console.Write('_');
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

            if (Program.current_directory.Count < Console.WindowHeight - 10)
            {
                int count = (Console.WindowHeight - 10 - Program.current_directory.Count) + 3;

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
            return Task.FromResult(true);
        }




        public static async Task<bool> Print_Current_Directory_Contents()
        {
            await Clear_Console();
            await Print_Location_Section();

            Console.Write('\n');
            Console.Write('\n');

            int start_index = 0;
            int end_index = 0;

            int small_width = (Console.BufferWidth / 10) * 3;
            int large_width = (Console.BufferWidth / 10) * 4;


            if (Program.current_directory.Count > Console.BufferHeight - 10)
            {
                start_index = Program.current_index;
                end_index = (Console.BufferHeight - 10) + start_index;
            }
            else
            {
                start_index = 0;
                end_index = Program.current_directory.Count;
            }


            for (int i = start_index; i < end_index; i++)
            {
                if(i < Program.current_directory.Count)
                {
                    Tuple<string, string, string, ConsoleColor> tuple = Program.current_directory[i];

                    Console.ForegroundColor = Program.Default_Console_Color;

                    if (i == Program.current_index)
                    {
                        Console.Write(" -->  ");
                    }

                    int count = 0;
                    StringBuilder whitespace = new StringBuilder();

                    Console.ForegroundColor = tuple.Item4;
                    Console.Write("Permissions: " + tuple.Item1);

                    Console.ForegroundColor = Program.Default_Console_Color;

                    while(count < (small_width - "Permissions: ".Length) - tuple.Item1.Length)
                    {
                        whitespace.Append(" ");
                        count++;
                    }
                    Console.Write(whitespace.ToString() + " | ");
                    whitespace.Clear();
                    count = 0;

                    Console.ForegroundColor = tuple.Item4;
                    Console.Write("File name: " + tuple.Item2);

                    while (count < large_width - "File name: ".Length - tuple.Item2.Length)
                    {
                        whitespace.Append(" ");
                        count++;
                    }

                    Console.ForegroundColor = Program.Default_Console_Color;
                    Console.Write(whitespace.ToString() + " | ");
                    whitespace.Clear();


                    Console.ForegroundColor = tuple.Item4;
                    Console.Write("Type: " + tuple.Item3);
                    Console.Write('\n');
                }
                else
                {
                    break;
                }
            }
            Console.ForegroundColor = Program.Default_Console_Color;


            await Print_Command_Section();

            return true;
        }
    }
}
