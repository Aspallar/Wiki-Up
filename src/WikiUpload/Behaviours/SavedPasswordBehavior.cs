using System;
using System.Security;
using System.Windows;
using System.Windows.Controls;

namespace WikiUpload
{
    public static class SavedPasswordBehavior
    {
        #region SavedPasswordUsername Property

        public static readonly DependencyProperty SavedPasswordUsername =
            DependencyProperty.RegisterAttached
            (
                "SavedPasswordUsername",
                typeof(string),
                typeof(SavedPasswordBehavior),
                new UIPropertyMetadata(null, SavedPasswordUsernameChanged)
            );

        public static String GetSavedPasswordUsername(DependencyObject obj)
        {
            return obj.GetValue(SavedPasswordUsername) as String;
        }

        public static void SetSavedPasswordUsername(DependencyObject obj, string value)
        {
            obj.SetValue(SavedPasswordUsername, value);
        }

        private static void SavedPasswordUsernameChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (!(sender is PasswordBox passwordBox))
                return;

            if (e.OldValue == null)
            {
                passwordBox.Loaded -= Loaded;
                passwordBox.Unloaded -= Unloaded;
                passwordBox.GotFocus -= GotFocus;
                if (e.NewValue != null)
                {
                    passwordBox.Loaded += Loaded;
                    passwordBox.Unloaded += Unloaded;
                    passwordBox.GotFocus += GotFocus;
                }
            }
            else
            {
                UpdatePassword(passwordBox, GetSavedPasswordSite(passwordBox), (string)e.NewValue);
            }
        }

        #endregion

        #region SavedPasswordSite Property

        public static readonly DependencyProperty SavedPasswordSite =
            DependencyProperty.RegisterAttached
            (
                "SavedPasswordSite",
                typeof(String),
                typeof(SavedPasswordBehavior),
                new UIPropertyMetadata(null, SavedPasswordSiteChanged)
            );

        private static void SavedPasswordSiteChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (!(sender is PasswordBox passwordBox))
                return;

            if (e.OldValue != null)
                UpdatePassword(passwordBox, (string)e.NewValue, GetSavedPasswordUsername(passwordBox));
        }

        public static String GetSavedPasswordSite(DependencyObject obj)
        {
            return (String)obj.GetValue(SavedPasswordSite);
        }

        public static void SetSavedPasswordSite(DependencyObject obj, String value)
        {
            obj.SetValue(SavedPasswordSite, value);
        }
        #endregion 

        #region SavedPasswordManager Property

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

        #region SavedPasswordAutoDisposePasswords Property

        public static readonly DependencyProperty SavedPasswordAutoDisposePasswords =
            DependencyProperty.RegisterAttached
            (
                "SavedPasswordAutoDisposePasswords",
                typeof(bool),
                typeof(SavedPasswordBehavior),
                new UIPropertyMetadata(false)
            );

        public static bool GetSavedPasswordAutoDisposePasswords(DependencyObject obj)
        {
            return (bool)obj.GetValue(SavedPasswordAutoDisposePasswords);
        }

        public static void SetSavedPasswordAutoDisposePasswords(DependencyObject obj, bool value)
        {
            obj.SetValue(SavedPasswordAutoDisposePasswords, value);
        }

        #endregion


        #region SavedPasswordSecurePassword Property

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

        #region PasswordBox event handlers

        private static void Loaded(object sender, RoutedEventArgs e)
        {
            if (e.OriginalSource is PasswordBox passwordBox)
                UpdatePassword(passwordBox, GetSavedPasswordSite(passwordBox), GetSavedPasswordUsername(passwordBox));
        }

        private static void PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (e.OriginalSource is PasswordBox passwordBox)
            {
                var savedPassword = GetSavedPasswordSecurePassword(passwordBox);
                savedPassword.Clear();
            }
        }

        private static void GotFocus(object sender, RoutedEventArgs e)
        {
            if (e.OriginalSource is PasswordBox passwordBox)
            {
                var savedPassword = GetSavedPasswordSecurePassword(passwordBox);
                if (savedPassword.Length != 0)
                    passwordBox.SelectAll();
            }
        }

        private static void Unloaded(object sender, RoutedEventArgs e)
        {
            if (e.OriginalSource is PasswordBox passwordBox)
            {
                if (GetSavedPasswordAutoDisposePasswords(passwordBox))
                {
                    passwordBox.SecurePassword.Dispose();
                    var savedPassword = GetSavedPasswordSecurePassword(passwordBox);
                    savedPassword.Dispose();
                }
            }
        }

        #endregion

        #region Private Methods

        private static void UpdatePassword(PasswordBox passwordBox, string site, string username)
        {
            passwordBox.PasswordChanged -= PasswordChanged;
            var savedPassword = GetSavedPasswordSecurePassword(passwordBox);
            if (string.IsNullOrEmpty(site) || string.IsNullOrEmpty(username))
            {
                ResetPasword(passwordBox, savedPassword);
            }
            else
            {
                var passwordManager = GetSavedPasswordManager(passwordBox);
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
            passwordBox.PasswordChanged += PasswordChanged;
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
