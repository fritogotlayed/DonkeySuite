using System;
using System.IO;
using System.Xml.Serialization;
using DonkeySuite.DesktopMonitor.Domain.Model;
using DonkeySuite.DesktopMonitor.Domain.Model.Providers;
using DonkeySuite.DesktopMonitor.Domain.Model.Requests;
using DonkeySuite.DesktopMonitor.Domain.Model.Settings;
using DonkeySuite.DesktopMonitor.Domain.Model.SortStrategies;
using log4net;
using MadDonkeySoftware.SystemWrappers;
using MadDonkeySoftware.SystemWrappers.IO;
using MadDonkeySoftware.SystemWrappers.Threading;
using MadDonkeySoftware.SystemWrappers.Xml.Serialization;
using Ninject;

namespace DonkeySuite.DesktopMonitor.Wpf
{
    public static class DependencyManager
    {
        // NOTE: If unit tests start stepping on one another it may be time
        // https://github.com/ninject/Ninject/wiki/Injection-Patterns

        private static volatile IKernel _kernel;
        private static readonly object SyncRoot = new object();
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public static Action<IKernel> AdditionalBindings { get; set; }

        static DependencyManager()
        {
            AdditionalBindings = kernel => { };
        }

        // TODO: Refactor calling code so that this may be removed.
        public static IKernel Kernel
        {
            get
            {
                if (_kernel == null)
                {
                    lock (SyncRoot)
                    {
                        if (_kernel == null)
                        {
                            _kernel = new StandardKernel();
                            Initialize(_kernel);
                        }
                    }
                }

                return _kernel;
            }
            set
            {
                Log.Debug("Kernel has been set by external caller.");
                _kernel = value;
            }
        }

        private static void Initialize(IKernel kernel)
        {
            Log.Debug("Initializing kernel.");

            kernel.Bind<AddImageRequest>().ToSelf().InTransientScope();
            kernel.Bind<IDirectory>().To<DirectoryWrapper>().InTransientScope();
            kernel.Bind<IEnvironment>().To<EnvironmentWrapper>().InTransientScope();
            kernel.Bind<IEnvironmentUtility>().To<EnvironmentUtility>().InTransientScope();
            kernel.Bind<IFile>().To<FileWrapper>().InTransientScope();
            kernel.Bind<ILog>().ToMethod(context => LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType));
            kernel.Bind<IPath>().To<PathWrapper>().InTransientScope();
            kernel.Bind<IRequestProvider>().To<RequestProvider>().InTransientScope();
            kernel.Bind<ISemaphore>().To<SemaphoreWrapper>().InTransientScope()
                .WithConstructorArgument("initialCount", 1)
                .WithConstructorArgument("maximumCount", 1);
            kernel.Bind<IServiceLocator>().To<ServiceLocator>();
            kernel.Bind<IXmlSerializer>()
                .To<XmlSerializerWrapper>()
                .InTransientScope()
                .WithConstructorArgument("serializer", new XmlSerializer(typeof (SettingsRoot)));
            kernel.Bind<SettingsManager>()
                .ToSelf()
                .InSingletonScope();
            kernel.Bind<SettingsRoot>().ToSelf().InTransientScope();
            kernel.Bind<TextWriter>().To<StreamWriter>().InTransientScope();
            kernel.Bind<WatchedFile>().ToSelf().InTransientScope();

            // Sort strategies are checked by using camel case in code.
            kernel.Bind<ISortStrategy>().To<SimpleSortStrategy>().Named("defaultSortStrategy");
            kernel.Bind<ISortStrategy>().To<SimpleSortStrategy>().Named("simpleSortStrategy");

            AdditionalBindings(kernel);
        }
    }
}