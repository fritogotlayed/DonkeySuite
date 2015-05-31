using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DonkeySuite.DesktopMonitor.Domain.Model.Providers;
using DonkeySuite.DesktopMonitor.Domain.Model.Repositories.Settings;
using DonkeySuite.DesktopMonitor.Domain.Model.Settings;
using DonkeySuite.DesktopMonitor.Domain.Model.SortStrategies;
using log4net;

namespace DonkeySuite.DesktopMonitor.Domain.Model
{
    public class WatchedDirectory
    {
        private string _directory;
        private bool _includeSubDirectories;
        private OperationMode _mode;
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private List<WatchedFile> _imageFiles;
        private readonly List<string> _acceptableExtensions;
        private ISortStrategy _sortStrategy;
        private IServiceLocator _serviceLocator;

        public WatchedDirectory()
        {
            _acceptableExtensions = new List<string>();
        }

        public WatchedDirectory(IServiceLocator serviceLocator) : this()
        {
            _serviceLocator = serviceLocator;
        }

        /*
        public WatchedDirectory(string directory) : this()
        {
            this._directory = directory;
        }
        */

        public void Configure(WatchDirectory watchDir)
        {
            _directory = watchDir.Path;
            _includeSubDirectories = watchDir.IncludeSubDirectories;
            _mode = watchDir.Mode;

            var strategy = watchDir.SortStrategy;
            if (string.IsNullOrEmpty(strategy))
            {
                _sortStrategy = _serviceLocator.ProvideDefaultSortStrategy();
            }
            else
            {
                strategy = strategy.ToLower();
                _sortStrategy = _serviceLocator.ProvideSortStrategy(strategy);
            }

            foreach (string ext in watchDir.FileExtensions.Split(','))
            {
                _acceptableExtensions.Add(string.Format(".{0}", ext));
            }
        }

        public void ProcessAvailableImages(IWatchedFileRepository watchedFileRepository)
        {
            var images = GetAvailableImages(watchedFileRepository);

            foreach (var image in images.Where(image => image != null))
            {
                try
                {
                    if (!image.UploadSuccessful && (_mode.Equals(OperationMode.UploadAndClear) || _mode.Equals(OperationMode.UploadOnly) || _mode.Equals(OperationMode.UploadAndSort)))
                    {
                        image.SendToServer();
                        watchedFileRepository.Save(image);
                    }

                    if (image.IsInBaseDirectory(_directory) && (_mode.Equals(OperationMode.SortOnly) || _mode.Equals(OperationMode.UploadAndSort)))
                    {
                        Log.Info(string.Format("Beginning sort of file: {0}", image.FullPath));
                        image.SortFile();
                    }
                }
                catch (IOException ex)
                {
                    image.UploadSuccessful = false;
                    watchedFileRepository.Save(image);
                    Log.Error("Failed: " + image, ex);
                }
            }
        }

        private List<WatchedFile> GetAvailableImages(IWatchedFileRepository watchedFileRepository)
        {
            Log.DebugFormat("Beginning to list available images: {0}", _directory);

            _imageFiles = new List<WatchedFile>();

            PopulateFilesForFolder(watchedFileRepository, _imageFiles, _directory);

            Log.DebugFormat("Returning list of available images: {0} with count {1}", _directory, _imageFiles.Count);

            return _imageFiles;
        }

        private void PopulateFilesForFolder(IWatchedFileRepository watchedFileRepository, List<WatchedFile> fileList, String path)
        {
            Log.DebugFormat("populating files and subfiles in {0}", path);

            foreach (var file in Directory.GetFiles(path).Where(f => _acceptableExtensions.Contains(Path.GetExtension(f))))
            {
                var watchedFile = watchedFileRepository.LoadFileForPath(file);
                Log.DebugFormat("Processing file {0}", file);

                // If we can't find the file then create it
                if (watchedFile == null)
                {
                    watchedFile = watchedFileRepository.CreateNew();
                    watchedFile.FullPath = file;
                    watchedFile.FileName = Path.GetFileName(file);
                    watchedFile.UploadSuccessful = false;
                }
                else
                {
                    // This is a hack until I can get things fully dependency injected.
                    var tmp = watchedFile;
                    watchedFile = watchedFileRepository.CreateNew();
                    watchedFile.FullPath = tmp.FullPath;
                    watchedFile.FileName = tmp.FileName;
                    watchedFile.UploadSuccessful = tmp.UploadSuccessful;
                }


                // Ensure each file has the correct sort strategy in-case it has changed.
                watchedFile.SortStrategy = _sortStrategy;
                fileList.Add(watchedFile);
            }

            if (_includeSubDirectories)
            {
                foreach (var directory in Directory.GetDirectories(path))
                {
                    PopulateFilesForFolder(watchedFileRepository, fileList, directory);
                }
            }

        }
    }
}