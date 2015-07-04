using System;
using DonkeySuite.DesktopMonitor.Domain.Model.Providers;
using DonkeySuite.DesktopMonitor.Domain.Model.Settings;
using DonkeySuite.DesktopMonitor.Domain.Model.SortStrategies;
using log4net;
using MadDonkeySoftware.SystemWrappers.IO;

namespace DonkeySuite.DesktopMonitor.Domain.Model
{
    public class WatchedFile : IWatchedFile
    {
        private ISortStrategy _sortStrategy;
        private readonly ILog _log;
        private readonly IEntityProvider _entityLocator;
        private readonly IFile _file;
        private readonly IRequestProvider _requestProvider;
        private readonly IEnvironmentUtility _environmentUtility;
        private readonly IDirectory _directory;

        public WatchedFile() : this(null, null, null, null, null, null) { }

        public WatchedFile(ILog log, IEntityProvider entityLocator, IFile file, IRequestProvider requestProvider, IEnvironmentUtility environmentUtility, IDirectory directory)
        {
            // TODO: Dependency injection feels like it is bloating the constructor. Might be time to re-address the responsibility of this classes methods.
            _log = log;
            _entityLocator = entityLocator;
            _file = file;
            _requestProvider = requestProvider;
            _environmentUtility = environmentUtility;
            _directory = directory;
        }

        public virtual string FullPath { get; set; }
        public virtual string FileName { get; set; }
        public virtual bool UploadSuccessful { get; set; }

        public virtual ISortStrategy SortStrategy
        {
            get { return _sortStrategy ??  (_sortStrategy = _entityLocator.ProvideSortStrategy(null)); }
            set { _sortStrategy = value; }
        }

        public virtual byte[] LoadImageBytes()
        {
            _log.DebugFormat("Beginning LoadImage: \"{0}\"", FullPath);

            var data = _file.ReadAllBytes(FullPath);

            _log.DebugFormat("Returning LoadImage: \"{0}\"", FullPath);

            return data;
        }

        public virtual void SendToServer(IImageServer server)
        {
            _log.DebugFormat("Transmitting image: {0}", FullPath);

            var req = _requestProvider.ProvideNewAddImageRequest(server, FileName, LoadImageBytes());

            UploadSuccessful = req.Post();
        }

        public virtual bool IsInBaseDirectory(String directory)
        {
            var tmp = _environmentUtility.CombinePath(directory, FileName);
            return tmp.Equals(FullPath);
        }

        public virtual void SortFile()
        {
            var oldPath = FullPath;
            var lastDirSeparator = oldPath.LastIndexOf(_environmentUtility.DirectorySeparatorChar);
            var baseDir = oldPath.Substring(0, lastDirSeparator);

            var newPath = SortStrategy.NewFileName(baseDir, FileName);

            if (_file.Exists(newPath))
            {
                _log.DebugFormat("Moving file failed due to existing file in destination. File name: {0}", FileName);
            }
            else
            {
                _log.DebugFormat("Renaming file. From: {0} To: {1}", oldPath, newPath);

                lastDirSeparator = newPath.LastIndexOf(_environmentUtility.DirectorySeparatorChar);
                _directory.CreateDirectory(newPath.Substring(0, lastDirSeparator));
                _file.Move(oldPath, newPath);
            }
        }

        public virtual void RemoveFromDisk()
        {
            _file.Delete(FullPath);
        }
    }
}