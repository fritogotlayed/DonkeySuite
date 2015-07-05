/*
<copyright file="DirectoryScanner.cs">
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
using System.Collections.Generic;
using System.Linq;
using DonkeySuite.DesktopMonitor.Domain.Model.Repositories;
using DonkeySuite.DesktopMonitor.Domain.Model.SortStrategies;
using log4net;
using MadDonkeySoftware.SystemWrappers.IO;

namespace DonkeySuite.DesktopMonitor.Domain.Model
{
    public class DirectoryScanner : IDirectoryScanner
    {
        private readonly ILog _log;
        private readonly IPath _path;
        private readonly IDirectory _directory;

        private ISortStrategy _sortStrategy;
        private IList<string> _acceptableExtensions;
        private bool _includeSubDirectories;

        public DirectoryScanner(ILogProvider logProvider, IPath path, IDirectory directory)
        {
            _log = logProvider.GetLogger(GetType());
            _path = path;
            _directory = directory;
        }

        public IList<IWatchedFile> GetAvailableImages(IWatchedFileRepository watchedFileRepository, string path, IList<string> acceptableExtensions, bool includeSubDirectories,
                                                      ISortStrategy sortStrategy)
        {
            _log.DebugFormat("Beginning to list available images: {0}", path);

            var imageFiles = new List<IWatchedFile>();

            _acceptableExtensions = acceptableExtensions;
            _includeSubDirectories = includeSubDirectories;

            PopulateFilesForFolder(watchedFileRepository, imageFiles, path);

            _log.DebugFormat("Returning list of available images: {0} with count {1}", path, imageFiles.Count);

            return imageFiles;
        }

        private void PopulateFilesForFolder(IWatchedFileRepository watchedFileRepository, List<IWatchedFile> fileList, string path)
        {
            _log.DebugFormat("populating files and subfiles in {0}", path);

            foreach (var file in _directory.GetFiles(path).Where(f => _acceptableExtensions.Contains(_path.GetExtension(f))))
            {
                var watchedFile = watchedFileRepository.LoadFileForPath(file);
                _log.DebugFormat("Processing file {0}", file);

                // If we can't find the file then create it
                if (watchedFile == null)
                {
                    watchedFile = watchedFileRepository.CreateNew();
                    watchedFile.FullPath = file;
                    watchedFile.FileName = _path.GetFileName(file);
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
                foreach (var directory in _directory.GetDirectories(path))
                {
                    PopulateFilesForFolder(watchedFileRepository, fileList, directory);
                }
            }

        }
    }
}