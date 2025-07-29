using System.Reflection;

namespace Sharpfile.Shared
{
    internal class Extra_Functions
    {
        protected static string Null_Check(string? result) => result == null ? string.Empty : result;

        public static System.Runtime.InteropServices.OSPlatform GetCurrentOs() => 
            System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.Windows) ? 
            System.Runtime.InteropServices.OSPlatform.Windows : 
            System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.Linux) ? 
            System.Runtime.InteropServices.OSPlatform.Linux : 
            System.Runtime.InteropServices.OSPlatform.OSX;

        public static bool Cursor_Position_Calculator()
        {
            if (cursor_location < 0)
            {
                cursor_location = Console.WindowHeight - 8;
                int calculated_value = current_index - cursor_location;
                switch (calculated_value >= 0)
                {
                    case true:
                        start_index = calculated_value;
                        break;
                    case false:
                        start_index = 0;
                        break;
                }
                return true;
            }
            else if (cursor_location > Console.WindowHeight - 8)
            {
                cursor_location = 0;
                start_index = current_index;
                return true;
            }
            else
            {
                return false;
            }
        }


        public static void Recalibrate_Indexes()
        {
            int found_index = current_index;

            current_index = 0;
            start_index = 0;
            cursor_location = 0;

            while (current_index != found_index)
            {
                current_index++;
                cursor_location++;
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
            if (current_os == System.Runtime.InteropServices.OSPlatform.Windows)
            {
                while (Console.KeyAvailable)
                {
                    Console.ReadKey(true);
                }
            }
            else if (current_os == System.Runtime.InteropServices.OSPlatform.Linux)
            {
                if (Selection_Mode == false)
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
