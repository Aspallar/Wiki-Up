using System;

namespace WikiUpload
{
    internal static class StringExtensions
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
                    for (var i = 0; i < array.Length; i++)
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

        public static string CapitalizeFirstLetter(this string str)
        {
            var a = str.ToCharArray();
            a[0] = Char.ToUpper(a[0]);
            return new string(a);
        }
    }
}
