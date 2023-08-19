namespace Sharpfile
{
    class Program:Main_Operational_Controller
    {
        public static ConsoleColor Default_Console_Color = ConsoleColor.White;
        public static string Current_Directory = System.IO.Directory.GetCurrentDirectory();
        public static void Main()
        {
            Default_Console_Color = Console.ForegroundColor;
            Main_Operation();
        }

        public static async void Main_Operation()
        {
            bool r = Initiate_Operation(Operations.List_Files, null).Result;
            Console.ReadLine();
            Main();
        }
    }
}
    