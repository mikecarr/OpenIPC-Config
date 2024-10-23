using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Reactive;
using System.Reactive.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.Input;
using OpenIPC_Config.Messages;
using OpenIPC_Config.Models;
using OpenIPC_Config.Services;
using OpenIPC_Config.Views;
using Prism.Events;
using ReactiveUI;
using YamlDotNet.RepresentationModel;

namespace OpenIPC_Config.ViewModels;

public partial class MainWindowViewModel : ObservableObject
{
    public event PropertyChangedEventHandler? PropertyChanged;

    private readonly IEventAggregator _eventAggregator;
    
    public ICommand SaveAndConnectCommand { get; }
    
    private readonly SshClientService _sshClientService;
    private string _selectedDeviceType;
    private ConfigurationService _configService;

    private string _selectedResolution;
    private string _selectedFPS;
    private string _selectedCodec;
    private string _selectedBitrate;
    private string _selectedExposure;
    private string _selectedContrast;
    private string _selectedHue;
    private string _selectedSaturation;
    private string _selectedLuminance;
    private string _selectedFlip;
    private string _selectedMirror;

    private string _username = string.Empty;
    private string _password = string.Empty;
    private string _ipAddress = string.Empty;
    private DeviceType _deviceType = DeviceType.None;

    private DeviceConfig _deviceConfig;

    private bool _canConnect;
    private Dictionary<string, string> _yamlConfig = new Dictionary<string, string>();
    private ObservableCollection<string> _logMessages = new ObservableCollection<string>();

    
    public DeviceConfig CurrentConfig { get; set; }

    public string Username
    {
        get => _username;
        set
        {
            SetField(ref _username, value);
            CheckIfCanConnect();
        }
    }

    public string Password
    {
        get => _password;
        set
        {
            SetField(ref _password, value);
            CheckIfCanConnect();
        }
    }

    public string IpAddress
    {
        get => _ipAddress;
        set
        {
            SetField(ref _ipAddress, value);
            CheckIfCanConnect();
        }
    }

    public DeviceType DeviceType
    {
        get => _deviceType;
        set
        {
            SetField(ref _deviceType, value);
            CheckIfCanConnect();
        }
    }

    public bool CanConnect
    {
        get => _canConnect;
        private set
        {
            if (SetField(ref _canConnect, value, nameof(CanConnect)))
            {
                //ConnectCommand.NotifyCanExecuteChanged(); // Notify command to re-evaluate CanExecute
            }
        }
    }

    public ObservableCollection<string> LogMessages
    {
        get => _logMessages;
        set
        {
            _logMessages = value;
            OnPropertyChanged(nameof(LogMessages));
        }
    }

    
    

    private static void RestartWfb()
    {
        string c = "";
        
        
    }
    
    public MainWindowViewModel(DeviceConfig deviceConfig, IEventAggregator eventAggregator)
    {
        _deviceConfig = deviceConfig;
        _eventAggregator = eventAggregator;


        _eventAggregator.GetEvent<WfbConfContentUpdatedEvent>().Subscribe((message) =>
        {
            // Handle the event here
            System.Diagnostics.Debug.WriteLine("Received event: " + message);
        });
        
        IpAddress = deviceConfig.IpAddress;
        Username = deviceConfig.Username;
        Password = deviceConfig.Password;
        DeviceType = deviceConfig.DeviceType;

        _sshClientService = new SshClientService();

        InitializeCollections();
        SetDefaultValues();
    
        // Ensure SaveAndConnectCommand runs on the UI thread  (Connect Button)
        SaveAndConnectCommand = new RelayCommand(SaveAndConnect);
    }

    public string SelectedDeviceType
    {
        get => _selectedDeviceType;
        
        set
        {
            if (_selectedDeviceType != value)
            {
                _selectedDeviceType = value;
                //OnPropertyChanged(nameof(SelectedDeviceType));
                CheckIfCanConnect();
                //LoadSelectedConfig();
            }
        }
        
    }
    
    // Method to save settings
    private void SaveSettings()
    {
        DeviceConfig deviceConfig = new DeviceConfig()
        {
            IpAddress = this.IpAddress,
            Username = this.Username,
            Password = this.Password,
            DeviceType = this.DeviceType
        };
        SettingsManager.SaveSettings(deviceConfig);

        _deviceConfig = new DeviceConfig();
        _deviceConfig.IpAddress = deviceConfig.IpAddress;
        _deviceConfig.Username = deviceConfig.Username;
        _deviceConfig.Password = deviceConfig.Password;
        _deviceConfig.DeviceType = deviceConfig.DeviceType; 
        
        
        Logger.Instance.Log("Settings saved.");

    }

    public void AddLogMessage(string message)
    {
        Dispatcher.UIThread.InvokeAsync(() =>
        {
            string formattedMessage = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} - {message}";
            LogMessages.Insert(0, formattedMessage); // Safely updating collection on the UI thread
        });
    }

    private void CheckIfCanConnect()
    {
        Dispatcher.UIThread.InvokeAsync(() =>
        {
            CanConnect = !string.IsNullOrWhiteSpace(Username)
                         && !string.IsNullOrWhiteSpace(Password)
                         && !string.IsNullOrWhiteSpace(IpAddress)
                         && (!string.IsNullOrWhiteSpace(SelectedDeviceType));
            
            Console.WriteLine($"CanConnect: {CanConnect}, Username: {Username}, Password: *****, IP: {IpAddress}, DeviceType: {SelectedDeviceType}()");
            AddLogMessage($"CanConnect: {CanConnect}, Username: {Username}, Password: *****, IP: {IpAddress}, DeviceType: {SelectedDeviceType}()");
        });
    }

    private void InitializeCollections()
    {
        
    }

    private void SetDefaultValues()
    {
        //IpAddress = "192.168.1.10";
        IpAddress = "10.100.0.199";
        Username = "root";
    }

    // YAML parsing and updating methods (unchanged)
    private void ParseYamlConfig(string content) { /* Your existing YAML parsing logic */ }
    private void UpdateYamlNode(YamlMappingNode root, string keyPath, string newValue) { /* Your existing node update logic */ }

    public async Task SaveRestartMajesticCommand()
    {
        AddLogMessage("Preparing to Save Majestic file.");
        var majesticYamlContent = await _sshClientService.DownloadFileAsync(_deviceConfig, OpenIPC.MAJESTIC_FILE_LOC);

        try
        {
            var yamlStream = new YamlStream();
            using (var reader = new StringReader(majesticYamlContent))
            {
                yamlStream.Load(reader);
            }

            var root = (YamlMappingNode)yamlStream.Documents[0].RootNode;

            foreach (var update in _yamlConfig)
            {
                UpdateYamlNode(root, update.Key, update.Value);
            }

            string updatedFileContent;
            using (var writer = new StringWriter())
            {
                yamlStream.Save(writer, false);
                updatedFileContent = writer.ToString();
            }

            await _sshClientService.UploadFileAsync(_deviceConfig, OpenIPC.MAJESTIC_FILE_LOC, updatedFileContent);
            await _sshClientService.ExecuteCommandAsync(_deviceConfig, DeviceCommands.MajesticStopCommand);
            await Task.Delay(5000);
            await _sshClientService.ExecuteCommandAsync(_deviceConfig, DeviceCommands.MajesticStartCommand);

            AddLogMessage("YAML file saved and majestic service restarted successfully.");
        }
        catch (Exception ex)
        {
            AddLogMessage($"Failed to save YAML file: {ex.Message}");
        }
    }

    public void UpdateYamlConfig(string key, string newValue)
    {
        if (_yamlConfig.ContainsKey(key))
        {
            _yamlConfig[key] = newValue;
        }
        else
        {
            _yamlConfig.Add(key, newValue);
        }
    }

    private void SaveAndConnect()
    {
        SaveSettings();
        
        Connect(_deviceConfig, content => {
            // WfbSettingsTabViewModel.WfbConfContent = content;
        });
    }

    private async void Connect(DeviceConfig deviceConfig, Action<string> onFileDownloaded)
    {
        
        String wfbConfContent = await _sshClientService.DownloadFileAsync(deviceConfig, OpenIPC.WFB_CONF_FILE_LOC);
        
        // Publish a message to WfbSettingsTabViewModel
        _eventAggregator.GetEvent<WfbConfContentUpdatedEvent>()
            .Publish(new WfbConfContentUpdatedMessage(wfbConfContent));
        

    }
}
