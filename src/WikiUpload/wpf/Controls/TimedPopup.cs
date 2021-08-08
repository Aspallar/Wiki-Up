using System;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Threading;

namespace WikiUpload
{
    internal class TimedPopup : Popup
    {
        DispatcherTimer _closeTimer;

        protected override void OnOpened(EventArgs e)
        {
            base.OnOpened(e);
            if (_closeTimer == null)
            {
                _closeTimer = new DispatcherTimer();
                _closeTimer.Interval = TimeSpan.FromMilliseconds(Duration);
                _closeTimer.Tick += CloseTimer_Tick;
                _closeTimer.Start();
            }
        }

        private void CloseTimer_Tick(object sender, EventArgs e)
        {
            _closeTimer.Stop();
            _closeTimer = null;
            IsOpen = false;
        }

        public int Duration
        {
            get { return (int)GetValue(DurationProperty); }
            set { SetValue(DurationProperty, value); }
        }

        public static readonly DependencyProperty DurationProperty =
            DependencyProperty.Register(
                nameof(Duration),
                typeof(int),
                typeof(TimedPopup),
                new PropertyMetadata(1000));
    }
}
