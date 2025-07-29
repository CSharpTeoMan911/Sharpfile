using System.Collections.Concurrent;
using System.Text;

namespace Sharpfile.Shared
{
    class Variables
    {
        public static System.Runtime.InteropServices.OSPlatform current_os;

        public static DateTime last_change = DateTime.UtcNow;

        public static bool enable_execute = false;

        public static List<Func<Task>> controller_gui_operations = new List<Func<Task>>();

        public static ConcurrentBag<Tuple<string, string, string, ConsoleColor>> current_directory = new ConcurrentBag<Tuple<string, string, string, ConsoleColor>>();
        public static string current_directory_permissions = "__";


        public static int current_index = 0;
        public static int cursor_location = 0;
        public static int start_index = 0;


        public static ConsoleColor Default_Console_Color = Console.ForegroundColor;
        public static ConsoleColor Default_Console_Background_Color = Console.BackgroundColor;

        public static ConcurrentStack<string> Directories_Browser = new ConcurrentStack<string>();

        public static int Current_Buffer_Width = 0;
        public static int Current_Buffer_Height = 0;

        public static StringBuilder current_input = new StringBuilder();
        public static string? selection_buffer = string.Empty;


        public static bool Selection_Mode = false;

        public static bool Directory_Creation_Mode = false;
        public static bool Item_Search_Mode = false;
        public static bool File_Rename_Mode = false;
        public static bool Location_Selection_Mode = false;
        public static bool File_Relocation_Mode = false;
        public static bool File_Copy_Mode = false;
        public static bool Delete_Mode = false;

        public static string Error = string.Empty;
    }
}
