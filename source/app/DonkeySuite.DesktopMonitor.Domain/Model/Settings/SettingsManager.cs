using System;
using System.IO;
using System.Runtime.Serialization;
using System.Threading;
using DonkeySuite.DesktopMonitor.Domain.Model.Providers;
using log4net;
using MadDonkeySoftware.SystemWrappers.IO;
using MadDonkeySoftware.SystemWrappers.Threading;
using MadDonkeySoftware.SystemWrappers.Xml.Serialization;

namespace DonkeySuite.DesktopMonitor.Domain.Model.Settings
{
    public class SettingsManager : ISettingsManager
    {
        private readonly ILog _log;
        private readonly ISemaphore _available;
        private readonly IFile _file;
        private readonly IXmlSerializer _serializer;
        private readonly IEnvironmentUtility _environmentUtility;
        private readonly IServiceLocator _serviceLocator;
        private const string FileName = "settings.xml";
        private SettingsRoot _settingsRoot;

        public SettingsManager(ILog log, ISemaphore semaphore, IFile file, IXmlSerializer serializer, IEnvironmentUtility environmentUtility, IServiceLocator serviceLocator)
        {
            // TODO: Dependency injection feels like it is bloating the constructor. Might be time to re-address the responsibility of this classes methods.
            _log = log;
            _available = semaphore;
            _file = file;
            _serializer = serializer;
            _environmentUtility = environmentUtility;
            _serviceLocator = serviceLocator;
        }

        public SettingsRoot GetSettings()
        {
            try
            {
                _available.WaitOne();
            }
            catch (AbandonedMutexException e)
            {
                _log.Warn("Failed to acquire semaphore lock when loading settings.", e);
                return null;
            }

            try
            {
                // If the settings have already been populated then return them
                if (_settingsRoot != null)
                {
                    _available.Release();
                    return _settingsRoot;
                }

                var path = GetSettingsFilePath();
                if (!_file.Exists(path))
                {
                    _log.Warn(string.Format("Settings file \"{0}\" does not exist. Populating with defaults.", path));
                    _settingsRoot = _serviceLocator.ProvideDefaultSettingsRoot();

                    try
                    {
                        using (var writer = _serviceLocator.ProvideTextWriter(path))
                        {
                            _serializer.Serialize(writer, _settingsRoot);
                        }
                    }
                    catch (SerializationException e)
                    {
                        _log.Warn("Could not save default settings.", e);
                    }
                }
                else
                {
                    _log.Info(string.Format("Settings file \"{0}\" used.", path));

                    try
                    {
                        using (var fileStream = _file.Open(path, FileMode.Open))
                        {
                            _settingsRoot = (SettingsRoot) _serializer.Deserialize(fileStream);
                        }
                    }
                    catch (SerializationException e)
                    {
                        _log.Error("Error opening settings file.", e);
                    }
                }

                _available.Release();

            }
            catch (Exception)
            {
                _available.Release();
                throw;
            }

            return _settingsRoot;
        }

        public void SaveSettings()
        {
            try
            {
                _available.WaitOne();

                var path = GetSettingsFilePath();
                using (var writer = _serviceLocator.ProvideTextWriter(path))
                {
                    _serializer.Serialize(writer, _settingsRoot);
                }
            }
            catch (ThreadInterruptedException e)
            {
                _log.Warn("Failed to acquire semaphore lock when saving settings.", e);
            }
            catch (SerializationException e)
            {
                _available.Release();
                throw new IOException("Could not save settings.", e);
            }
            catch (Exception e)
            {
                _log.Fatal("Error saving settings.", e);
                _available.Release();
                throw;
            }
            _available.Release();
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