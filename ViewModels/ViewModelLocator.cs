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
    
    public TelemetryTabViewModel TelemetryTabViewModel { get; }
    
    

    public IEventAggregator EventAggregator => _eventAggregator;
    private IEventAggregator _eventAggregator;
    

    
    public ViewModelLocator(IEventAggregator eventAggregator)
    {
        Instance = this;
        _eventAggregator = eventAggregator;
        var settings = SettingsManager.LoadSettings();
        
        MessageViewModel = new MessageViewModel(eventAggregator);
        MainWindowViewModel = new MainWindowViewModel(settings, eventAggregator);

        WfbSettingsTabViewModel = new WfbSettingsTabViewModel(settings,eventAggregator, MainWindowViewModel);
        CameraSettingsTabViewModel = new CameraSettingsTabViewModel(settings,eventAggregator, MainWindowViewModel);
        TelemetryTabViewModel = new TelemetryTabViewModel(settings,eventAggregator, MainWindowViewModel);

        _eventAggregator.GetEvent<WfbConfContentUpdatedEvent>().Subscribe(x 
            => Logger.Instance().Log("**** WfbConfContentUpdatedEvent event fired!"));
        _eventAggregator.GetEvent<MajesticContentUpdatedEvent>().Subscribe(x 
            => Logger.Instance().Log("**** MajesticContentUpdatedEvent event fired!"));
        _eventAggregator.GetEvent<TelemetryContentUpdatedEvent>().Subscribe(x 
            => Logger.Instance().Log("**** TelemetryContentUpdatedEvent event fired!"));
        _eventAggregator.GetEvent<MessageEvent>().Subscribe(x 
            => Logger.Instance().Log("**** MessageEvent event fired!"));

    }
    
   
}