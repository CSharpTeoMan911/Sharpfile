using Microsoft.Extensions.Internal;
using Microsoft.Extensions.Options;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sharpfile
{
    internal class Operational_State_Machine
    {
        private static System.Collections.Concurrent.ConcurrentDictionary<Tuple<string, int>, List<Operation_States>?> operational_states_cache = new System.Collections.Concurrent.ConcurrentDictionary<Tuple<string, int>, List<Operation_States>?>();

        public enum Operation_States
        {
            File_Deletion_Initiated,
            File_Open_Initiated,
            File_Relocation_Initiated,
            File_Copy_Initiated,
            Directory_Navigation_Initiated
        }

        protected static void Initiate_State(Tuple<string, int> process, Operation_States state)
        {
        }

        protected static bool Verify_State_For_Operation(List<Operation_States>? states, Operation_States state)
        {
            bool operation_permitted = false;

            return operation_permitted;
        }

        protected static void Remove_State(Tuple<string, int> process, Operation_States state)
        {
        }

        private static List<Operation_States>? Get_States(Tuple<string, int> process)
        {
            List<Operation_States>? states = null;
            operational_states_cache.TryGetValue(process, out states);
            return states;
        }
    }
}
