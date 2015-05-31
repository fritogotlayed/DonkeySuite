using DonkeySuite.DesktopMonitor.Domain.Model;
using DonkeySuite.DesktopMonitor.Domain.Model.Repositories.Settings;
using NHibernate;
using Ninject;

namespace DonkeySuite.DesktopMonitor.Wpf.Repositories
{
    public class WatchedFileRepository : Repository, IWatchedFileRepository
    {
        private readonly IKernel _kernel;

        public WatchedFileRepository(ISession session, IKernel kernel) : base(session)
        {
            _kernel = kernel;
        }

        public WatchedFile CreateNew()
        {
            return _kernel.Get<WatchedFile>();
        }

        public WatchedFile LoadFileForPath(string filePath)
        {
            return Session.Get<WatchedFile>(filePath);
        }

        public void Save(WatchedFile watchedFile)
        {
            Session.Save(watchedFile);
        }
    }
}