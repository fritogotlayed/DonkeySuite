using System.Configuration;
using DonkeySuite.DesktopMonitor.Domain;
using DonkeySuite.DesktopMonitor.Domain.Model;
using DonkeySuite.DesktopMonitor.Wpf.Mappings;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using NHibernate;
using NHibernate.Tool.hbm2ddl;
using Ninject;

namespace DonkeySuite.DesktopMonitor.Wpf
{
    public class NHibernateHelper
    {
        private static ISessionFactory _sessionFactory;
        private static NHibernate.Cfg.Configuration _configuration;

        private static NHibernate.Cfg.Configuration Configuration
        {
            get
            {
                if (_configuration != null) return _configuration;

                var environmentWrapper = DependencyManager.Kernel.Get<IEnvironmentUtility>();
                var sep = environmentWrapper.DirectorySeparatorChar.ToString();
                var userHome = environmentWrapper.UserHomeDirectory;
                var fullFilePath = string.Join(sep, userHome, ".mdsoftware", "dirWatcher", "dirWatcher.db");
                var connString = string.Format("Data Source={0};Version=3", fullFilePath);

                var fluentConfiguration = Fluently.Configure()
                    .Database(SQLiteConfiguration.Standard.ConnectionString(connString))
                    .Mappings(m => m.FluentMappings.AddFromAssemblyOf<WatchedFileMap>());

                bool rebuildDatabase;
                bool.TryParse(ConfigurationManager.AppSettings["rebuildDatabase"], out rebuildDatabase);
                if (rebuildDatabase)
                {
                    fluentConfiguration.ExposeConfiguration(cfg => new SchemaExport(cfg).Create(false, true));
                }
                else
                {
                    fluentConfiguration.ExposeConfiguration(cfg => new SchemaUpdate(cfg).Execute(false, true));
                }

                _configuration = fluentConfiguration.BuildConfiguration();
                
                return _configuration;
            }
        }

        private static ISessionFactory SessionFactory
        {
            get { return _sessionFactory ?? (_sessionFactory = Configuration.BuildSessionFactory()); }
        }

        public static ISession OpenSession()
        {
            return SessionFactory.OpenSession();
        }
    }
}