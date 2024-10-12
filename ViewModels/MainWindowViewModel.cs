using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Reactive;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Animation;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.Input;

namespace OpenIPC_Config.ViewModels;

public partial class MainWindowViewModel : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    private readonly ISshClientService _sshClientService;

    // Singleton instance
    private static MainWindowViewModel _instance;

    private string _selectedDeviceType;
    private ConfigurationService _configService;

    
    // Fields for properties
    private string _selected58GHzFrequency;
    private string _selected24GHzFrequency;
    private int _selected58GHzPower;
    private int _selected24GHzPower;
    private int _selectedMCSIndex;
    private int _selectedSTBC;
    private int _selectedLDPC;
    private int _selectedFecK;
    private int _selectedFecN;

    private string _username = string.Empty;
    private string _password = string.Empty;
    private string _ipAddress = string.Empty;

    private bool _canConnect;
    
    public RelayCommand ConnectCommand { get; }

    // Collections
    private Dictionary<string, string> _configFileData = new Dictionary<string, string>();

    public ObservableCollection<string> Frequencies58GHz { get; set; } = new ObservableCollection<string>();

    public ObservableCollection<string> Frequencies24GHz { get; set; } = new ObservableCollection<string>();
    public ObservableCollection<int> Power58GHz { get; set; } = new ObservableCollection<int>();
    public ObservableCollection<int> Power24GHz { get; set; } = new ObservableCollection<int>();
    public ObservableCollection<int> MCSIndex { get; set; } = new ObservableCollection<int>();
    public ObservableCollection<int> STBC { get; set; } = new ObservableCollection<int>();
    public ObservableCollection<int> LDPC { get; set; } = new ObservableCollection<int>();
    public ObservableCollection<int> FecK { get; set; } = new ObservableCollection<int>();
    public ObservableCollection<int> FecN { get; set; } = new ObservableCollection<int>();
    private ObservableCollection<string> _logMessages = new ObservableCollection<string>();

    public DeviceConfig CurrentConfig { get; set; }
    
    
    // Properties
    public string Username
    {
        get => _username;
        set
        {
            if (_username != value)
            {
                _username = value;
                OnPropertyChanged(nameof(Username));
                CheckIfCanConnect();
            }
        }
    }

    public string Password
    {
        get => _password;
        set
        {
            if (_password != value)
            {
                _password = value;
                OnPropertyChanged(nameof(Password));
                CheckIfCanConnect();
            }
        }
    }

    public string IpAddress
    {
        get => _ipAddress;
        set
        {
            if (_ipAddress != value)
            {
                _ipAddress = value;
                OnPropertyChanged(nameof(IpAddress));
                CheckIfCanConnect();
            }
        }
    }

    public bool CanConnect
    {
        get => _canConnect;
        private set
        {
            if (_canConnect != value)
            {
                _canConnect = value;
                OnPropertyChanged(nameof(CanConnect));
                ConnectCommand.NotifyCanExecuteChanged(); // Notify command to re-evaluate CanExecute
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

    public string Selected58GHzFrequency
    {
        get => _selected58GHzFrequency;
        set
        {
            if (_selected58GHzFrequency != value)
            {
                _selected58GHzFrequency = value;
                OnPropertyChanged(nameof(Selected58GHzFrequency));
                UpdateConfigValue("channel", GetFrequencyChannelFromString(value)); // Update file content
            }
        }
    }


    public string Selected24GHzFrequency
    {
        get => _selected24GHzFrequency;
        set
        {
            if (_selected24GHzFrequency != value)
            {
                _selected24GHzFrequency = value;
                OnPropertyChanged(nameof(Selected24GHzFrequency));
            }
        }
    }

    public int Selected58GHzPower
    {
        get => _selected58GHzPower;
        set
        {
            if (_selected58GHzPower != value)
            {
                _selected58GHzPower = value;
                OnPropertyChanged(nameof(Selected58GHzPower));
            }
        }
    }

    public int Selected24GHzPower
    {
        get => _selected24GHzPower;
        set
        {
            if (_selected24GHzPower != value)
            {
                _selected24GHzPower = value;
                OnPropertyChanged(nameof(Selected24GHzPower));
            }
        }
    }

    public int SelectedMCSIndex
    {
        get => _selectedMCSIndex;
        set
        {
            if (_selectedMCSIndex != value)
            {
                _selectedMCSIndex = value;
                OnPropertyChanged(nameof(SelectedMCSIndex));
            }
        }
    }

    public int SelectedSTBC
    {
        get => _selectedSTBC;
        set
        {
            if (_selectedSTBC != value)
            {
                _selectedSTBC = value;
                OnPropertyChanged(nameof(SelectedSTBC));
            }
        }
    }

    public int SelectedLDPC
    {
        get => _selectedLDPC;
        set
        {
            if (_selectedLDPC != value)
            {
                _selectedLDPC = value;
                OnPropertyChanged(nameof(SelectedLDPC));
            }
        }
    }

    public int SelectedFecK
    {
        get => _selectedFecK;
        set
        {
            if (_selectedFecK != value)
            {
                _selectedFecK = value;
                OnPropertyChanged(nameof(SelectedFecK));
            }
        }
    }

    public int SelectedFecN
    {
        get => _selectedFecN;
        set
        {
            if (_selectedFecN != value)
            {
                _selectedFecN = value;
                OnPropertyChanged(nameof(SelectedFecN));
            }
        }
    }

    
    
    private readonly Dictionary<int, string> _58frequencyMapping = new()
    {
        { 36, "5180 MHz [36]" },
        { 40, "5200 MHz [40]" },
        { 44, "5220 MHz [44]" },
        { 48, "5240 MHz [48]" },
        { 52, "5260 MHz [52]" },
        { 56, "5280 MHz [56]" },
        { 60, "5300 MHz [60]" },
        { 64, "5320 MHz [64]" },
        { 100, "5500 MHz [100]" },
        { 104, "5520 MHz [104]" },
        { 108, "5540 MHz [108]" },
        { 112, "5560 MHz [112]" },
        { 116, "5580 MHz [116]" },
        { 120, "5600 MHz [120]" },
        { 124, "5620 MHz [124]" },
        { 128, "5640 MHz [128]" },
        { 132, "5660 MHz [132]" },
        { 136, "5680 MHz [136]" },
        { 140, "5700 MHz [140]" },
        { 144, "5720 MHz [144]" },
        { 149, "5745 MHz [149]" },
        { 153, "5765 MHz [153]" },
        { 157, "5785 MHz [157]" },
        { 161, "5805 MHz [161]" },
        { 165, "5825 MHz [165]" },
        { 169, "5845 MHz [169]" },
        { 173, "5865 MHz [173]" },
        { 177, "5885 MHz [177]" },
    };

    // Singleton Instance
    public static MainWindowViewModel Instance => _instance ??= new MainWindowViewModel();

    // Constructor
    public MainWindowViewModel()
    {
        _configService = new ConfigurationService();
        
        _sshClientService = new SshClientService();
        ConnectCommand = new RelayCommand(async () => await ExecuteConnectAsync(), () => CanConnect);
        InitializeCollections();
        SetDefaultValues();
    }
    
    

    public void AddLogMessage(string message)
    {
        string formattedMessage = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} - {message}";
        LogMessages.Insert(0, formattedMessage);
        //LogMessages.Add(message);
    }

    private void InitializeCollections()
    {
        Frequencies58GHz = new ObservableCollection<string>(_58frequencyMapping.Values);
        // Frequencies58GHz = new ObservableCollection<string>
        // {
        //     "5180 MHz [36]",
        //     "5200 MHz [40]",
        //     "5220 MHz [44]",
        //     "5240 MHz [48]",
        //     "5260 MHz [52]",
        //     "5280 MHz [56]",
        //     "5300 MHz [60]",
        //     "5320 MHz [64]",
        //     "5500 MHz [100]",
        //     "5520 MHz [104]",
        //     "5540 MHz [108]",
        //     "5560 MHz [112]",
        //     "5580 MHz [116]",
        //     "5600 MHz [120]",
        //     "5620 MHz [124]",
        //     "5640 MHz [128]",
        //     "5660 MHz [132]",
        //     "5680 MHz [136]",
        //     "5700 MHz [140]",
        //     "5720 MHz [144]",
        //     "5745 MHz [149]",
        //     "5765 MHz [153]",
        //     "5785 MHz [157]",
        //     "5805 MHz [161]",
        //     "5825 MHz [165]",
        //     "5845 MHz [169]",
        //     "5865 MHz [173]",
        //     "5885 MHz [177]" 
        //     
        // };

        Frequencies24GHz = new ObservableCollection<string>
        {
            "2412 MHz [1]", "2417 MHz [2]", "2422 MHz [3]", "2427 MHz [4]",
            // Add other frequencies here
        };

        Power58GHz = new ObservableCollection<int> { 1, 5, 10, 15, 20 };
        Power24GHz = new ObservableCollection<int> { 20, 25, 30, 35, 40 };
        MCSIndex = new ObservableCollection<int> { 0, 1, 2, 3, 4, 5, 6, 7 };
        STBC = new ObservableCollection<int> { 0, 1 };
        LDPC = new ObservableCollection<int> { 0, 1 };
        FecK = new ObservableCollection<int> { 0, 1, 2, 3 };
        FecN = new ObservableCollection<int> { 0, 1, 2, 3 };
    }

    private void SetDefaultValues()
    {
        Selected58GHzFrequency = Frequencies58GHz[0];
        Selected58GHzPower = Power58GHz[0];
        Selected24GHzFrequency = Frequencies24GHz[0];
        Selected24GHzPower = Power24GHz[0];
        SelectedMCSIndex = MCSIndex[0];
        SelectedSTBC = STBC[0];
        SelectedLDPC = LDPC[0];
        SelectedFecK = FecK[0];
        SelectedFecN = FecN[0];
    }
    
    public string SelectedDeviceType
    {
        get => _selectedDeviceType;
        
        set
        {
            if (_selectedDeviceType != value)
            {
                _selectedDeviceType = value;
                OnPropertyChanged(nameof(SelectedDeviceType));
                CheckIfCanConnect();
                LoadSelectedConfig();
            }
        }
        
    }
    private void LoadSelectedConfig()
    {
        CurrentConfig = _configService.GetConfig(SelectedDeviceType);
    
        // Notify property changes for bindings
        OnPropertyChanged(nameof(CurrentConfig));
        OnPropertyChanged(nameof(CurrentConfig.Username));
        OnPropertyChanged(nameof(CurrentConfig.Password));
        OnPropertyChanged(nameof(CurrentConfig.IpAddress));
    }
    
    public void SaveConfig()
    {
        _configService.SaveConfig(SelectedDeviceType, CurrentConfig);
    }


    private void CheckIfCanConnect()
    {
        CanConnect = !string.IsNullOrWhiteSpace(Username)
                     && !string.IsNullOrWhiteSpace(Password)
                     && !string.IsNullOrWhiteSpace(IpAddress)
                     && (!string.IsNullOrWhiteSpace(SelectedDeviceType));

        //Console.WriteLine($"CanConnect: {CanConnect}, Username: {Username}, Password: {Password}, IP: {IpAddress}, DeviceType: {SelectedDeviceType}");
        //AddLogMessage($"CanConnect: {CanConnect}, Username: {Username}, Password: {Password}, IP: {IpAddress}, DeviceType: {SelectedDeviceType}()");  
        AddLogMessage($"CanConnect: {CanConnect}, Username: {Username}, Password: *****, IP: {IpAddress}, DeviceType: {SelectedDeviceType}()");    ;
    }

    private void ParseFileContent(string filePath, string content)
    {
        // Implement your logic to match file content to UI properties.
        if (filePath == "/etc/wfb.conf")
        {
            // Example parsing logic:
            // Assume the content is in key=value format
            var lines = content.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var line in lines)
            {
                var parts = line.Split('=');
                if (parts.Length == 2)
                {
                    string key = parts[0].Trim();
                    string value = parts[1].Trim();

                    // Match the key to your UI properties
                    switch (key)
                    {
                        case "channel":
                            if (int.TryParse(value, out int channel))
                            {
                                // Check if the channel number has a corresponding frequency string
                                if (_58frequencyMapping.TryGetValue(channel, out var displayValue))
                                {
                                    Logger.Instance.Log($"Found matching frequency: {displayValue}");
                                    Selected58GHzFrequency = displayValue;
                                }
                                else
                                {
                                    Logger.Instance.Log($"No matching frequency found for channel: {channel}");
                                }
                            }

                            break;

                        // Handle other configuration parameters here
                    }
                }
            }
        }
    }


    private async Task ExecuteConnectAsync()
    {
        try
        {
            await Dispatcher.UIThread.InvokeAsync(() =>
            {
                // Update UI to show connecting
                AddLogMessage("Attempting to connect.");
            });

            await Task.Run(() =>
            {
                // Perform actual SSH/SCP connection
                string remotePath = "/etc/wfb.conf";
                Task<string> wfb_config =
                    _sshClientService.DownloadFileAsync(IpAddress, Username, Password, remotePath);
                //AddLogMessage($"File contents: {wfb_config.Result}");

                // You can now parse the fileContent to match it to your UI elements
                ParseFileContent(remotePath, wfb_config.Result);
            });

            await Dispatcher.UIThread.InvokeAsync(() =>
            {
                // Update UI to show connection success
                AddLogMessage("Connected successfully.");
            });
        }
        catch (Exception ex)
        {
            await Dispatcher.UIThread.InvokeAsync(() => { AddLogMessage($"Connection failed: {ex.Message}"); });
        }
    }

    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public async Task SaveConfigCommand()
    {
        // Load the existing config file from the remote system
        var configContent = await _sshClientService.DownloadFileAsync(IpAddress, Username, Password, "/etc/wfb.conf");

        // Convert the file content to a dictionary of key-value pairs
        var existingConfig = ParseConfigFile(configContent);

        // Merge the existing config with the in-memory _configFileData (new or modified values)
        foreach (var entry in _configFileData)
        {
            if (existingConfig.ContainsKey(entry.Key))
            {
                // Update the value for existing keys
                existingConfig[entry.Key] = entry.Value;
            }
            else
            {
                // Add new keys
                existingConfig.Add(entry.Key, entry.Value);
            }
        }

        // Convert the merged dictionary back into key=value format
        List<string> fileLines = new List<string>();
        foreach (var entry in existingConfig)
        {
            fileLines.Add($"{entry.Key}={entry.Value}");
        }

        string updatedFileContent = string.Join(Environment.NewLine, fileLines);

        // Upload the updated file to the remote system
        string remotePath = "/etc/wfb.conf"; // Assuming this is the path
        await _sshClientService.UploadFileAsync(IpAddress, Username, Password, remotePath, updatedFileContent);
        await _sshClientService.ExecuteCommandAsync(IpAddress, Username, Password, CameraCommands.WfbStopCommand);
        Thread.Sleep(5);
        await _sshClientService.ExecuteCommandAsync(IpAddress, Username, Password, CameraCommands.WfbStartCommand);

        AddLogMessage("Configuration saved successfully.");
    }

// Helper method to parse the config file content into a dictionary
    private Dictionary<string, string> ParseConfigFile(string fileContent)
    {
        var config = new Dictionary<string, string>();

        using (StringReader reader = new StringReader(fileContent))
        {
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                if (string.IsNullOrWhiteSpace(line) || !line.Contains("="))
                    continue;

                var parts = line.Split('=', 2);
                var key = parts[0].Trim();
                var value = parts[1].Trim();

                config[key] = value;
            }
        }

        return config;
    }

    private void UpdateConfigValue(string key, object value)
    {
        if (_configFileData.ContainsKey(key))
        {
            _configFileData[key] = value.ToString();
        }
        else
        {
            _configFileData.Add(key, value.ToString());
        }
    }

    // Helper method to map the frequency string back to the channel number
    private int GetFrequencyChannelFromString(string frequencyString)
    {
        foreach (var entry in _58frequencyMapping)
        {
            if (entry.Value == frequencyString)
                return entry.Key;
        }

        return -1; // or handle error if no match is found
    }
    
    
}