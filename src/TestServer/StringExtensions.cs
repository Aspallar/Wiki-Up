using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestServer
{
    public static class StringExtensions
    {
        public static bool ContainsAll(this string s, IList<string> items)
        {
            return items.All(x => s.Contains(x));
        }
    }
}
