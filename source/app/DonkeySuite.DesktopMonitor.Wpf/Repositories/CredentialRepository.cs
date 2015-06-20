using System;
using DonkeySuite.DesktopMonitor.Domain.Model.Repositories;

namespace DonkeySuite.DesktopMonitor.Wpf.Repositories
{
    public class CredentialRepository : ICredentialRepository
    {
        public string GetApiKey()
        {
            return Guid.NewGuid().ToString();
        }
    }
}