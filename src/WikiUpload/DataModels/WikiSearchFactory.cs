namespace WikiUpload
{
    public class WikiSearchFactory : IWikiSearchFactory
    {
        public IWikiSearch CreateCategorySearch(IFileUploader fileUploader)
            => new CategorySearch(fileUploader);

        public IWikiSearch CreateTemplateSearch(IFileUploader fileUploader)
            => new TemplateSearch(fileUploader);
    }
}
