using System;
using System.IO;
using System.Runtime.Serialization;
using System.Threading;
using DonkeySuite.SystemWrappers.Interfaces;
using log4net;
using Ninject;
using Ninject.Parameters;

namespace DonkeySuite.DesktopMonitor.Domain.Model.Settings
{
    public class SettingsManager
    {
        private readonly ILog _log;
        private const string FileName = "settings.xml";
        private readonly ISemaphore _available;
        private SettingsRoot _settingsRoot;

        public SettingsManager(ILog log, ISemaphore semaphore)
        {
            // TODO: Get rid of the constructor injection
            _log = log;
            _available = semaphore;
        }

        // NOTE: Not sure I want to do this everywhere. Just a thing to try for now.
        private IKernel Kernel
        {
            get { return DependencyManager.Kernel; }
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

                var serializer = Kernel.Get<ISerializer>("SettingsSerializer");

                var path = GetSettingsFilePath();
                if (!Kernel.Get<IFile>().Exists(path))
                {
                    _log.Warn(string.Format("Settings file \"{0}\" does not exist. Populating with defaults.", path));
                    _settingsRoot = DependencyManager.Kernel.Get<SettingsRoot>();
                    _settingsRoot.PopulateWithDefaults();

                    try
                    {
                        using (var writer = Kernel.Get<TextWriter>(new ConstructorArgument("path", path)))
                        {
                            serializer.Serialize(writer, _settingsRoot);
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
                        using (var fileStream = Kernel.Get<IFile>().Open(path, FileMode.Open))
                        {
                            _settingsRoot = (SettingsRoot) serializer.Deserialize(fileStream);
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
                var serializer = DependencyManager.Kernel.Get<ISerializer>();
                using (var writer = Kernel.Get<TextWriter>(new ConstructorArgument("path", path)))
                {
                    serializer.Serialize(writer, _settingsRoot);
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
                _log.Error("Error saving settings.", e);
                _available.Release();
                throw;
            }
            _available.Release();
        }

        private string GetSettingsFilePath()
        {
            var environmentWrapper = Kernel.Get<IEnvironment>();
            var sep = environmentWrapper.DirectorySeparatorChar.ToString();
            var userHome = environmentWrapper.UserHomeDirectory;
            var fileFullPath = string.Join(sep, userHome, ".mdsoftware", "dirWatcher", FileName);

            return fileFullPath;
        }
    }
}