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

            Bind<INavigatorService>()
                .ToConstant(App.Navigator);

            Bind<Properties.IAppSettings>()
                .To<Properties.AppSettings>()
                .InSingletonScope();

            Bind<IDialogManager>()
                .To<DialogManager>();

            Bind<IPasswordManager>()
                .To<PasswordManager>();

            Bind<IDelay>()
                .To<Delay>();

            Bind<IPasswordStore>()
                .To<PasswordStore>();

            Bind<ITextFile>()
                .To<TextFile>();

            Bind<IUploadListSerializer>()
                .To<UploadListSerializer>();
        }
    }
}