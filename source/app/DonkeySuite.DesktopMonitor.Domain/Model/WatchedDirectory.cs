using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DonkeySuite.DesktopMonitor.Domain.Model.Providers;
using DonkeySuite.DesktopMonitor.Domain.Model.Repositories;
using DonkeySuite.DesktopMonitor.Domain.Model.Settings;
using DonkeySuite.DesktopMonitor.Domain.Model.SortStrategies;
using log4net;

namespace DonkeySuite.DesktopMonitor.Domain.Model
{
    public class WatchedDirectory
    {
        private string _watchPath;
        private bool _includeSubDirectories;
        private OperationMode _mode;
        private readonly ILog _log;
        private readonly List<string> _acceptableExtensions;
        private ISortStrategy _sortStrategy;
        private readonly IEntityProvider _entityLocator;
        private readonly IDirectoryScanner _directoryScanner;

        public WatchedDirectory(IEntityProvider entityLocator, ILogProvider logProvider, IDirectoryScanner directoryScanner)
        {
            _entityLocator = entityLocator;
            _log = logProvider.GetLogger(GetType());
            _acceptableExtensions = new List<string>();
            _directoryScanner = directoryScanner;
        }

        public void Configure(IWatchDirectory watchDir)
        {
            _watchPath = watchDir.Path;
            _includeSubDirectories = watchDir.IncludeSubDirectories;
            _mode = watchDir.Mode;

            var strategy = watchDir.SortStrategy;
            if (string.IsNullOrEmpty(strategy))
            {
                _sortStrategy = _entityLocator.ProvideDefaultSortStrategy();
            }
            else
            {
                strategy = strategy.ToLower();
                _sortStrategy = _entityLocator.ProvideSortStrategy(strategy);
            }

            foreach (string ext in watchDir.FileExtensions.Split(','))
            {
                _acceptableExtensions.Add(string.Format(".{0}", ext));
            }
        }

        public void ProcessAvailableImages(IWatchedFileRepository watchedFileRepository, IImageServer server)
        {
            if (_mode.Equals(OperationMode.UploadAndClear)) throw new NotImplementedException();

            var images = _directoryScanner.GetAvailableImages(watchedFileRepository, _watchPath, _acceptableExtensions, _includeSubDirectories, _sortStrategy);

            foreach (var image in images.Where(image => image != null))
            {
                try
                {
                    if (!image.UploadSuccessful
                        && (_mode.Equals(OperationMode.UploadAndClear) || _mode.Equals(OperationMode.UploadOnly) || _mode.Equals(OperationMode.UploadAndSort)))
                    {
                        image.SendToServer(server);
                        watchedFileRepository.Save(image);
                    }

                    if (image.IsInBaseDirectory(_watchPath)
                        && (_mode.Equals(OperationMode.SortOnly) || _mode.Equals(OperationMode.UploadAndSort)))
                    {
                        _log.Debug(string.Format("Beginning sort of file: {0}", image.FullPath));
                        image.SortFile();
                    }

                }
                catch (IOException ex)
                {
                    image.UploadSuccessful = false;
                    watchedFileRepository.Save(image);
                    _log.Error("Failed: " + image, ex);
                }
            }
        }
    }
}