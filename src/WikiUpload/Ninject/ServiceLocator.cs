using Ninject;

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

        public AboutViewModel AboutViewModel => _kernel.Get<AboutViewModel>();
    }
}
