/*
<copyright file="EntityProvider.cs">
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
using DonkeySuite.DesktopMonitor.Domain.Model.Providers;
using DonkeySuite.DesktopMonitor.Domain.Model.Settings;
using DonkeySuite.DesktopMonitor.Domain.Model.SortStrategies;
using MadDonkeySoftware.SystemWrappers;
using Ninject;
using Ninject.Parameters;

namespace DonkeySuite.DesktopMonitor.Wpf
{
    public class EntityProvider : IEntityProvider
    {
        private readonly IKernel _kernel;
        private readonly IEnvironment _environment;

        public EntityProvider(IKernel kernel, IEnvironment environment)
        {
            _kernel = kernel;
            _environment = environment;
        }

        public TextWriter ProvideTextWriter(string path)
        {
            return _kernel.Get<TextWriter>(new ConstructorArgument("path", path));
        }

        // TODO: Move below to repositories.
        public ISortStrategy ProvideDefaultSortStrategy()
        {
            return _kernel.Get<ISortStrategy>("defaultSortStrategy");
        }

        public ISortStrategy ProvideSortStrategy(string key)
        {
            const string suffix = "SortStrategy";
            if (string.IsNullOrEmpty(key)) return ProvideDefaultSortStrategy();
            if (!key.EndsWith(suffix)) key += suffix;
            return _kernel.Get<ISortStrategy>(key);
        }

        public SettingsRoot ProvideDefaultSettingsRoot()
        {
            var settings = new SettingsRoot(this);
            settings.PopulateWithDefaults();
            return settings;
        }

        public WatchDirectory ProvideDefaultWatchDirectory()
        {
            var wd = new WatchDirectory(_environment);
            wd.PopulateWithDefaults();
            return wd;
        }

        public IWatchDirectories ProvideDefaultWatchDirectories()
        {
            var wd = new WatchDirectories(this);
            wd.PopulateWithDefaults();
            return wd;
        }

        public ImageServer ProvideDefaultImageServer()
        {
            var imageServer = new ImageServer(this);
            imageServer.PopulateWithDefaults();
            return imageServer;
        }

        public IImageServers ProvideDefaultImageServers()
        {
            var imageServers = new ImageServers(this);
            imageServers.PopulateWithDefaults();
            return imageServers;
        }
    }
}