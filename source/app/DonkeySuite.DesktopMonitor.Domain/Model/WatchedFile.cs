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
        private readonly Lazy<ILog> _log = new Lazy<ILog>(() => DependencyManager.Kernel.Get<ILog>());

        public string FullPath { get; set; }
        public string FileName { get; set; }
        public bool UploadSuccessful { get; set; }

        public ILog Log { get { return _log.Value; } }

        public ISortStrategy SortStrategy
        {
            get { return _sortStrategy ?? (_sortStrategy = DependencyManager.Kernel.Get<ISortStrategy>("defaultSortStrategy")); }
            set { _sortStrategy = value; }
        }

        public byte[] LoadImageBytes()
        {
            if (Log.IsInfoEnabled)
            {
                Log.InfoFormat("Beginning LoadImage: \"{0}\"", FullPath);
            }
            else
            {
                Log.Debug("Beginning LoadImage method.");
            }

            // TODO: This needs to be abstracted away similar to the XML stuff with settings
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

        public void SendToServer()
        {
            Log.InfoFormat("Transmitting image: {0}", FullPath);

            var req = DependencyManager.Kernel.Get<AddImageRequest>();
            var settings = DependencyManager.Kernel.Get<SettingsManager>().GetSettings();
            req.RequestUrl = settings.ImageServer.ServerUrl;
            req.FileName = FileName;
            req.FileBytes = LoadImageBytes();

            UploadSuccessful = req.Post();
        }

        public bool IsInBaseDirectory(String directory)
        {
            var tmp = Path.Combine(directory, FileName);
            return tmp.Equals(FullPath);
        }

        public void SortFile()
        {
            if (SortStrategy == null) throw new InvalidOperationException("Sort strategy is not set.");

            var oldPath = FullPath;
            var lastDirSeparator = oldPath.LastIndexOf(Path.DirectorySeparatorChar);
            var baseDir = oldPath.Substring(0, lastDirSeparator);

            var newPath = SortStrategy.NewFileName(baseDir, FileName);

            if (File.Exists(newPath))
            {
                Log.Info(string.Format("Renaming file. From: {0} To: {1}", oldPath, newPath));
                lastDirSeparator = newPath.LastIndexOf(Path.DirectorySeparatorChar);
                Directory.CreateDirectory(newPath.Substring(0, lastDirSeparator));
                File.Move(oldPath, newPath);
            }
            else
            {
                Log.Info(string.Format("Moving file failed due to existing file in destination. File name: {0}", FileName));
            }
        }
    }
}