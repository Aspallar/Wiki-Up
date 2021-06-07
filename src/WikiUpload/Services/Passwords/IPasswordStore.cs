namespace WikiUpload
{
    internal interface IPasswordStore
    {
        PasswordDictionary Load();
        void Save(PasswordDictionary passwords);
    }
}