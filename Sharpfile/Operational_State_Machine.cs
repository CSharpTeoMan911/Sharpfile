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
        private static System.Collections.Concurrent.ConcurrentDictionary<string, List<Operation_States>?> operational_states_cache = new System.Collections.Concurrent.ConcurrentDictionary<string, List<Operation_States>?>();

        public enum Operation_States
        {
            File_Deletion_Initiated,
            File_Open_Initiated,
            File_Relocation_Initiated,
            File_Copy_Initiated,
            Directory_Navigation_Initiated
        }

        protected static bool Initiate_State(string process_path, Operation_States state)
        {
            bool operation_permitted = false;

            List<Operation_States>? states = Get_States(process_path); 

            if(states != null)
            {
                if(Verify_State_For_Operation(states, state) == true)
                {
                    List<Operation_States>? updated_states = new List<Operation_States>(states);
                    updated_states.Add(state);

                    operational_states_cache.TryUpdate(process_path, states, updated_states);
                    operation_permitted = true;
                }
            }
            else
            {
                states = new List<Operation_States>() { state };
                operational_states_cache.TryAdd(process_path, states);
                operation_permitted = true;
            }

            return operation_permitted;
        }

        private static bool Verify_State_For_Operation(List<Operation_States> states, Operation_States state)
        {
            bool operation_permitted = false;

            /*
            switch (state)
            {
                case Operation_States.File_Open_Initiated:
                    if(states.Contains(Operation_States.File_Copy_Initiated) == true)
                    {
                        operation_permitted = true;
                    }
                    break;
                case Operation_States.File_Copy_Initiated:
                    if (states.Contains(Operation_States.File_Open_Initiated) == true)
                    {
                        operation_permitted = true;
                    }
                    else if (states.Contains(Operation_States.Directory_Navigation_Initiated) == true)
                    {
                        operation_permitted = true;
                    }
                    break;
                case Operation_States.Directory_Navigation_Initiated:
                    if (states.Contains(Operation_States.File_Copy_Initiated) == true)
                    {
                        operation_permitted = true;
                    }
                    break;
            }
            */

            return operation_permitted;
        }

        protected static void Remove_State(string process_path, Operation_States state)
        {
            List<Operation_States>? states = Get_States(process_path);

            if(states != null)
            {
                List<Operation_States>? updated_states = new List<Operation_States>(states);
                updated_states.Remove(state);

                operational_states_cache.TryUpdate(process_path, states, updated_states);
            }
            else
            {
                operational_states_cache.TryRemove(process_path, out states);
            }
        }

        private static List<Operation_States>? Get_States(string process_path)
        {
            List<Operation_States>? states = null;
            operational_states_cache.TryGetValue(process_path, out states);
            return states;
        }
    }
}
