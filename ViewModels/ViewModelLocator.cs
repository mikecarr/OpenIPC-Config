using System;
using OpenIPC_Config.Messages;
using OpenIPC_Config.ViewModels;
using OpenIPC_Config.Views;
using Prism.Events;
using Prism.Ioc;

namespace OpenIPC_Config.ViewModels;

public class ViewModelLocator
{
    public static ViewModelLocator Instance { get; private set; }
    public static MessageViewModel MessageViewModel { get; private set; }
    
    public WfbSettingsTabViewModel WfbSettingsTabViewModel { get; }


    private IEventAggregator _eventAggregator;
    

    
    public ViewModelLocator(IEventAggregator eventAggregator)
    {
        _eventAggregator = eventAggregator;
        
        MessageViewModel = new MessageViewModel(eventAggregator);
        WfbSettingsTabViewModel = new WfbSettingsTabViewModel(eventAggregator);
        
        Instance = this;

        _eventAggregator.GetEvent<WfbConfContentUpdatedEvent>().Subscribe(x 
            => Console.WriteLine("Here"));

    }
    
    public IEventAggregator EventAggregator => _eventAggregator;
}