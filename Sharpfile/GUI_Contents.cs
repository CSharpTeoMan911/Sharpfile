using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sharpfile
{
    internal class GUI_Contents
    {
        public static void Main_Menu_GUI()
        {

        }

        public static void Help_GUI()
        {

        }


        public static void Print_Current_Directory_Contents(string content, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            Console.WriteLine(content);
            Console.ForegroundColor = Program.Default_Console_Color;
        }
    }
}
