using System.Collections.Generic;
using System.Collections.ObjectModel;
using WikiUpload.Properties;

namespace WikiUpload
{
    internal class ApplicationColorThemes : ReadOnlyCollection<ColorTheme>
    {
        public ApplicationColorThemes() : base(
            new List<ColorTheme> {
               new ColorTheme(Skin.PurpleOverload, Resources.PutpleOverloadText),
               new ColorTheme(Skin.PurpleHaze, Resources.PutpleHazeText),
               new ColorTheme(Skin.GreenForest, Resources.GreenForestText),
               new ColorTheme(Skin.BlueLight, Resources.BlueLightText),
               new ColorTheme(Skin.Solarized, Resources.SolarizedText),
               new ColorTheme(Skin.Rakdos, Resources.RakdosText),
               new ColorTheme(Skin.MidnightLights, Resources.MidnightLightsText),
               new ColorTheme(Skin.PlainGray, Resources.PlainGray),
               new ColorTheme(Skin.Psi, "Psi"),
            })
        {
        }
    }
}
