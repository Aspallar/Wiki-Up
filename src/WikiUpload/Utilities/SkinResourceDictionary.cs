using PropertyChanged;
using System;
using System.ComponentModel;
using System.Windows;

namespace WikiUpload
{
    [AddINotifyPropertyChangedInterface]
    public class SkinResourceDictionary : ResourceDictionary, INotifyPropertyChanged
    {
        public SkinResourceDictionary() : base()
        {
            PropertyChanged += UpdateSource;
        }

        private void UpdateSource(object sender, PropertyChangedEventArgs e)
        {
            Uri source = AppSkinResource();
            if (source != null && Source != source)
                Source = source;
        }

        public Uri PurpleHazeSource { get; set; }

        public Uri BlueLightSource { get; set; }

        public Uri PurpleOverloadSource { get; set; }

        public Uri GreenForestSource { get; set; }

        public Uri SolarizedSource { get; set; }

        public Uri RakdosSource { get; set; }

        public Uri MidnightLightsSource { get; set; }

        private Uri AppSkinResource()
        {
            Uri thisSource = null;
            switch (App.Skin)
            {
                case Skin.PurpleHaze:
                    thisSource = PurpleHazeSource;
                    break;
                case Skin.PurpleOverload:
                    thisSource = PurpleOverloadSource;
                    break;
                case Skin.GreenForest:
                    thisSource = GreenForestSource;
                    break;
                case Skin.BlueLight:
                    thisSource = BlueLightSource;
                    break;
                case Skin.Solarized:
                    thisSource = SolarizedSource;
                    break;
                case Skin.Rakdos:
                    thisSource = RakdosSource;
                    break;
                case Skin.MidnightLights:
                    thisSource = MidnightLightsSource;
                    break;
            }
            return thisSource;
        }

        // disable never used warning, Fody will use it
        #pragma warning disable CS0067
        public event PropertyChangedEventHandler PropertyChanged;
        #pragma warning restore CS0067
    }
}
