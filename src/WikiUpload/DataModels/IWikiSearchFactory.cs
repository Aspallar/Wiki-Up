namespace WikiUpload
{
    internal interface IWikiSearchFactory
    {
        IWikiSearch CreateCategorySearch(IFileUploader fileUploader);
        IWikiSearch CreateTemplateSearch(IFileUploader fileUploader);
    }
}