/*
<copyright file="SettingsRepository.cs">
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
using System;
using System.IO;
using System.Runtime.Serialization;
using DonkeySuite.DesktopMonitor.Domain.Model;
using DonkeySuite.DesktopMonitor.Domain.Model.Providers;
using DonkeySuite.DesktopMonitor.Domain.Model.Repositories;
using DonkeySuite.DesktopMonitor.Domain.Model.Settings;
using log4net;
using MadDonkeySoftware.SystemWrappers.IO;
using MadDonkeySoftware.SystemWrappers.Xml.Serialization;

namespace DonkeySuite.DesktopMonitor.Wpf.Repositories
{
    public class SettingsRepository  : ISettingsRepository
    {
        private const string FileName = "settings.xml";

        private readonly ILog _log;
        private readonly IEntityProvider _entityProvider;
        private readonly IXmlSerializer _serializer;
        private readonly IFile _file;
        private readonly IEnvironmentUtility _environmentUtility;

        public SettingsRepository(ILogProvider logProvider, IEntityProvider entityProvider, IXmlSerializer xmlSerializer, IFile file, IEnvironmentUtility environmentUtility)
        {
            _log = logProvider.GetLogger(GetType());
            _entityProvider = entityProvider;
            _serializer = xmlSerializer;
            _file = file;
            _environmentUtility = environmentUtility;
        }

        public void Save(SettingsRoot settingsRoot)
        {
            try
            {
                var path = GetSettingsFilePath();
                using (var writer = _entityProvider.ProvideTextWriter(path))
                {
                    _serializer.Serialize(writer, settingsRoot);
                }
            }
            catch (SerializationException e)
            {
                _log.Warn("Failed to serialize the provided settings root.", e);
                throw new IOException("Could not save settings.", e);
            }
            catch (Exception e)
            {
                _log.Fatal("Error saving settings.", e);
                throw;
            }
        }

        public SettingsRoot Load()
        {
            SettingsRoot settingsRoot = null;
            var path = GetSettingsFilePath();
            if (!_file.Exists(path))
            {
                _log.Warn(string.Format("Settings file \"{0}\" does not exist.", path));
            }
            else
            {
                _log.Info(string.Format("Settings file \"{0}\" used.", path));

                try
                {
                    using (var fileStream = _file.Open(path, FileMode.Open))
                    {
                        settingsRoot = (SettingsRoot)_serializer.Deserialize(fileStream);
                    }
                }
                catch (SerializationException e)
                {
                    _log.Error("Error opening settings file.", e);
                }
            }

            return settingsRoot;
        }

        public SettingsRoot CreateNewSettings()
        {
                return _entityProvider.ProvideDefaultSettingsRoot();
        }

        private string GetSettingsFilePath()
        {
            var sep = _environmentUtility.DirectorySeparatorChar.ToString();
            var userHome = _environmentUtility.UserHomeDirectory;
            var fileFullPath = string.Join(sep, userHome, ".mdsoftware", "dirWatcher", FileName);

            return fileFullPath;
        }
    }
}