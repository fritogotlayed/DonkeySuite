using DonkeySuite.DesktopMonitor.Domain.Model;
using NHibernate;

namespace DonkeySuite.DesktopMonitor.Wpf.Repositories
{
    public class WatchedFileRepository : Repository<WatchedFile, string>
    {
        public WatchedFileRepository(ISession session) : base(session)
        {
        }

        public override string Insert(WatchedFile entity)
        {
            return (string)Session.Save(entity);
        }

        public override void Delete(WatchedFile entity)
        {
            Session.Delete(entity);
        }

        public override void Update(WatchedFile entity)
        {
            Session.Update(entity);
        }

        public override WatchedFile GetById(string id)
        {
            return Session.Get<WatchedFile>(id);
        }

    }
}