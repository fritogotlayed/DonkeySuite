using System;
using System.IO;
using System.Runtime.Serialization;
using System.Threading;
using System.Xml.Serialization;
using log4net;
using Ninject;

namespace DonkeySuite.DesktopMonitor.Domain.Model.Settings
{
    public class SettingsManager
    {
        public static SettingsManager Instance = new SettingsManager();

        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private static string _fileName = "settings.xml";
        private Semaphore available = new Semaphore(0, 1);
        private Settings settings;

        private SettingsManager()
        {
            // Hide the constructor from others. This doesn't protect against reflection.
        }

        public Settings GetSettings()
        {
            try
            {
                available.WaitOne();
            }
            catch (AbandonedMutexException e)
            {
                Log.Warn("Failed to acquire semaphore lock when loading settings.", e);
                return null;
            }

            try
            {
                // If the settings have already been populated then return them
                if (settings != null)
                {
                    available.Release();
                    return settings;
                }

                var serializer = DependencyManager.Kernel.Get<XmlSerializer>();

                var path = GetSettingsFilePath();
                if (!File.Exists(path))
                {
                    Log.Warn(string.Format("Settings file \"%s\" does not exist. Populating with defaults.", path));
                    settings = DependencyManager.Kernel.Get<Settings>("settings");
                    settings.PopulateWithDefaults();

                    try
                    {
                        using (var writer = new StreamWriter(path))
                        {
                            serializer.Serialize(writer, settings);
                        }
                    }
                    catch (SerializationException e)
                    {
                        Log.Warn("Could not save default settings.", e);
                    }
                }
                else
                {
                    Log.Info(string.Format("Settings file \"%s\" used.", path));

                    try
                    {
                        using (var fileStream = new FileStream(path, FileMode.Open))
                        {
                            settings = (Settings) serializer.Deserialize(fileStream);
                        }
                    }
                    catch (SerializationException e)
                    {
                        Log.Error("Error opening settings file.", e);
                    }
                }

                available.Release();

            }
            catch (Exception)
            {
                available.Release();
                throw;
            }

            return settings;
        }

        public void SaveSettings()
        {
            try
            {
                available.WaitOne();

                var path = GetSettingsFilePath();
                var serializer = DependencyManager.Kernel.Get<XmlSerializer>();
                using (var writer = new StreamWriter(path))
                {
                    serializer.Serialize(writer, settings);
                }
            }
            catch (ThreadInterruptedException e)
            {
                Log.Warn("Failed to acquire semaphore lock when saving settings.", e);
            }
            catch (SerializationException e)
            {
                available.Release();
                throw new IOException("Could not save settings.", e);
            }
            catch (Exception e)
            {
                Log.Error("Error saving settings.", e);
                available.Release();
                throw;
            }
            available.Release();
        }

        private string GetSettingsFilePath()
        {
            var sep = Path.PathSeparator.ToString();
            var userHome = (Environment.OSVersion.Platform == PlatformID.Unix ||
                            Environment.OSVersion.Platform == PlatformID.MacOSX)
                ? Environment.GetEnvironmentVariable("HOME")
                : Environment.ExpandEnvironmentVariables("%HOMEDRIVE%%HOMEPATH%");
            var fileFullPath = string.Join(sep, userHome, ".mdsoftware", "dirWatcher", _fileName);

            return fileFullPath;
        }
    }
}