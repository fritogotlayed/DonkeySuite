namespace DonkeySuite.DesktopMonitor.Domain.Model.Settings
{
    public interface ISettingsManager
    {
        SettingsRoot GetSettings();
        void SaveSettings();
    }
}