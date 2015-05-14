using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DonkeySuite.DesktopMonitor.Domain.Model.Settings;
using DonkeySuite.DesktopMonitor.Domain.Model.SortStrategies;
using log4net;
using NHibernate;
using Ninject;

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

        public WatchedDirectory()
        {
            _acceptableExtensions = new List<string>();
        }

        public WatchedDirectory(string directory) : this()
        {
            this._directory = directory;
        }

        public void Configure(WatchDirectory watchDir)
        {
            _directory = watchDir.Path;
            _includeSubDirectories = watchDir.IncludeSubDirectories;
            _mode = watchDir.Mode;

            var strategy = watchDir.SortStrategy;
            if (string.IsNullOrEmpty(strategy))
            {
                _sortStrategy = DependencyManager.Kernel.Get<ISortStrategy>("defaultSortStrategy");
            }
            else
            {
                strategy = strategy.ToLower();
                _sortStrategy = DependencyManager.Kernel.Get<ISortStrategy>(string.Join(string.Empty, strategy, "SortStrategy"));
            }

            foreach (string ext in watchDir.FileExtensions.Split(','))
            {
                _acceptableExtensions.Add(ext);
            }
        }

        public void ProcessAvailableImages(ISession session)
        {
            var images = GetAvailableImages(session);

            foreach (var image in images.Where(image => image != null))
            {
                try
                {
                    if (!image.UploadSuccessful && (_mode.Equals(OperationMode.UploadAndClear) || _mode.Equals(OperationMode.UploadOnly) || _mode.Equals(OperationMode.UploadAndSort)))
                    {
                        image.SendToServer();
                        session.Save(image);
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
                    session.Save(image);
                    Log.Error("Failed: " + image, ex);
                }
            }
        }

        private List<WatchedFile> GetAvailableImages(ISession session)
        {

            if (Log.IsInfoEnabled)
            {
                Log.Info(string.Format("Beginning to list available images: {0}", _directory));
            }
            else
            {
                Log.Debug("Beginning to list available images.");
            }

            _imageFiles = new List<WatchedFile>();

            PopulateFilesForFolder(session, _imageFiles, _directory);

            if (Log.IsInfoEnabled)
            {
                Log.Info(string.Format("Returning list of available images: {0} with count {1}", _directory,
                    _imageFiles.Count));
            }
            else
            {
                Log.Debug("Returning list of available images.");
            }

            return _imageFiles;
        }

        private void PopulateFilesForFolder(ISession session, List<WatchedFile> fileList, String path)
        {
            if (Log.IsInfoEnabled)
            {
                Log.Info(string.Format("populating files and subfiles in {0}", path));
            }

            if (_includeSubDirectories)
            {
                foreach (var directory in Directory.GetDirectories(path))
                {
                    PopulateFilesForFolder(session, fileList, directory);
                }
            }

            foreach (var file in Directory.GetFiles(path).Where(f => _acceptableExtensions.Contains(Path.GetExtension(f))))
            {
                var watchedFile = session.Get<WatchedFile>(file);

                // If we can't find the file then create it
                if (watchedFile == null)
                {
                    watchedFile = DependencyManager.Kernel.Get<WatchedFile>();
                    watchedFile.FullPath = file;
                    watchedFile.FileName = Path.GetFileName(file);
                    watchedFile.UploadSuccessful = false;
                }

                // Ensure each file has the correct sort strategy in-case it has changed.
                watchedFile.SortStrategy = _sortStrategy;
                fileList.Add(watchedFile);
            }
        }
    }
}