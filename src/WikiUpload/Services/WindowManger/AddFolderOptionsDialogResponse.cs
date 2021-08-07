namespace WikiUpload
{
    internal class AddFolderOptionsDialogResponse
    {
        public bool Ok { get; set; }
        public bool IncludeSubfolders { get; set; }
        public IncludeFiles IncludeFiles { get; set; }
        public string Extension { get; set; }
    }
}
