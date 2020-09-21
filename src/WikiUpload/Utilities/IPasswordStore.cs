namespace WikiUpload
{
    public interface IPasswordStore
    {
        PasswordDictionary Load();
        void Save(PasswordDictionary passwords);
    }
}