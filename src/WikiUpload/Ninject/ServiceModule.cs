using Ninject.Modules;

namespace WikiUpload
{
    public class ServiceModule : NinjectModule
    {
        public override void Load()
        {
            Bind<IFileUploader>()
                .To<FileUploader>()
                .InSingletonScope()
                .WithConstructorArgument("userAgent", App.UserAgent)
                .WithConstructorArgument("timeout", App.Timewout);
        }
    }
}