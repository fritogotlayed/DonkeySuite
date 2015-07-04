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