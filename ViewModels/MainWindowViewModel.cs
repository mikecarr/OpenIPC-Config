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
using YamlDotNet.RepresentationModel;

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

    private int _selectedResolution;
    private int _selectedFPS;

    private int _selectedCodec;
    private int _selectedBitrate;
    private int _selectedExposure;
    private int _selectedContrast;
    private int _selectedHUE;
    private int _selectedSaturation;
    private int _selectedLuminance;
    private Boolean _selectedFlip;
    private int _selectedMirror;
    
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
    
    public ObservableCollection<string> Resolution { get; set; } = new ObservableCollection<string>();
    
    public ObservableCollection<int> FPS { get; set; } = new ObservableCollection<int>();
    
    public ObservableCollection<int> Codec { get; set; } = new ObservableCollection<int>();
    public ObservableCollection<int> Bitrate { get; set; } = new ObservableCollection<int>();
    public ObservableCollection<int> Exposure { get; set; } = new ObservableCollection<int>();
    public ObservableCollection<int> Contrast { get; set; } = new ObservableCollection<int>();
    public ObservableCollection<int> HUE { get; set; } = new ObservableCollection<int>();
    public ObservableCollection<int> Saturation { get; set; } = new ObservableCollection<int>();
    public ObservableCollection<int> Luminance { get; set; } = new ObservableCollection<int>();
    public ObservableCollection<Boolean> Flip { get; set; } = new ObservableCollection<Boolean>();
    public ObservableCollection<int> Mirror { get; set; } = new ObservableCollection<int>();
    
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
                int channel = GetFrequencyChannelFromString(value);
                UpdateConfigValue("channel", channel); 
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
                int channel = GetFrequencyChannelFromString(value);
                UpdateConfigValue("channel", channel);
                
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
                UpdateConfigValue("driver_txpower_override", value.ToString()); 
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
                UpdateConfigValue("txpower", value.ToString());
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
                UpdateConfigValue("mcs_index", value.ToString());
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
                UpdateConfigValue("stbc", value.ToString());
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
                UpdateConfigValue("ldpc", value.ToString());
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
                UpdateConfigValue("fec_k", value.ToString());
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
                UpdateConfigValue("fec_n", value.ToString());
            }
        }
    }

    public int SelectedFPS
    {
        get => _selectedFPS;
        set
        {
            if (_selectedFPS != value)
            {
                _selectedFPS = value;
                OnPropertyChanged(nameof(_selectedFPS));
                UpdateConfigValue("fps", value.ToString());
            }
        }
    }
    public int SelectedCodec
    {
        get => _selectedCodec;
        set
        {
            if (_selectedCodec != value)
            {
                _selectedCodec = value;
                OnPropertyChanged(nameof(_selectedCodec));
                UpdateConfigValue("codec", value.ToString());
            }
        }
    }
    
    public int  SelectedBitrate
    {
        get => _selectedBitrate;
        set
        {
            if (_selectedBitrate != value)
            {
                _selectedBitrate = value;
                OnPropertyChanged(nameof(_selectedBitrate));
                UpdateConfigValue("bitrate", value.ToString());
            }
        }
    }
    
    public int SelectedExposure
    {
        get => _selectedExposure;
        set
        {
            if (_selectedExposure != value)
            {
                _selectedExposure = value;
                OnPropertyChanged(nameof(_selectedExposure));
                UpdateConfigValue("bitrate", value.ToString());
            }
        }
    }
    
    public int SelectedContrast
    {
        get => _selectedContrast;
        set
        {
            if (_selectedContrast != value)
            {
                _selectedContrast = value;
                OnPropertyChanged(nameof(_selectedContrast));
                UpdateConfigValue("contrast", value.ToString());
            }
        }
    }
    public int SelectedHUE
    {
        get => _selectedHUE;
        set
        {
            if (_selectedHUE != value)
            {
                _selectedHUE = value;
                OnPropertyChanged(nameof(_selectedHUE));
                UpdateConfigValue("hue", value.ToString());
            }
        }
    }
    
    public int SelectedSaturation
    {
        get => _selectedSaturation;
        set
        {
            if (_selectedSaturation != value)
            {
                _selectedSaturation = value;
                OnPropertyChanged(nameof(_selectedSaturation));
                UpdateConfigValue("saturation", value.ToString());
            }
        }
    }
    
    public int SelectedLuminance
    {
        get => _selectedLuminance;
        set
        {
            if (_selectedLuminance != value)
            {
                _selectedLuminance = value;
                OnPropertyChanged(nameof(_selectedLuminance));
                UpdateConfigValue("luminance", value.ToString());
            }
        }
    }
    
    public Boolean SelectedFlip
    {
        get => _selectedFlip;
        set
        {
            if (_selectedFlip != value)
            {
                _selectedFlip = value;
                OnPropertyChanged(nameof(_selectedFlip));
                UpdateConfigValue("flip", value.ToString());
            }
        }
    }
    
    public int SelectedMirror
    {
        get => _selectedMirror;
        set
        {
            if (_selectedMirror != value)
            {
                _selectedMirror = value;
                OnPropertyChanged(nameof(_selectedMirror));
                UpdateConfigValue("mirror", value.ToString());
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
    private readonly Dictionary<int, string> _24frequencyMapping = new()
    {
        { 1, "2412 MHz [1]" },
        { 2, "2417 MHz [2]" },
        { 3, "2422 MHz [3]" },
        { 4, "2427 MHz [4]" },
        { 5, "2432 MHz [5]" },
        { 6, "2437 MHz [6]" },
        { 7, "2442 MHz [7]" },
        { 8, "2447 MHz [8]" },
        { 9, "2452 MHz [9]" },
        { 10, "2457 MHz [10]" },
        { 11, "2462 MHz [11]" },
        { 12, "2467 MHz [12]" },
        { 13, "2472 MHz [13]" },
        { 14, "2484 MHz [14]" }
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
    }

    private void InitializeCollections()
    {
        Frequencies58GHz = new ObservableCollection<string>(_58frequencyMapping.Values);
        Frequencies24GHz = new ObservableCollection<string>(_24frequencyMapping.Values);

        

        Power58GHz = new ObservableCollection<int> { 1, 5, 10, 15, 20, 25 };
        Power24GHz = new ObservableCollection<int> { 20, 25, 30, 35, 40 };
        MCSIndex = new ObservableCollection<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31};
        STBC = new ObservableCollection<int> { 0, 1 };
        LDPC = new ObservableCollection<int> { 0, 1 };
        FecK = new ObservableCollection<int> { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12 };
        FecN = new ObservableCollection<int> { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12 };
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
        
        //AddLogMessage($"CanConnect: {CanConnect}, Username: {Username}, Password: *****, IP: {IpAddress}, DeviceType: {SelectedDeviceType}()");    ;
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
                        case Wfb.Channel:
                            if (int.TryParse(value, out int channel))
                            {
                                string displayValue = GetFrequencyDisplayFromChannel(channel);
                                if (channel > 30)
                                {
                                    if (!string.IsNullOrEmpty(displayValue))
                                    {
                                        Selected58GHzFrequency = displayValue;
                                    }
                                }
                                else
                                {
                                    if (!string.IsNullOrEmpty(displayValue))
                                    {
                                        _selected24GHzFrequency = displayValue;
                                    }

                                }
                            }

                            break;
                        case Wfb.DriverTxpowerOverride:
                            if (int.TryParse(value, out int tx_power))
                            {
                                Selected58GHzPower = tx_power;
                                AddLogMessage($"Found matching power: {tx_power}");
                            }

                            break;
                        case Wfb.Txpower:
                            if (int.TryParse(value, out int txpower))
                            {
                                Selected24GHzPower = txpower;
                                AddLogMessage($"Found matching tx_power: {txpower}");
                            }

                            break;
                        case Wfb.Stbc:
                            if (int.TryParse(value, out int stbc))
                            {
                                SelectedSTBC = stbc;
                                AddLogMessage($"Found matching value stbc: {stbc}");
                            }

                            break;
                        case Wfb.Ldpc:
                            if (int.TryParse(value, out int ldpc))
                            {
                                SelectedLDPC = ldpc;
                                AddLogMessage($"Found matching value ldpc: {ldpc}");
                            }

                            break;
                        case Wfb.McsIndex:
                            if (int.TryParse(value, out int mcs_index))
                            {
                                SelectedMCSIndex = mcs_index;
                                AddLogMessage($"Found matching value mcs_index: {mcs_index}");
                            }

                            break;
                        case Wfb.FecK:
                            if (int.TryParse(value, out int fec_k))
                            {
                                SelectedFecK = fec_k;
                                AddLogMessage($"Found matching value fec_k: {fec_k}");
                            }

                            break;
                        case Wfb.FecN:
                            if (int.TryParse(value, out int fec_n))
                            {
                                SelectedFecK = fec_n;
                                AddLogMessage($"Found matching value fec_n: {fec_n}");
                            }

                            break;

                        // Handle other configuration parameters here
                    }
                }
            }
        }
        else if (filePath == "/etc/majestic.yaml")
        {
            using var reader = new StringReader(content);
            var yaml = new YamlStream();
            yaml.Load(reader);

            var root = (YamlMappingNode)yaml.Documents[0].RootNode;
            foreach (var entry in root.Children)
            {
                ParseYamlNode(entry.Key.ToString(), entry.Value);
            }
        }
    }

    private void ParseYamlNode(string parentKey, YamlNode node)
    {
        if (node is YamlMappingNode mappingNode)
        {
            foreach (var child in mappingNode.Children)
            {
                string childKey = child.Key.ToString();
                ParseYamlNode($"{parentKey}.{childKey}", child.Value);
            }
        }
        else if (node is YamlScalarNode scalarNode)
        {
            string fullKey = parentKey;
            
            if (fullKey == "video0.size")
            {
                Logger.Instance.Log($"Found video0.size: {scalarNode.Value}");
                
            }
            
            else
            {
                Logger.Instance.Log($"{fullKey}: {scalarNode.Value}");
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
                
                // You can now parse the fileContent to match it to your UI elements
                ParseFileContent(remotePath, wfb_config.Result);
                
                remotePath = "/etc/majestic.yaml";
                Task<string> majestic_yaml =
                    _sshClientService.DownloadFileAsync(IpAddress, Username, Password, remotePath);

                ParseFileContent(remotePath, majestic_yaml.Result);
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
    private string GetFrequencyDisplayFromChannel(int channel)
    {
        if (_58frequencyMapping.TryGetValue(channel, out string displayValue))
        {
            return displayValue;
        }

        if (_24frequencyMapping.TryGetValue(channel, out displayValue))
        {
            return displayValue;
        }

        return string.Empty; // No match found
    }
    
    private int GetFrequencyChannelFromString(string frequencyString)
    {
        foreach (var entry in _58frequencyMapping)
        {
            if (entry.Value == frequencyString)
                return entry.Key;
        }
        foreach (var entry in _24frequencyMapping)
        {
            if (entry.Value == frequencyString)
                return entry.Key;
        }


        return -1; // or handle error if no match is found
    }
    
}