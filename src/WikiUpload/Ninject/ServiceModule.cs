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
                .WithConstructorArgument("timeoutSeconds", App.Timewout);

            Bind<INavigatorService>()
                .ToConstant(App.Navigator);

            Bind<Properties.IAppSettings>()
                .To<Properties.AppSettings>()
                .InSingletonScope();

            Bind<IDialogManager>()
                .To<DialogManager>();

            Bind<IPasswordManager>()
                .To<PasswordManager>();

            Bind<IPasswordStore>()
                .To<PasswordStore>();

            Bind<IUploadListSerializer>()
                .To<UploadListSerializer>();

            Bind<IHelpers>()
                .To<Helpers>();

            Bind<UploadViewModel>()
                .ToSelf()
                .InSingletonScope();

            Bind<IWikiSearchFactory>()
                .To<WikiSearchFactory>();

            Bind<IUpdateCheck>()
                .To<UpdateCheck>();

            Bind<IWindowManager>()
                .To<WindowManager>();

            Bind<IYoutube>()
                .To<Youtube>();
        }
    }
}