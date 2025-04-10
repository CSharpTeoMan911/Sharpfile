﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Sharpfile
{
    internal class Extra_Functions
    {
        protected static string Null_Check(string? result) => result == null ? String.Empty : result;

        public static bool Cursor_Position_Calculator()
        {
            if (Program.cursor_location < 0)
            {
                Program.cursor_location = Console.WindowHeight - 8;
                int calculated_value = Program.current_index - Program.cursor_location;
                switch (calculated_value >= 0)
                {
                    case true:
                        Program.start_index = calculated_value;
                        break;
                    case false:
                        Program.start_index = 0;
                        break;
                }
                return true;
            }
            else if (Program.cursor_location > Console.WindowHeight - 8)
            {
                Program.cursor_location = 0;
                Program.start_index = Program.current_index;
                return true;
            }
            else
            {
                return false;
            }
        }


        public static void Recalibrate_Indexes()
        {
            int found_index = Program.current_index;

            Program.current_index = 0;
            Program.start_index = 0;
            Program.cursor_location = 0;

            while (Program.current_index != found_index)
            {
                Program.current_index++;
                Program.cursor_location++;
                Cursor_Position_Calculator();
            }
        }

        protected static bool Load_Application_Modules()
        {
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();

            foreach (Assembly assembly in assemblies)
            {
                foreach (AssemblyName an in assembly.GetReferencedAssemblies())
                {
                    Assembly.Load(an);
                }
            }

            return true;
        }

        protected static void Empty_STDIN_Buffered_Stream()
        {
            if (System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.Windows))
            {
                while (Console.KeyAvailable)
                {
                    Console.ReadKey(true);
                }
            }
            else if (System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.Linux))
            {
                if (Program.Selection_Mode == false)
                {
                    while (Console.KeyAvailable)
                    {
                        Console.ReadKey(true);
                    }
                }
            }

        }
    }
}
