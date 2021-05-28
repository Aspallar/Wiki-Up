using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace WikiUpload
{
    internal class ApplicationLanguages : ReadOnlyCollection<Language>
    {
        public ApplicationLanguages() : base(
            new List<Language>
            {
                new Language("English", "en-US"),
                new Language("Deutsch (German)", "de-DE"),
                new Language("Eesti (Estonian)", "et-EE"),
                new Language("Français (French)", "fr-FR"),
            })
        {
        }
    }
}
