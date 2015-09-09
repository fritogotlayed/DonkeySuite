﻿/*
<copyright file="DesktopMonitorBaseViewModel.cs">
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
using DonkeySuite.DesktopMonitor.Domain.Model;
using GalaSoft.MvvmLight;
using log4net;

namespace DonkeySuite.DesktopMonitor.Wpf.ViewModel
{
    public abstract class DesktopMonitorBaseViewModel : ViewModelBase
    {

        protected DesktopMonitorBaseViewModel(ILogProvider logProvider)
        {
            if (!IsInDesignMode)
            {
                Log = logProvider.GetLogger(GetType());
            }
        }

        protected ILog Log { get; private set; }
    }
}