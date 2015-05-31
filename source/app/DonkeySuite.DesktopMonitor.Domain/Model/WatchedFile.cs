using System;
using DonkeySuite.DesktopMonitor.Domain.Model.Providers;
using DonkeySuite.DesktopMonitor.Domain.Model.SortStrategies;
using log4net;
using MadDonkeySoftware.SystemWrappers.IO;

namespace DonkeySuite.DesktopMonitor.Domain.Model
{
    public class WatchedFile
    {
        private ISortStrategy _sortStrategy;
        private readonly ILog _log;
        private readonly IServiceLocator _serviceLocator;
        private readonly IFile _file;
        private readonly IRequestProvider _requestProvider;
        private readonly IEnvironmentUtility _environmentUtility;
        private readonly IDirectory _directory;
        private readonly IPath _path;

        public WatchedFile() : this(null, null, null, null, null, null, null) { }

        public WatchedFile(ILog log, IServiceLocator serviceLocator, IFile file, IRequestProvider requestProvider, IEnvironmentUtility environmentUtility, IDirectory directory, IPath path)
        {
            // TODO: Dependency injection feels like it is bloating the constructor. Might be time to re-address the responsibility of this classes methods.
            _log = log;
            _serviceLocator = serviceLocator;
            _file = file;
            _requestProvider = requestProvider;
            _environmentUtility = environmentUtility;
            _directory = directory;
            _path = path;
        }

        public virtual string FullPath { get; set; }
        public virtual string FileName { get; set; }
        public virtual bool UploadSuccessful { get; set; }

        public virtual ISortStrategy SortStrategy
        {
            get { return _sortStrategy ??  (_sortStrategy = _serviceLocator.ProvideSortStrategy(null)); }
            set { _sortStrategy = value; }
        }

        public virtual byte[] LoadImageBytes()
        {
            if (_log.IsInfoEnabled)
            {
                _log.InfoFormat("Beginning LoadImage: \"{0}\"", FullPath);
            }
            else
            {
                _log.Debug("Beginning LoadImage method.");
            }

            var data = _file.ReadAllBytes(FullPath);

            if (_log.IsInfoEnabled)
            {
                _log.InfoFormat("Returning LoadImage: \"{0}\"", FullPath);
            }
            else
            {
                _log.Debug("Returning LoadImage result.");
            }

            return data;
        }

        public virtual void SendToServer()
        {
            _log.InfoFormat("Transmitting image: {0}", FullPath);

            var req = _requestProvider.ProvideNewAddImageRequest(FileName, LoadImageBytes());

            UploadSuccessful = req.Post();
        }

        public virtual bool IsInBaseDirectory(String directory)
        {
            var tmp = _path.Combine(directory, FileName);
            return tmp.Equals(FullPath);
        }

        public virtual void SortFile()
        {
            var oldPath = FullPath;
            var lastDirSeparator = oldPath.LastIndexOf(_environmentUtility.DirectorySeparatorChar);
            var baseDir = oldPath.Substring(0, lastDirSeparator);

            // try
            // {
                var newPath = SortStrategy.NewFileName(baseDir, FileName);
            // }
            // catch (ActivationException ex)
            // {
                // _log.Debug("Activation exception on SortStrategy", ex);
                // throw new InvalidOperationException("Sort strategy is not set.");
            // }

            if (_file.Exists(newPath))
            {
                _log.InfoFormat("Moving file failed due to existing file in destination. File name: {0}", FileName);
            }
            else
            {
                _log.InfoFormat("Renaming file. From: {0} To: {1}", oldPath, newPath);

                lastDirSeparator = newPath.LastIndexOf(_environmentUtility.DirectorySeparatorChar);
                _directory.CreateDirectory(newPath.Substring(0, lastDirSeparator));
                _file.Move(oldPath, newPath);
            }
        }
    }
}