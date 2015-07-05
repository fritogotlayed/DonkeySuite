/*
<copyright file="IEntityProvider.cs">
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
using System.IO;
using DonkeySuite.DesktopMonitor.Domain.Model.Settings;
using DonkeySuite.DesktopMonitor.Domain.Model.SortStrategies;

namespace DonkeySuite.DesktopMonitor.Domain.Model.Providers
{
    public interface IEntityProvider
    {
        TextWriter ProvideTextWriter(string path);
        ISortStrategy ProvideDefaultSortStrategy();
        ISortStrategy ProvideSortStrategy(string key);
        SettingsRoot ProvideDefaultSettingsRoot();
        WatchDirectory ProvideDefaultWatchDirectory();
        IWatchDirectories ProvideDefaultWatchDirectories();
        ImageServer ProvideDefaultImageServer();
        IImageServers ProvideDefaultImageServers();
    }
}