using DonkeySuite.DesktopMonitor.Domain.Model.Requests;
using Ninject;

namespace DonkeySuite.DesktopMonitor.Wpf
{
    public static class DependencyManager
    {
        private static volatile IKernel kernel;
        private static readonly object _syncRoot = new object();

        public static IKernel Kernel
        {
            get
            {
                if (kernel == null)
                {
                    lock (_syncRoot)
                    {
                        if (kernel == null)
                        {
                            kernel = new StandardKernel();
                            Initialize(kernel);
                        }
                    }
                }

                return kernel;
            }
        }

        private static void Initialize(IKernel kernel)
        {
            kernel.Bind<AddImageRequest>().ToSelf().Named("addImageRequest");
        }
    }
}