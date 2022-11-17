using MahApps.Metro.IconPacks;
using System.Windows;
using System.Windows.Controls;

namespace WikiUpload
{
    internal partial class SecrionHeader : UserControl
    {
        public SecrionHeader()
        {
            InitializeComponent();
            DataContext = this;
        }

        #region Text Property

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register(
                nameof(Text),
                typeof(string),
                typeof(SecrionHeader));

        #endregion

        #region IconKind Property

        public PackIconFontAwesomeKind IconKind
        {
            get { return (PackIconFontAwesomeKind)GetValue(IconKindProperty); }
            set { SetValue(IconKindProperty, value); }
        }

        public static readonly DependencyProperty IconKindProperty =
            DependencyProperty.Register(
                nameof(IconKind),
                typeof(PackIconFontAwesomeKind),
                typeof(SecrionHeader));

        #endregion

        #region Subtext Property

        public string Subtext
        {
            get { return (string)GetValue(SubtextProperty); }
            set { SetValue(SubtextProperty, value); }
        }

        public static readonly DependencyProperty SubtextProperty =
            DependencyProperty.Register(
                nameof(Subtext),
                typeof(string),
                typeof(SecrionHeader));

        #endregion
    }
}
