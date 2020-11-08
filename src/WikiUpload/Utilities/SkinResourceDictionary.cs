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

        private void UpdateSource()
        {
            Uri source = AppSkinResource();
            if (source != null && base.Source != source)
                base.Source = source;
        }

        private Uri AppSkinResource()
        {
            Uri source = null;
            switch (App.Skin)
            {
                case Skin.PurpleHaze:
                    source = _purpleHazeSource;
                    break;
                case Skin.PurpleOverload:
                    source = _purpleOverloadSource;
                    break;
                case Skin.GreenForest:
                    source = _greenForestSource;
                    break;
                case Skin.BlueLight:
                    source = _blueLightSource;
                    break;
            }
            return source;
        }
    }
}
