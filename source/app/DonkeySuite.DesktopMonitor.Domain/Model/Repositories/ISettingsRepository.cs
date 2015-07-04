using DonkeySuite.DesktopMonitor.Domain.Model.Settings;

namespace DonkeySuite.DesktopMonitor.Domain.Model.Repositories
{
    public interface ISettingsRepository
    {
        void Save(SettingsRoot settingsRoot);
        SettingsRoot Load();
        SettingsRoot CreateNewSettings();
    }
}