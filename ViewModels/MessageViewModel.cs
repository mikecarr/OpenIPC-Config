using System;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using OpenIPC_Config.Messages;
using Prism.Events;
using CommunityToolkit.Mvvm.ComponentModel;

namespace OpenIPC_Config.ViewModels;

public class MessageViewModel : ObservableObject
{
    private readonly IEventAggregator _eventAggregator;
    private string _messageText;


    public MessageViewModel() : this(new EventAggregator()) { }
    
    public MessageViewModel(IEventAggregator eventAggregator)
    {
        _eventAggregator = eventAggregator;
        _eventAggregator.GetEvent<MessageEvent>().Subscribe(OnMessageReceived);
    }

    public string MessageText
    {
        get => _messageText;
        set => SetField(ref _messageText, value);
    }
    
    private void OnMessageReceived(Message message)
    {
        if (MessageText != null)
        {
            MessageText += "\n" + message.Text;
        }
        else
        {
            MessageText = message.Text;
        }
    }
    
    public void LogMessage(string message)
    {
        var logMessage = new Message($"{DateTime.Now.ToString()} - {message}");
        _eventAggregator.GetEvent<MessageEvent>().Publish(logMessage);
    }
}