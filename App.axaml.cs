using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core;
using Avalonia.Data.Core.Plugins;
using Avalonia.Markup.Xaml;
using OpenIPC_Config.Messages;
using OpenIPC_Config.Services;
using OpenIPC_Config.ViewModels;
using OpenIPC_Config.Views;
using Prism.Events;

namespace OpenIPC_Config;

public partial class App : Application
{
    private IEventAggregator _eventAggregator;
    
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            // Remove Avalonia's default data validation to avoid duplication
            BindingPlugins.DataValidators.RemoveAt(0);

            // Load settings from the settings file
            var settings = SettingsManager.LoadSettings();
            
            // Create a single instance of the EventAggregator
            _eventAggregator = new EventAggregator();
            _eventAggregator.GetEvent<WfbConfContentUpdatedEvent>().Subscribe((message) =>
            {
                // Handle the event here
                System.Diagnostics.Debug.WriteLine("Received event: " + message);
            });
            
            // Create the MainWindow and set its DataContext with loaded settings
            desktop.MainWindow = new MainWindow(new ViewModelLocator(_eventAggregator))
            {
                DataContext = new MainWindowViewModel(settings,_eventAggregator), // Pass settings to ViewModel
            };
        }

        
        base.OnFrameworkInitializationCompleted();
    }
    

}