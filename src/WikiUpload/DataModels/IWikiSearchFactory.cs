namespace WikiUpload
{
    public interface IWikiSearchFactory
    {
        IWikiSearch CreateCategorySearch(IFileUploader fileUploader);
        IWikiSearch CreateTemplateSearch(IFileUploader fileUploader);
    }
}