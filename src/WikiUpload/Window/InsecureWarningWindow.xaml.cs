﻿using System.Windows;

namespace WikiUpload
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class InsecureWarningWindow : Window
    {
        public InsecureWarningWindow()
        {
            Owner = Application.Current.MainWindow;
            InitializeComponent();
            DataContext = App.ServiceLocator.InsecureWarningViewModel(this);
        }

        private void Yes_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }
    }
}