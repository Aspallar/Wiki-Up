using System;
using System.Windows;

namespace WikiUpload
{
    public class SkinResourceDictionary : ResourceDictionary
    {
        private Uri _purpleHazeSource;
        private Uri _purpleOverloadSource;
        private Uri _greenForestSource;
        private Uri _blueLightSource;
        private Uri _solarizedSource;
        private Uri _rakdosSource;

        public Uri PurpleHazeSource
        {
            get { return _purpleHazeSource; }
            set
            {
                _purpleHazeSource = value;
                UpdateSource();
            }
        }

        public Uri BlueLightSource
        {
            get { return _blueLightSource; }
            set
            {
                _blueLightSource = value;
                UpdateSource();
            }
        }

        public Uri PurpleOverloadSource
        {
            get { return _purpleOverloadSource; }
            set
            {
                _purpleOverloadSource = value;
                UpdateSource();
            }
        }

        public Uri GreenForestSource
        {
            get { return _greenForestSource; }
            set
            {
                _greenForestSource = value;
                UpdateSource();
            }
        }

        public Uri SolarizedSource
        {
            get { return _solarizedSource; }
            set
            {
                _solarizedSource = value;
                UpdateSource();
            }
        }

        public Uri RakdosSource
        {
            get { return _rakdosSource; }
            set
            {
                _rakdosSource = value;
                UpdateSource();
            }
        }

        private void UpdateSource()
        {
            Uri source = AppSkinResource();
            if (source != null && base.Source != source)
                base.Source = source;
        }

        private Uri AppSkinResource()
        {
            Uri thisSource = null;
            switch (App.Skin)
            {
                case Skin.PurpleHaze:
                    thisSource = _purpleHazeSource;
                    break;
                case Skin.PurpleOverload:
                    thisSource = _purpleOverloadSource;
                    break;
                case Skin.GreenForest:
                    thisSource = _greenForestSource;
                    break;
                case Skin.BlueLight:
                    thisSource = _blueLightSource;
                    break;
                case Skin.Solarized:
                    thisSource = _solarizedSource;
                    break;
                case Skin.Rakdos:
                    thisSource = _rakdosSource;
                    break;
            }
            return thisSource;
        }
    }
}
