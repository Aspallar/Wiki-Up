using System.Text;

namespace WikiUpload
{
    internal static class StringBuilderExtensions
    {
        public static StringBuilder RemoveLastCharacter(this StringBuilder sb)
        {
            if (sb.Length > 0) --sb.Length;
            return sb;
        }

        public static StringBuilder AppendEnclosed(this StringBuilder sb, string text)
            => sb.Append('[').Append(text).Append(']');
    }
}
