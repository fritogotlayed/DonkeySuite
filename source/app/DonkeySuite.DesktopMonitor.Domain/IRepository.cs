using System.Collections.Generic;

namespace DonkeySuite.DesktopMonitor.Domain
{
    public interface IRepository<T1, T2>
    {
        T2 Insert(T1 entity);
        void Delete(T1 entity);
        void Update(T1 entity);
        T1 GetById(T2 id);
        /*IEnumerable<T> FindAll(DetachedCriteria criteria);*/ // <-- Figure out a way to make this not NHibernateSpecific. I.e. do it after I need it.
    }
}