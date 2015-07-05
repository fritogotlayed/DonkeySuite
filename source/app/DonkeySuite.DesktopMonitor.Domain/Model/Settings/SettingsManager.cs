/*
<copyright file="SettingsManager.cs">
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