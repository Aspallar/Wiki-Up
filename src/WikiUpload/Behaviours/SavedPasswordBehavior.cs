using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace WikiUpload
{
    public static class SavedPasswordBehavior
    {
        //private static readonly TextChangedEventHandler onTextChanged = new TextChangedEventHandler(OnTextChanged);
        private static readonly RoutedEventHandler onGotFocus = new RoutedEventHandler(OnGotFocus);

        #region SavedPasswordUsername

        public static readonly DependencyProperty SavedPasswordUsername =
            DependencyProperty.RegisterAttached
            (
                "SavedPasswordUsername",
                typeof(string),
                typeof(SavedPasswordBehavior),
                new UIPropertyMetadata(null, OnSavedPasswordUsername)
            );

        public static String GetSavedPasswordUsername(DependencyObject obj)
        {
            return obj.GetValue(SavedPasswordUsername) as String;
        }

        public static void SetSavedPasswordUsername(DependencyObject obj, string value)
        {
            obj.SetValue(SavedPasswordUsername, value);
        }

        private static void OnSavedPasswordUsername(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (!(sender is PasswordBox passwordBox))
                return;

            if (e.OldValue == null)
            {
                passwordBox.GotFocus -= onGotFocus;
                if (e.NewValue != null)
                    passwordBox.GotFocus += onGotFocus;
            }
        }

        #endregion

        #region SavedPasswordSite

        public static readonly DependencyProperty SavedPasswordSite =
            DependencyProperty.RegisterAttached
            (
                "SavedPasswordSite",
                typeof(String),
                typeof(SavedPasswordBehavior),
                new UIPropertyMetadata(String.Empty)
            );

        public static String GetSavedPasswordSite(DependencyObject obj)
        {
            return (String)obj.GetValue(SavedPasswordSite);
        }

        public static void SetSavedPasswordSite(DependencyObject obj, String value)
        {
            obj.SetValue(SavedPasswordSite, value);
        }
        #endregion

        #region SavedPasswordManager

        public static readonly DependencyProperty SavedPasswordManager =
            DependencyProperty.RegisterAttached
            (
                "SavedPasswordManager",
                typeof(IPasswordManager),
                typeof(SavedPasswordBehavior),
                new UIPropertyMetadata(null)
            );

        public static IPasswordManager GetSavedPasswordManager(DependencyObject obj)
        {
            return (IPasswordManager)obj.GetValue(SavedPasswordManager);
        }

        public static void SetSavedPasswordManager(DependencyObject obj, String value)
        {
            obj.SetValue(SavedPasswordManager, value);
        }

        #endregion

        private static void OnGotFocus(object sender, RoutedEventArgs e)
        {
            if (!(e.OriginalSource is PasswordBox passwordBox))
                return;

            string username = GetSavedPasswordUsername(passwordBox);
            string site = GetSavedPasswordSite(passwordBox);

            if (passwordBox.SecurePassword.Length == 0 &&
                !string.IsNullOrEmpty(username) &&
                !string.IsNullOrEmpty(site))
            {
                var passwordManager = GetSavedPasswordManager(passwordBox);
                var password = passwordManager.GetPassword(site, username);
                if (password != null)
                    passwordBox.Password = password;
            }
        }
    }
}
