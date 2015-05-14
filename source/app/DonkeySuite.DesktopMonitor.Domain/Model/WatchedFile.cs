using System;
using System.IO;
using DonkeySuite.DesktopMonitor.Domain.Model.Requests;
using DonkeySuite.DesktopMonitor.Domain.Model.Settings;
using DonkeySuite.DesktopMonitor.Domain.Model.SortStrategies;
using log4net;
using Ninject;

namespace DonkeySuite.DesktopMonitor.Domain.Model
{
    public class WatchedFile
    {
        private ISortStrategy _sortStrategy;
        private readonly ILog _log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public string FullPath { get; set; }
        public string FileName { get; set; }
        public bool UploadSuccessful { get; set; }

        public ISortStrategy SortStrategy
        {
            get { return _sortStrategy ?? (_sortStrategy = DependencyManager.Kernel.Get<ISortStrategy>("defaultSortStrategy")); }
            set { _sortStrategy = value; }
        }

        public byte[] LoadImageBytes()
        {
            if (_log.IsInfoEnabled)
            {
                _log.Info(string.Format("Beginning loadImage: \"{0}\"", FullPath));
            }
            else
            {
                _log.Debug("Beginning loadImage method.");
            }

            // TODO: This needs to be abstracted away similar to the XML stuff with settings
            var data = File.ReadAllBytes(FullPath);

            if (_log.IsInfoEnabled)
            {
                _log.Info(string.Format("Returning loadImage: \"{0}\"", FullPath));
            }
            else
            {
                _log.Debug("Returning loadImage result.");
            }

            return data;
        }

        public void SendToServer()
        {
            _log.Info("Transmitting image: " + FullPath);

            var req = DependencyManager.Kernel.Get<AddImageRequest>();
            req.RequestUrl = SettingsManager.Instance.GetSettings().ImageServer.ServerUrl;
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
                _log.Info(string.Format("Renaming file. From: {0} To: {1}", oldPath, newPath));
                lastDirSeparator = newPath.LastIndexOf(Path.DirectorySeparatorChar);
                Directory.CreateDirectory(newPath.Substring(0, lastDirSeparator));
                File.Move(oldPath, newPath);
            }
            else
            {
                _log.Info(string.Format("Moving file failed due to existing file in destination. File name: {0}", FileName));
            }
        }
    }
}