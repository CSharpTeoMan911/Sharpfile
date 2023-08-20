namespace Sharpfile
{
    class Program:Main_Operational_Controller
    {
        public static List<Tuple<string, string, string, ConsoleColor>> previous_directory = new List<Tuple<string, string, string, ConsoleColor>>();
        public static List<Tuple<string, string, string, ConsoleColor>> current_directory = new List<Tuple<string, string, string, ConsoleColor>>();

        public static ConsoleColor Default_Console_Color = ConsoleColor.White;
        public static string Current_Directory = Directory.GetCurrentDirectory();

        public static void Main()
        {
            Default_Console_Color = Console.ForegroundColor;
            Main_Operation();
        }

        public static async void Main_Operation()
        {
            bool r = Initiate_Operation(Operations.List_Files, "").Result;
            GUI_Contents.Print_Current_Directory_Contents();
            Console.ReadLine();
            Main_Operation();
        }
    }
}
    