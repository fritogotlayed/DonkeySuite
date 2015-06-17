using DonkeySuite.DesktopMonitor.Domain.Model;
using DonkeySuite.DesktopMonitor.Domain.Model.Repositories;
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

        public IWatchedFile CreateNew()
        {
            return _kernel.Get<IWatchedFile>();
        }

        public IWatchedFile LoadFileForPath(string filePath)
        {
            return Session.Get<IWatchedFile>(filePath);
        }

        public void Save(IWatchedFile watchedFile)
        {
            Session.Save(watchedFile);
        }
    }
}