using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace WikiUpload
{
    public static class SavedPasswordBehavior
    {
        private static readonly RoutedEventHandler initializePassword = new RoutedEventHandler(InitializePassword);
        private static readonly RoutedEventHandler passwordChanged = new RoutedEventHandler(OnPasswordChanged);
        private static readonly RoutedEventHandler unloaded = new RoutedEventHandler(OnUnloaded);


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
                passwordBox.GotFocus -= initializePassword;
                passwordBox.Loaded -= initializePassword;
                passwordBox.Unloaded -= unloaded;
                if (e.NewValue != null)
                {
                    passwordBox.GotFocus += initializePassword;
                    passwordBox.Loaded += initializePassword;
                    passwordBox.Unloaded += unloaded;
                }
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

        #region SavedPasswordSecurePassword

        public static readonly DependencyProperty SavedPasswordSecurePassword =
            DependencyProperty.RegisterAttached
            (
                "SavedPasswordSecurePassword",
                typeof(SecureString),
                typeof(SavedPasswordBehavior),
                new UIPropertyMetadata(null)
            );

        public static SecureString GetSavedPasswordSecurePassword(DependencyObject obj)
        {
            return (SecureString)obj.GetValue(SavedPasswordSecurePassword);
        }

        public static void SetSavedPasswordSecurePassword(DependencyObject obj, String value)
        {
            obj.SetValue(SavedPasswordSecurePassword, value);
        }

        #endregion

        private static void InitializePassword(object sender, RoutedEventArgs e)
        {
            if (e.OriginalSource is PasswordBox passwordBox &&
                passwordBox.SecurePassword.Length == 0)
            {
                string username = GetSavedPasswordUsername(passwordBox);
                string site = GetSavedPasswordSite(passwordBox);

                if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(site))
                {
                    var passwordManager = GetSavedPasswordManager(passwordBox);
                    var password = passwordManager.GetPassword(site, username);
                    if (password != null)
                    {
                        SecureString savedPassword = GetSavedPasswordSecurePassword(passwordBox);
                        savedPassword.Clear();
                        foreach (var c in password)
                            savedPassword.AppendChar(c);
                        Array.Clear(password, 0, password.Length);
                        passwordBox.Password = new string('x', password.Length);
                        passwordBox.PasswordChanged += passwordChanged;
                    }
                }
            }
        }

        private static void OnPasswordChanged(object sender, RoutedEventArgs e)
        {
            if (e.OriginalSource is PasswordBox passwordBox)
            {
                var savedPassword = GetSavedPasswordSecurePassword(passwordBox);
                savedPassword.Clear();
                passwordBox.PasswordChanged -= passwordChanged;
            }
        }

        private static void OnUnloaded(object sender, RoutedEventArgs e)
        {
            if (e.OriginalSource is PasswordBox passwordBox)
            {
                // we know for sure that we are finished with the secure passwords as an
                // unload will only happen when log in is done with, or app is closing.

                passwordBox.SecurePassword.Dispose();

                // the saved passwoird dependancy property is disposed of here rather than in the
                // view model as when the page is unloaded we get an extra PasswordChanged event
                // which causes an exception if it's already been disposed by the view model.

                var savedPassword = GetSavedPasswordSecurePassword(passwordBox);
                savedPassword.Dispose();
            }
        }

    }
}
