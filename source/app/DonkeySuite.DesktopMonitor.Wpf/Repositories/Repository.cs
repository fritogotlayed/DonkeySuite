using DonkeySuite.DesktopMonitor.Domain;
using NHibernate;

namespace DonkeySuite.DesktopMonitor.Wpf.Repositories
{
    public abstract class Repository<T1, T2> : IRepository<T1, T2>
    {
        protected ISession Session { get; private set; }

        protected Repository(ISession session)
        {
            Session = session;
        }

        public abstract T2 Insert(T1 entity);
        public abstract void Delete(T1 entity);
        public abstract void Update(T1 entity);
        public abstract T1 GetById(T2 id);
    }
}