using NHibernate;

namespace DonkeySuite.DesktopMonitor.Wpf.Repositories
{
    public abstract class Repository
    {
        protected ISession Session { get; private set; }

        protected Repository(ISession session)
        {
            Session = session;
        }
    }
}