using System;
using System.Security;
using System.Windows;
using System.Windows.Controls;

namespace WikiUpload
{
    public static class SavedPassword
    {
        #region Username Property

        public static readonly DependencyProperty UsernameProperty =
            DependencyProperty.RegisterAttached
            (
                "Username",
                typeof(string),
                typeof(SavedPassword),
                new UIPropertyMetadata(null, OnUsernameChanged)
            );

        public static String GetUsername(DependencyObject obj)
        {
            return obj.GetValue(UsernameProperty) as String;
        }

        public static void SetUsername(DependencyObject obj, string value)
        {
            obj.SetValue(UsernameProperty, value);
        }

        private static void OnUsernameChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (!(sender is PasswordBox passwordBox))
                return;

            if (e.OldValue == null)
            {
                passwordBox.Loaded -= PasswordBox_Loaded;
                passwordBox.Unloaded -= PasswordBox_Unloaded;
                passwordBox.GotFocus -= PasswordBox_GotFocus;
                if (e.NewValue != null)
                {
                    passwordBox.Loaded += PasswordBox_Loaded;
                    passwordBox.Unloaded += PasswordBox_Unloaded;
                    passwordBox.GotFocus += PasswordBox_GotFocus;
                }
            }
            else
            {
                UpdatePassword(passwordBox, GetSite(passwordBox), (string)e.NewValue);
            }
        }

        #endregion

        #region Site Property

        public static readonly DependencyProperty SiteProperty =
            DependencyProperty.RegisterAttached
            (
                "Site",
                typeof(String),
                typeof(SavedPassword),
                new UIPropertyMetadata(null, OnSiteChanged)
            );

        private static void OnSiteChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (!(sender is PasswordBox passwordBox))
                return;

            if (e.OldValue != null)
                UpdatePassword(passwordBox, (string)e.NewValue, GetUsername(passwordBox));
        }

        public static String GetSite(DependencyObject obj)
        {
            return (String)obj.GetValue(SiteProperty);
        }

        public static void SetSite(DependencyObject obj, String value)
        {
            obj.SetValue(SiteProperty, value);
        }
        #endregion 

        #region PasswordManager Property

        public static readonly DependencyProperty PasswordManagerProperty =
            DependencyProperty.RegisterAttached
            (
                "PasswordManager",
                typeof(IPasswordManager),
                typeof(SavedPassword),
                new UIPropertyMetadata(null)
            );

        public static IPasswordManager GetPasswordManager(DependencyObject obj)
        {
            return (IPasswordManager)obj.GetValue(PasswordManagerProperty);
        }

        public static void SetPasswordManager(DependencyObject obj, String value)
        {
            obj.SetValue(PasswordManagerProperty, value);
        }

        #endregion

        #region AutoDisposePasswords Property

        public static readonly DependencyProperty AutoDisposePasswordsProperty =
            DependencyProperty.RegisterAttached
            (
                "AutoDisposePasswords",
                typeof(bool),
                typeof(SavedPassword),
                new UIPropertyMetadata(false)
            );

        public static bool GetAutoDisposePasswords(DependencyObject obj)
        {
            return (bool)obj.GetValue(AutoDisposePasswordsProperty);
        }

        public static void SetAutoDisposePasswords(DependencyObject obj, bool value)
        {
            obj.SetValue(AutoDisposePasswordsProperty, value);
        }

        #endregion

        #region Password Property

        public static readonly DependencyProperty PasswordProperty =
            DependencyProperty.RegisterAttached
            (
                "Password",
                typeof(SecureString),
                typeof(SavedPassword),
                new UIPropertyMetadata(null)
            );

        public static SecureString GetPassword(DependencyObject obj)
            => (SecureString)obj.GetValue(PasswordProperty);

        public static void SetPassword(DependencyObject obj, String value)
            => obj.SetValue(PasswordProperty, value);

        #endregion

        #region PasswordBox event handlers

        private static void PasswordBox_Loaded(object sender, RoutedEventArgs e)
        {
            if (e.OriginalSource is PasswordBox passwordBox)
                UpdatePassword(passwordBox, GetSite(passwordBox), GetUsername(passwordBox));
        }

        private static void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (e.OriginalSource is PasswordBox passwordBox)
            {
                var savedPassword = GetPassword(passwordBox);
                savedPassword.Clear();
            }
        }

        private static void PasswordBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (e.OriginalSource is PasswordBox passwordBox)
            {
                var savedPassword = GetPassword(passwordBox);
                if (savedPassword.Length != 0)
                    passwordBox.SelectAll();
            }
        }

        private static void PasswordBox_Unloaded(object sender, RoutedEventArgs e)
        {
            if (e.OriginalSource is PasswordBox passwordBox)
            {
                if (GetAutoDisposePasswords(passwordBox))
                {
                    passwordBox.SecurePassword.Dispose();
                    var savedPassword = GetPassword(passwordBox);
                    savedPassword.Dispose();
                }
            }
        }

        #endregion

        #region Private Methods

        private static void UpdatePassword(PasswordBox passwordBox, string site, string username)
        {
            passwordBox.PasswordChanged -= PasswordBox_PasswordChanged;
            var savedPassword = GetPassword(passwordBox);
            if (string.IsNullOrEmpty(site) || string.IsNullOrEmpty(username))
            {
                ResetPasword(passwordBox, savedPassword);
            }
            else
            {
                var passwordManager = GetPasswordManager(passwordBox);
                if (passwordManager.HasPassword(site, username))
                {
                    savedPassword.Clear();
                    int length;
                    using (var password = passwordManager.GetPassword(site, username))
                    {
                        foreach (var c in password.Data)
                            savedPassword.AppendChar(c);
                        length = password.Data.Length;
                    }
                    passwordBox.Password = new string('-', length);
                }
                else
                {
                    ResetPasword(passwordBox, savedPassword);
                }
            }
            passwordBox.PasswordChanged += PasswordBox_PasswordChanged;
        }

        private static void ResetPasword(PasswordBox passwordBox, SecureString savedPassword)
        {
            if (savedPassword.Length != 0)
            {
                savedPassword.Clear();
                passwordBox.Password = "";
            }
        }

        #endregion
    }

}
