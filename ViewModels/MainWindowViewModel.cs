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
using DynamicData.Binding;
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

    private bool _canConnect;

    public RelayCommand ConnectCommand { get; }

    // Collections
    private Dictionary<string, string> _configFileData = new Dictionary<string, string>();
    private Dictionary<string, string> _yamlConfig = new Dictionary<string, string>();


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

    public ObservableCollection<string> Resolution { get; set; }

    public ObservableCollection<string> FPS { get; set; }

    public ObservableCollection<string> Codec { get; set; }
    public ObservableCollection<string> Bitrate { get; set; }
    public ObservableCollection<string> Exposure { get; set; }
    public ObservableCollection<string> Contrast { get; set; }
    public ObservableCollection<string> Hue { get; set; }
    public ObservableCollection<string> Saturation { get; set; }
    public ObservableCollection<string> Luminance { get; set; }
    public ObservableCollection<string> Flip { get; set; }
    public ObservableCollection<string> Mirror { get; set; }

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

    // Majestic controls (Camera)

    public string SelectedResolution
    {
        get => _selectedResolution;
        set
        {
            if (_selectedResolution != value)
            {
                _selectedResolution = value;
                OnPropertyChanged(nameof(SelectedResolution));
                UpdateYamlConfig(Majestic.VideoSize, value.ToString());
            }
        }
    }

    public string SelectedFps
    {
        get => _selectedFPS;
        set
        {
            if (_selectedFPS != value)
            {
                _selectedFPS = value;
                OnPropertyChanged(nameof(SelectedFps));
                UpdateYamlConfig(Majestic.VideoFps, value.ToString());
            }
        }
    }

    public string SelectedCodec
    {
        get => _selectedCodec;
        set
        {
            if (_selectedCodec != value)
            {
                _selectedCodec = value;
                OnPropertyChanged(nameof(SelectedCodec));
                UpdateYamlConfig(Majestic.VideoCodec, value.ToString());
            }
        }
    }

    public string SelectedBitrate
    {
        get => _selectedBitrate;
        set
        {
            if (_selectedBitrate != value)
            {
                _selectedBitrate = value;
                OnPropertyChanged(nameof(SelectedBitrate));
                UpdateYamlConfig(Majestic.VideoBitrate, value.ToString());
            }
        }
    }

    public string SelectedExposure
    {
        get => _selectedExposure;
        set
        {
            if (_selectedExposure != value)
            {
                _selectedExposure = value;
                OnPropertyChanged(nameof(SelectedExposure));
                UpdateYamlConfig(Majestic.IspExposure, value.ToString());
            }
        }
    }

    public string SelectedContrast
    {
        get => _selectedContrast;
        set
        {
            if (_selectedContrast != value)
            {
                _selectedContrast = value;
                OnPropertyChanged(nameof(SelectedContrast));
                UpdateYamlConfig(Majestic.ImageContrast, value.ToString());
            }
        }
    }

    public string SelectedHue
    {
        get => _selectedHue;
        set
        {
            if (_selectedHue != value)
            {
                _selectedHue = value;
                OnPropertyChanged(nameof(SelectedHue));
                UpdateYamlConfig(Majestic.ImageHue, value.ToString());
            }
        }
    }

    public string SelectedSaturation
    {
        get => _selectedSaturation;
        set
        {
            if (_selectedSaturation != value)
            {
                _selectedSaturation = value;
                OnPropertyChanged(nameof(SelectedSaturation));
                UpdateYamlConfig(Majestic.ImageSaturation, value.ToString());
            }
        }
    }

    public string SelectedLuminance
    {
        get => _selectedLuminance;
        set
        {
            if (_selectedLuminance != value)
            {
                _selectedLuminance = value;
                OnPropertyChanged(nameof(SelectedLuminance));
                UpdateYamlConfig(Majestic.ImageLuminance, value.ToString());
            }
        }
    }

    public string SelectedFlip
    {
        get => _selectedFlip;
        set
        {
            if (_selectedFlip != value)
            {
                _selectedFlip = value;
                OnPropertyChanged(nameof(SelectedFlip));
                UpdateYamlConfig(Majestic.ImageFlip, value.ToString());
            }
        }
    }

    public string SelectedMirror
    {
        get => _selectedMirror;
        set
        {
            if (_selectedMirror != value)
            {
                _selectedMirror = value;
                OnPropertyChanged(nameof(SelectedMirror));
                UpdateYamlConfig(Majestic.ImageMirror, value.ToString());
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
        MCSIndex = new ObservableCollection<int>
        {
            1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29,
            30, 31
        };
        STBC = new ObservableCollection<int> { 0, 1 };
        LDPC = new ObservableCollection<int> { 0, 1 };
        FecK = new ObservableCollection<int> { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12 };
        FecN = new ObservableCollection<int> { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12 };

        Resolution = new ObservableCollection<string>
        {
            "1280x720",
            "1456x816",
            "1920x1080",
            "2104x1184",
            "2208x1248",
            "2240x1264",
            "2312x1304",
            "2512x1416",
            "2560x1440",
            "2560x1920",
            "3200x1800",
            "3840x2160"
        };

        FPS = new ObservableCollection<string>
        {
            "20",
            "30",
            "40",
            "50",
            "60",
            "70",
            "80",
            "90",
            "100",
            "110",
            "120"
        };

        Codec = new ObservableCollection<string>
        {
            "h264",
            "h265"
        };

        Bitrate = new ObservableCollection<string>
        {
            "1024",
            "2048",
            "3072",
            "4096",
            "5120",
            "6144",
            "7168",
            "8192",
            "9216",
            "10240",
            "11264",
            "12288",
            "13312",
            "14336",
            "15360",
            "16384",
            "17408",
            "18432",
            "19456",
            "19968"
        };

        Exposure = new ObservableCollection<string>
        {
            "5",
            "6",
            "8",
            "10",
            "11",
            "12",
            "14",
            "16",
            "33",
            "50"
        };

        Contrast = new ObservableCollection<string>
        {
            "1",
            "5",
            "10",
            "20",
            "30",
            "40",
            "50",
            "60",
            "70",
            "80",
            "90",
            "100"
        };

        Hue = new ObservableCollection<string>
        {
            "1",
            "5",
            "10",
            "20",
            "30",
            "40",
            "50",
            "60",
            "70",
            "80",
            "90",
            "100"
        };

        Saturation = new ObservableCollection<string>
        {
            "1",
            "5",
            "10",
            "20",
            "30",
            "40",
            "50",
            "60",
            "70",
            "80",
            "90",
            "100"
        };

        Luminance = new ObservableCollection<string>
        {
            "1",
            "5",
            "10",
            "20",
            "30",
            "40",
            "50",
            "60",
            "70",
            "80",
            "90",
            "100"
        };

        Flip = new ObservableCollection<string> { "true", "false" };
        Mirror = new ObservableCollection<string> { "true", "false" };
    }

    private void SetDefaultValues()
    {
        //TODO: should we do this or let the files populate this, might be less confusing

        // Selected58GHzFrequency = Frequencies58GHz[0];
        // Selected58GHzPower = Power58GHz[0];
        // Selected24GHzFrequency = Frequencies24GHz[0];
        // Selected24GHzPower = Power24GHz[0];
        // SelectedMCSIndex = MCSIndex[0];
        // SelectedSTBC = STBC[0];
        // SelectedLDPC = LDPC[0];
        // SelectedFecK = FecK[0];
        // SelectedFecN = FecN[0];

        //SelectedResolution = Resolution[2];
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
                        case Wfb.Frequency:
                            if (int.TryParse(value, out int channel_frequency))
                            {
                                string displayValue = GetFrequencyDisplayFromChannel(channel_frequency);
                                if (channel_frequency > 30)
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
        else if (filePath == OpenIPC.MAJESTIC_FILE_LOC)
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
            var value = scalarNode.Value;

            if (_yamlConfig.ContainsKey(fullKey))
            {
                _yamlConfig[fullKey] = value;
            }
            else
            {
                _yamlConfig.Add(fullKey, value);
            }

            AddLogMessage($"Found {fullKey}: {scalarNode.Value}");

            // Update UI properties based on the keys found
            switch (fullKey)
            {
                case Majestic.VideoSize:
                    SelectedResolution = value;
                    break;
                case Majestic.VideoFps:
                    SelectedFps = value;
                    break;
                case Majestic.VideoCodec:
                    SelectedCodec = value;
                    break;
                case Majestic.VideoBitrate:
                    SelectedBitrate = value;
                    break;
                case Majestic.IspExposure:
                    SelectedExposure = value;
                    break;
                case Majestic.ImageContrast:
                    SelectedContrast = value;
                    break;
                case Majestic.ImageHue:
                    SelectedHue = value;
                    break;
                case Majestic.ImageSaturation:
                    SelectedSaturation = value;
                    break;
                case Majestic.ImageLuminance:
                    SelectedLuminance = value;
                    break;
                case Majestic.ImageFlip:
                    SelectedFlip = value;
                    break;
                case Majestic.ImageMirror:
                    SelectedMirror = value;
                    break;
                default:
                    break;
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
                string remotePath = OpenIPC.WFB_CONF_FILE_LOC;
                Task<string> wfb_config =
                    _sshClientService.DownloadFileAsync(IpAddress, Username, Password, remotePath);

                // You can now parse the fileContent to match it to your UI elements
                ParseFileContent(remotePath, wfb_config.Result);

                remotePath = OpenIPC.MAJESTIC_FILE_LOC;
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

    public async Task SaveRestartMajesticCommand()
    {
        AddLogMessage("Preparing to Save Majestic file.");
        // Load the existing config file from the remote system
        var majesticYamlContent =
            await _sshClientService.DownloadFileAsync(IpAddress, Username, Password, OpenIPC.MAJESTIC_FILE_LOC);

        try
        {
            // Load the existing YAML file
            var yamlStream = new YamlStream();
            using (var reader = new StringReader(majesticYamlContent))
            {
                yamlStream.Load(reader);
            }

            var root = (YamlMappingNode)yamlStream.Documents[0].RootNode;

            // Apply changes from configUpdates
            foreach (var update in _yamlConfig)
            {
                UpdateYamlNode(root, update.Key, update.Value);
            }

            // Serialize and save the updated YAML content
            string updatedFileContent;
            using (var writer = new StringWriter())
            {
                yamlStream.Save(writer, false);
                updatedFileContent = writer.ToString();
            }

            // Upload the updated file to the remote system
            await _sshClientService.UploadFileAsync(IpAddress, Username, Password, OpenIPC.MAJESTIC_FILE_LOC,
                updatedFileContent);

            // Restart the majestic service
            await _sshClientService.ExecuteCommandAsync(IpAddress, Username, Password,
                DeviceCommands.MajesticStopCommand);
            await Task.Delay(5000); // Wait for 5 seconds
            await _sshClientService.ExecuteCommandAsync(IpAddress, Username, Password,
                DeviceCommands.MajesticStartCommand);

            Logger.Instance.Log("YAML file saved and majestic service restarted successfully.");
        }
        catch (Exception ex)
        {
            Logger.Instance.Log($"Failed to save YAML file: {ex.Message}");
        }

        AddLogMessage("Configuration saved successfully.");
    }

// Recursively update YAML node based on key path
    private void UpdateYamlNode(YamlMappingNode root, string keyPath, string newValue)
    {
        var keys = keyPath.Split('.');
        YamlMappingNode currentNode = root;

        for (int i = 0; i < keys.Length - 1; i++)
        {
            var key = keys[i];
            if (currentNode.Children.ContainsKey(new YamlScalarNode(key)))
            {
                currentNode = (YamlMappingNode)currentNode.Children[new YamlScalarNode(key)];
            }
            else
            {
                throw new KeyNotFoundException($"Key '{key}' not found in YAML.");
            }
        }

        var lastKey = keys[^1];
        if (currentNode.Children.ContainsKey(new YamlScalarNode(lastKey)))
        {
            currentNode.Children[new YamlScalarNode(lastKey)] = new YamlScalarNode(newValue);
        }
        else
        {
            throw new KeyNotFoundException($"Key '{lastKey}' not found in YAML.");
        }
    }

    public async Task SaveWfbConfigCommand()
    {
        // Load the existing config file from the remote system
        var configContent =
            await _sshClientService.DownloadFileAsync(IpAddress, Username, Password, OpenIPC.WFB_CONF_FILE_LOC);

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
        string remotePath = OpenIPC.WFB_CONF_FILE_LOC;
        await _sshClientService.UploadFileAsync(IpAddress, Username, Password, remotePath, updatedFileContent);
        await _sshClientService.ExecuteCommandAsync(IpAddress, Username, Password, DeviceCommands.WfbStopCommand);
        Thread.Sleep(5);
        await _sshClientService.ExecuteCommandAsync(IpAddress, Username, Password, DeviceCommands.WfbStartCommand);

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