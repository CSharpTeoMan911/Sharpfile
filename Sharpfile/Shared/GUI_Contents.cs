using System.Text;

namespace Sharpfile.Shared
{
    internal class GUI_Contents:Extra_Functions
    {
        private static readonly string pointer = " >  ";
        private static readonly string separator = "|";
        private static readonly string File_Name_Label = "Name: ";
        private static readonly string Permissions_Label = "Permissions ";
        private static readonly string Type_Label = "Type: ";
        private static readonly string current_directory_label = "   Location: ";
        private static readonly string directory_creation_label = "   Directory name: ";
        private static readonly string item_search_label = "   Item name: ";
        private static readonly string file_name_label = "   File name: ";
        private static readonly string Input = " Input: ";

        public static StringBuilder location_line = new StringBuilder();

        public static void Main_Menu_GUI()
        {
            
        }

        public static void Help_GUI()
        {

        }


        public static async Task Redraw_Screen()
        {
            try
            {
                Console.Clear();

                int small_width = Console.WindowWidth / 10 * 3;
                int large_width = Console.WindowWidth / 10 * 4;

                Recalibrate_Indexes();

                int end_index = 
                    
                    
                    
                    
                    
                    
                    start_index + (Console.WindowHeight - 7);

                string? dir = string.Empty;
                Directories_Browser.TryPeek(out dir);

                if (Console.WindowWidth >= 1 && Console.WindowHeight >= 1)
                {
                    Console.SetCursorPosition(0, 0);
                    while (Console.CursorLeft != 0 && Console.CursorTop != 0)
                    {
                        // !!! WAIT FOR CURSOR TO REACH THE SPECIFIED POSITION !!!
                    }
                }


                await Print_Location_Section(Null_Check(dir));
                await Print_Current_Directory_Contents(start_index, end_index, small_width, large_width);
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
            catch(Exception e) 
            {
                await Console.Out.WriteLineAsync(e.Message);
            }
        }

        public static async Task Redraw_File_Unit()
        {
            try
            {

                if (location_line.Length > Console.WindowWidth)
                {
                    if (Console.WindowWidth >= 1 && Console.WindowHeight >= 1)
                    {
                        Console.SetCursorPosition(0, 0);
                        while (Console.CursorLeft != 0 && Console.CursorTop != 0)
                        {
                            // !!! WAIT FOR CURSOR TO REACH THE SPECIFIED POSITION !!!
                        }
                    }

                    string? dir = string.Empty;
                    Directories_Browser.TryPeek(out dir);

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
                        int small_width = Console.WindowWidth / 10 * 3;
                        int large_width = Console.WindowWidth / 10 * 4;

                        Recalibrate_Indexes();

                        int end_index = start_index + (Console.WindowHeight - 7);

                        await Print_Current_Directory_Contents(start_index, end_index, small_width, large_width);
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



        private static async Task Print_Location_Section(string directory)
        {

            string current_label = string.Empty;

            if (Directory_Creation_Mode == true)
            {
                current_label = directory_creation_label;
            }
            else if (Item_Search_Mode == true)
            {
                current_label = item_search_label;
            }
            else if (File_Rename_Mode == true)
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


            await Console.Out.WriteAsync(new string(' ', Console.WindowWidth + 3));


            await Console.Out.WriteAsync(current_label);
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
                    await Console.Out.WriteAsync(directory[ii]);
                    location_line.Append(directory[ii]);
                }
                else
                {
                    cursor_left++;
                    await Console.Out.WriteAsync('.');
                    location_line.Append('.');
                }
            }

            int width = Console.WindowWidth - current_label.Length - cursor_left - 6;

            if (width > 0)
            {
                await Console.Out.WriteAsync(new string(' ', width));
            }

            await Console.Out.WriteAsync(new string(' ', Console.WindowWidth + 3));

            Console.ForegroundColor = Default_Console_Color;
            Console.BackgroundColor = Default_Console_Background_Color;
        }


        private static async Task Print_Command_Section()
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

            int cursor_left = 0;

            Console.BackgroundColor = ConsoleColor.DarkGreen;
            Console.ForegroundColor = ConsoleColor.Black;

            await Console.Out.WriteAsync(new string(' ', Console.WindowWidth));

            string selected_current_input = current_input.ToString();


            await Console.Out.WriteAsync(Input + selected_current_input);
            cursor_left += Input.Length + selected_current_input.Length;


            if (Error != string.Empty)
            {
                Console.ForegroundColor = ConsoleColor.DarkRed;
                await Console.Out.WriteAsync(" ");
                await Console.Out.WriteAsync(Error);
                await Console.Out.WriteAsync(" ");
                Console.ForegroundColor = ConsoleColor.Green;
                cursor_left += Error.Length + 2;
            }

            int width = Console.WindowWidth - cursor_left;

            if (width > 0)
            {
                await Console.Out.WriteAsync(new string(' ', Console.WindowWidth - cursor_left));
            }

            await Console.Out.WriteAsync(new string(' ', Console.WindowWidth));
            await Console.Out.WriteAsync(new string(' ', Console.WindowWidth));

            Console.ForegroundColor = Default_Console_Color;
            Console.BackgroundColor = Default_Console_Background_Color;
        }



        public static async Task Draw_Line(int i, int small_width, int large_width)
        {
            int cursor_left = 0;
            int limiter = 0;

            ConsoleColor default_background = Console.BackgroundColor;

            Tuple<string, string, string, ConsoleColor> tuple = current_directory.ElementAt(i);

            Console.ForegroundColor = Default_Console_Color;

            if (i == current_index)
            {
                Console.BackgroundColor = ConsoleColor.Green;
                Console.ForegroundColor = ConsoleColor.Black;
                await Console.Out.WriteAsync(pointer);
                cursor_left += pointer.Length;
                limiter = pointer.Length;
            }
            else
            {
                Console.BackgroundColor = ConsoleColor.Black;
                Console.ForegroundColor = tuple.Item4;
            }

            await Console.Out.WriteAsync(' ');
            cursor_left += 1;


            int count = 0;
            int width = 0;

            for (int ii = 0; ii < small_width - limiter - Permissions_Label.Length - tuple.Item1.Length - 2; ii++)
            {
                if (tuple.Item1.Length > small_width - limiter - Permissions_Label.Length - 2)
                {
                    if (ii < small_width - limiter - Permissions_Label.Length - 5)
                    {
                        await Console.Out.WriteAsync(Permissions_Label[ii]);
                        cursor_left += 1;
                        count++;
                    }
                    else
                    {
                        await Console.Out.WriteAsync('.');
                        cursor_left += 1;
                        count++;
                    }
                }
                else
                {
                    if (ii < Permissions_Label.Length)
                    {
                        await Console.Out.WriteAsync(Permissions_Label[ii]);
                        cursor_left += 1;
                        count++;
                    }
                }
            }

            if (count == 0)
            {
                await Console.Out.WriteAsync("P");
                cursor_left += 1;
            }

            await Console.Out.WriteAsync(": ");
            await Console.Out.WriteAsync(tuple.Item1);
            await Console.Out.WriteAsync(" ");

            cursor_left += 3 + tuple.Item1.Length;
            width = small_width - limiter - Permissions_Label.Length - tuple.Item1.Length - 2;

            if (width > 0)
            {
                cursor_left += width;
                await Console.Out.WriteAsync(new string(' ', small_width - limiter - Permissions_Label.Length - tuple.Item1.Length - 2));
            }

            if (i == current_index)
            {
                Console.ForegroundColor = ConsoleColor.Black;
            }
            else
            {
                Console.ForegroundColor = Default_Console_Color;
            }

            await Console.Out.WriteAsync(separator);
            cursor_left += separator.Length;


            if (i == current_index)
            {
                Console.ForegroundColor = ConsoleColor.Black;
            }
            else
            {
                Console.ForegroundColor = tuple.Item4;
            }

            await Console.Out.WriteAsync(' ');
            await Console.Out.WriteAsync(File_Name_Label);

            cursor_left += 1 + File_Name_Label.Length;


            for (int ii = 0; ii < large_width - Permissions_Label.Length - 2; ii++)
            {
                if (tuple.Item2.Length > large_width - limiter - Permissions_Label.Length - 2)
                {
                    if (ii < large_width - limiter - Permissions_Label.Length - 5)
                    {
                        await Console.Out.WriteAsync(tuple.Item2[ii]);
                        cursor_left += 1;
                    }
                    else
                    {
                        await Console.Out.WriteAsync('.');
                        cursor_left += 1;
                    }
                }
                else
                {
                    if (ii < tuple.Item2.Length)
                    {
                        await Console.Out.WriteAsync(tuple.Item2[ii]);
                        cursor_left += 1;
                    }
                    else
                    {
                        await Console.Out.WriteAsync(' ');
                        cursor_left += 1;
                    }
                }
            }

            if (i == current_index)
            {
                Console.ForegroundColor = ConsoleColor.Black;
            }
            else
            {
                Console.ForegroundColor = Default_Console_Color;
            }

            await Console.Out.WriteAsync(separator);
            cursor_left += separator.Length;


            if (i == current_index)
            {
                Console.ForegroundColor = ConsoleColor.Black;
            }
            else
            {
                Console.ForegroundColor = tuple.Item4;
            }

            await Console.Out.WriteAsync(' ');
            await Console.Out.WriteAsync(Type_Label);

            cursor_left += 1 + Type_Label.Length;

            int dot_count = 0;

            for (int ii = 0; ii < small_width - Type_Label.Length - 2; ii++)
            {
                if (tuple.Item3.Length > small_width - 8 - Type_Label.Length - 2)
                {
                    if (ii < small_width - 8 - Type_Label.Length - 8)
                    {
                        if (ii < tuple.Item3.Length - 3)
                        {
                            await Console.Out.WriteAsync(tuple.Item3[ii]);
                            cursor_left += 1;
                        }
                    }
                    else
                    {
                        if (dot_count < 3)
                        {
                            await Console.Out.WriteAsync('.');
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
                        await Console.Out.WriteAsync(tuple.Item3[ii]);
                        cursor_left += 1;
                    }
                }
            }

            width = Console.WindowWidth - cursor_left;

            if (width > 0)
            {
                await Console.Out.WriteAsync(new string(' ', width));
            }

            Console.ForegroundColor = Default_Console_Color;
            Console.BackgroundColor = Default_Console_Background_Color;
        }


        public static async Task Print_Current_Directory_Contents(int start_index, int end_index, int small_width, int large_width)
        {

            int cursor_top = 0;

            for (int i = start_index; i < end_index; i++)
            {
                if (i < current_directory.Count)
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
                await Console.Out.WriteAsync(new string(' ', Console.WindowWidth));
                cursor_top++;
            }

            Console.BackgroundColor = Default_Console_Background_Color;
        }


        public static async Task Location_Selection(string current_location)
        {
            Console.SetCursorPosition(0, 0);

            // !!! WAIT FOR CURSOR TO REACH THE SPECIFIED POSITION !!!
            while (Console.CursorLeft != 0 && Console.CursorTop != 0);

            await Print_Location_Section(current_location);

            Console.SetCursorPosition(0, Console.WindowHeight - 4);

            // !!! WAIT FOR CURSOR TO REACH THE SPECIFIED POSITION !!!
            while (Console.CursorLeft != 0 && Console.CursorTop != Console.WindowHeight - 4);

            await Print_Command_Section();

            Console.SetCursorPosition(0, 0);

            // !!! WAIT FOR CURSOR TO REACH THE SPECIFIED POSITION !!!
            while (Console.CursorLeft != 0 && Console.CursorTop != 0);
        }
    }
}
