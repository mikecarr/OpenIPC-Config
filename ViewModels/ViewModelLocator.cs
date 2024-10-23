using System;
using OpenIPC_Config.Messages;
using OpenIPC_Config.Models;
using OpenIPC_Config.Services;
using OpenIPC_Config.ViewModels;
using OpenIPC_Config.Views;
using Prism.Events;
using Prism.Ioc;

namespace OpenIPC_Config.ViewModels;

public class ViewModelLocator
{
    public static ViewModelLocator Instance { get; private set; }
    public static MessageViewModel MessageViewModel { get; private set; }
    
    public MainWindowViewModel MainWindowViewModel { get; private set; }
    public WfbSettingsTabViewModel WfbSettingsTabViewModel { get; }
    public CameraSettingsTabViewModel CameraSettingsTabViewModel { get; }

    public IEventAggregator EventAggregator => _eventAggregator;
    private IEventAggregator _eventAggregator;
    

    
    public ViewModelLocator(IEventAggregator eventAggregator)
    {
        _eventAggregator = eventAggregator;
        
        MessageViewModel = new MessageViewModel(eventAggregator);
        var settings = SettingsManager.LoadSettings();
        MainWindowViewModel = new MainWindowViewModel(settings, eventAggregator);

        WfbSettingsTabViewModel = new WfbSettingsTabViewModel(settings,eventAggregator, MainWindowViewModel);
        CameraSettingsTabViewModel = new CameraSettingsTabViewModel(settings,eventAggregator, MainWindowViewModel);
        
        Instance = this;

        _eventAggregator.GetEvent<WfbConfContentUpdatedEvent>().Subscribe(x 
            => Console.WriteLine("WfbConfContentUpdatedEvent event fired!"));
        _eventAggregator.GetEvent<MajesticContentUpdatedEvent>().Subscribe(x 
            => Console.WriteLine("MajesticContentUpdatedEvent event fired!"));

    }
    
   
}