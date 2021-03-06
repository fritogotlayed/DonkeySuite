﻿/*
<copyright file="DependencyManager.cs">
   Copyright 2015 MadDonkey Software

   Licensed under the Apache License, Version 2.0 (the "License");
   you may not use this file except in compliance with the License.
   You may obtain a copy of the License at

       http://www.apache.org/licenses/LICENSE-2.0

   Unless required by applicable law or agreed to in writing, software
   distributed under the License is distributed on an "AS IS" BASIS,
   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
   See the License for the specific language governing permissions and
   limitations under the License.
</copyright>
*/
using System.IO;
using System.Xml.Serialization;
using DonkeySuite.DesktopMonitor.Domain.Model;
using DonkeySuite.DesktopMonitor.Domain.Model.Providers;
using DonkeySuite.DesktopMonitor.Domain.Model.Repositories;
using DonkeySuite.DesktopMonitor.Domain.Model.Requests;
using DonkeySuite.DesktopMonitor.Domain.Model.Settings;
using DonkeySuite.DesktopMonitor.Domain.Model.SortStrategies;
using DonkeySuite.DesktopMonitor.Wpf.Repositories;
using DonkeySuite.DesktopMonitor.Wpf.ViewModel;
using log4net;
using MadDonkeySoftware.SystemWrappers;
using MadDonkeySoftware.SystemWrappers.IO;
using MadDonkeySoftware.SystemWrappers.Net;
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

        static DependencyManager()
        {
        }

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

            // Domain stuff
            kernel.Bind<AddImageRequest>().ToSelf().InTransientScope();
            kernel.Bind<IDirectory>().To<DirectoryWrapper>().InTransientScope();
            kernel.Bind<ICredentialRepository>().To<CredentialRepository>();
            kernel.Bind<IDirectoryScanner>().To<DirectoryScanner>().InTransientScope();
            kernel.Bind<IEnvironment>().To<EnvironmentWrapper>().InTransientScope();
            kernel.Bind<IEnvironmentUtility>().To<EnvironmentUtility>().InTransientScope();
            kernel.Bind<IFile>().To<FileWrapper>().InTransientScope();
            kernel.Bind<ILog>().ToMethod(context => LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType));
            kernel.Bind<ILogProvider>().To<LogProvider>();
            kernel.Bind<IPath>().To<PathWrapper>().InTransientScope();
            kernel.Bind<IRequestProvider>().To<RequestProvider>().InTransientScope();
            kernel.Bind<ISemaphore>().To<SemaphoreWrapper>().InTransientScope()
                  .WithConstructorArgument("initialCount", 1)
                  .WithConstructorArgument("maximumCount", 1);
            kernel.Bind<IEntityProvider>().To<EntityProvider>();
            kernel.Bind<ISettingsManager>().To<SettingsManager>();
            kernel.Bind<ISettingsRepository>().To<SettingsRepository>();
            kernel.Bind<IWatchedFile>().To<WatchedFile>();
            kernel.Bind<IWebRequestFactory>().To<WebRequestFactory>();
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

            // View Models
            kernel.Bind<MainWindowViewModel>().ToSelf().InSingletonScope();
            kernel.Bind<MainViewModel>().ToSelf().InTransientScope();
            kernel.Bind<SettingsViewModel>().ToSelf().InTransientScope();
        }
    }
}