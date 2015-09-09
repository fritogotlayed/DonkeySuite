/*
<copyright file="NHibernateHelper.cs">
   Copyright 2015 MadDonkey Software

   Licensed under the Apache License, Version 2.0 (the "License");
   you may not use this file except in compliance with the License.
   You may obtain a copy of the License at

       http://www.apache.org/licenses/LICENSE-2.0

   Unless required by applicable law or agreed to in writing, software
   distributed under the License is distributed on an "AS IS" BASIS,
   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
   See the License for the specific language governing permissions and
   limitations under the License.
</copyright>
*/

using System.Configuration;
using DonkeySuite.DesktopMonitor.Domain.Model;
using DonkeySuite.DesktopMonitor.Wpf.Mappings;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using MadDonkeySoftware.SystemWrappers.IO;
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
                var directory = DependencyManager.Kernel.Get<IDirectory>();

                var sep = environmentWrapper.DirectorySeparatorChar.ToString();
                var userHome = environmentWrapper.UserHomeDirectory;
                var settingsFolder = string.Join(sep, userHome, ".mdsoftware", "dirWatcher");
                var fullFilePath = string.Join(sep, settingsFolder, "dirWatcher.db");
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

                // Make sure our settings folder exists
                directory.CreateDirectory(settingsFolder);

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