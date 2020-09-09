using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace WikiUpload.Extensions
{
    public static class StringExtensions
    {
        public static bool IsEquivalentTo(this string str, char[] array)
        {
            if (str.Length != array.Length)
            {
                return false;
            }
            else
            {
                var strArray = str.ToCharArray();
                try
                {
                    for (int i = 0; i < array.Length; i++)
                    {
                        if (strArray[i] != array[i])
                            return false;
                    }
                }
                finally
                {
                    Array.Clear(strArray, 0, strArray.Length);
                }
            }
            return true;
        }
    }
}
