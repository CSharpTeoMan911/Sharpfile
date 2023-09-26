using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sharpfile
{
    internal class Extra_Functions
    {
        protected static string Null_Check(string? result)
        {
            return result == null ? String.Empty : result;
        }
    }
}
