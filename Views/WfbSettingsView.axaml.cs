using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Threading;
using OpenIPC_Config.ViewModels;
using Prism.Events;

namespace OpenIPC_Config.Views
{
    public partial class WfbSettingsView : UserControl, INotifyPropertyChanged
    {
        // public WfbSettingsView(IEventAggregator eventAggregator)
        // {
        //     InitializeComponent();
        //     DataContext = new WfbSettingsTabViewModel(eventAggregator);
        // }
        
        public WfbSettingsView()
        {
            InitializeComponent();
            
            DataContext = ViewModelLocator.Instance.WfbSettingsTabViewModel;
        }
    }
}