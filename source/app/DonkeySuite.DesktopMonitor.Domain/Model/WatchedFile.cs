using System;
using System.IO;
using DonkeySuite.DesktopMonitor.Domain.Model.Requests;
using DonkeySuite.DesktopMonitor.Domain.Model.Settings;
using DonkeySuite.DesktopMonitor.Domain.Model.SortStrategies;
using DonkeySuite.DesktopMonitor.Domain.Model.Wrappers;
using log4net;
using Ninject;

namespace DonkeySuite.DesktopMonitor.Domain.Model
{
    public class WatchedFile
    {
        private ISortStrategy _sortStrategy;
        private readonly ILog _log = DependencyManager.Kernel.Get<ILog>();

        public virtual string FullPath { get; set; }
        public virtual string FileName { get; set; }
        public virtual bool UploadSuccessful { get; set; }

        protected virtual ILog Log { get { return _log; } }

        public virtual ISortStrategy SortStrategy
        {
            get { return _sortStrategy ?? (_sortStrategy = DependencyManager.Kernel.Get<ISortStrategy>("defaultSortStrategy")); }
            set { _sortStrategy = value; }
        }

        public virtual byte[] LoadImageBytes()
        {
            if (Log.IsInfoEnabled)
            {
                Log.InfoFormat("Beginning LoadImage: \"{0}\"", FullPath);
            }
            else
            {
                Log.Debug("Beginning LoadImage method.");
            }

            var file = DependencyManager.Kernel.Get<IFileWrapper>();
            var data = file.ReadAllBytes(FullPath);

            if (Log.IsInfoEnabled)
            {
                Log.InfoFormat("Returning LoadImage: \"{0}\"", FullPath);
            }
            else
            {
                Log.Debug("Returning LoadImage result.");
            }

            return data;
        }

        public virtual void SendToServer()
        {
            Log.InfoFormat("Transmitting image: {0}", FullPath);

            var req = DependencyManager.Kernel.Get<AddImageRequest>();
            var settings = DependencyManager.Kernel.Get<SettingsManager>().GetSettings();
            req.RequestUrl = settings.ImageServer.ServerUrl;
            req.FileName = FileName;
            req.FileBytes = LoadImageBytes();

            UploadSuccessful = req.Post();
        }

        public virtual bool IsInBaseDirectory(String directory)
        {
            var tmp = Path.Combine(directory, FileName);
            return tmp.Equals(FullPath);
        }

        public virtual void SortFile()
        {
            var oldPath = FullPath;
            var environmentWrapper = DependencyManager.Kernel.Get<IEnvironmentWrapper>();
            var lastDirSeparator = oldPath.LastIndexOf(environmentWrapper.DirectorySeparatorChar);
            var baseDir = oldPath.Substring(0, lastDirSeparator);
            string newPath;

            try
            {
                newPath = SortStrategy.NewFileName(baseDir, FileName);
            }
            catch (ActivationException ex)
            {
                _log.Debug("Activation exception on SortStrategy", ex);
                throw new InvalidOperationException("Sort strategy is not set.");
            }

            var fileWrapper = DependencyManager.Kernel.Get<IFileWrapper>();
            if (fileWrapper.Exists(newPath))
            {
                Log.InfoFormat("Moving file failed due to existing file in destination. File name: {0}", FileName);
            }
            else
            {
                var directoryWrapper = DependencyManager.Kernel.Get<IDirectoryWrapper>();

                Log.InfoFormat("Renaming file. From: {0} To: {1}", oldPath, newPath);

                lastDirSeparator = newPath.LastIndexOf(environmentWrapper.DirectorySeparatorChar);
                directoryWrapper.CreateDirectory(newPath.Substring(0, lastDirSeparator));
                fileWrapper.Move(oldPath, newPath);
            }
        }
    }
}