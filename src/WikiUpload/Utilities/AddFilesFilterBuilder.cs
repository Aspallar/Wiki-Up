using System;
using System.Linq;

namespace WikiUpload
{
    public static class AddFilesFilterBuilder
    {
        private static char[] _separator = new char[] { ';' };

        public static string Build(string[] permittedExtensions, string imageExtensionsString)
        {
            const string othersPrefix = "|Other Files|*";
            const string imagesPrefix = "|Image Files|*";

            var imageExtensions = imageExtensionsString.Split(_separator, StringSplitOptions.RemoveEmptyEntries)
                .Select(x => '.' + x).ToList();
            string images, others;

            if (permittedExtensions.Length == 0)
            {
                others = othersPrefix + ".odt;*.ods;*.odp;*.odg;*.odc;*.odf;*.odi;*.odm;*.ogg;*.ogv;*.oga";
                images = imageExtensions.Count > 0 ? imagesPrefix + string.Join(";*", imageExtensions) : string.Empty;
            }
            else
            {
                var imageFiles = permittedExtensions.Intersect(imageExtensions).ToList();
                others = string.Join(";*", permittedExtensions.Except(imageFiles));
                if (others.Length > 0)
                    others = othersPrefix + others;
                images = string.Join(";*", imageFiles);
                if (images.Length > 0)
                    images = imagesPrefix + images;
            }

            return $"{images}{others}|All Files|*.*".Substring(1);
        }
    }
}
