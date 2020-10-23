using Ninject;
using Ninject.Parameters;
using System;
using System.Windows;

namespace WikiUpload
{
    public class ServiceLocator
    {
        private readonly IKernel _kernel;

        public ServiceLocator()
        {
            _kernel = new StandardKernel(new ServiceModule());
        }

        public LoginViewModel LoginViewModel => _kernel.Get<LoginViewModel>();

        public UploadViewModel UploadViewModel => _kernel.Get<UploadViewModel>();

        public SettingsViewModel SettingsViewModel => _kernel.Get<SettingsViewModel>();

        internal ErrorMessageViewModel ErrorMessageViewModel(Window window)
            => _kernel.Get<ErrorMessageViewModel>(new ConstructorArgument("window", window));

        public AboutBoxViewModel AboutBoxViewModel(Window window) 
            => _kernel.Get<AboutBoxViewModel>(new ConstructorArgument("window", window));

        public InsecureWarningViewModel InsecureWarningViewModel(Window window) 
            => _kernel.Get<InsecureWarningViewModel>(new ConstructorArgument("window", window));
    }
}
