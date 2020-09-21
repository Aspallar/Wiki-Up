using Ninject.Modules;
using WikiUpload.Service;

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

        }
    }
}