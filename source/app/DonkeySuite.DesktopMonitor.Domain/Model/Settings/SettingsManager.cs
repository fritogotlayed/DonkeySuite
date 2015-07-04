using System;
using System.Threading;
using DonkeySuite.DesktopMonitor.Domain.Model.Repositories;
using log4net;
using MadDonkeySoftware.SystemWrappers.Threading;

namespace DonkeySuite.DesktopMonitor.Domain.Model.Settings
{
    public class SettingsManager : ISettingsManager
    {
        private readonly ILog _log;
        private readonly ISemaphore _available;
        private readonly ISettingsRepository _settingsRepository;
        private SettingsRoot _settingsRoot;

        public SettingsManager(ILog log, ISemaphore semaphore, ISettingsRepository settingsRepository)
        {
            _log = log;
            _available = semaphore;
            _settingsRepository = settingsRepository;
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

                _settingsRoot = _settingsRepository.Load();
                if (_settingsRoot == null)
                {
                    _settingsRoot = _settingsRepository.CreateNewSettings();
                    try
                    {
                        _settingsRepository.Save(_settingsRoot);
                    }
                    catch (Exception ex)
                    {
                        _log.Warn("Saving new settings failed.", ex);
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
                _settingsRepository.Save(_settingsRoot);
                _available.Release();
            }
            catch (Exception e)
            {
                _log.Fatal("Error saving settings.", e);
                _available.Release();
                throw;
            }
        }
    }
}