using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using ExCSS;
using OpenIPC_Config.Messages;
using OpenIPC_Config.Models;
using OpenIPC_Config.Services;
using Prism.Events;
using ReactiveUI;
using YamlDotNet.RepresentationModel;

namespace OpenIPC_Config.ViewModels;

public class CameraSettingsTabViewModel : ViewModelBase
{
    private readonly MainWindowViewModel _mainWindowViewModel;
    
    private readonly ISshClientService _sshClientService;

    private IEventAggregator _eventAggregator;

    private DeviceConfig _deviceConfig;
    
    private Dictionary<string, string> _yamlConfig = new Dictionary<string, string>();
    
    private bool _canConnect;

    public bool CanConnect
    {
        get => _canConnect;
        set
        {
            this.RaiseAndSetIfChanged(ref _canConnect, value);
            Logger.Instance().Log($"CanConnect {value}");
        }
    }
    
    private async void RestartMajestic(MainWindowViewModel mainWindowViewModel)
    {
        Logger.Instance().Log("*** TODO : RestartMajesticCommand executed");

        await SaveRestartMajesticCommand();
    }

    public ICommand RestartMajesticCommand { get; private set; }
    // ObservableCollections
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
    
    private string _selectedResolution;
    public string SelectedResolution
    {
        get => _selectedResolution;
        set
        {
            this.RaiseAndSetIfChanged(ref _selectedResolution, value);
            Logger.Instance().Log($"SelectedResolution updated to {value}");
            UpdateYamlConfig(Majestic.VideoSize, value.ToString());
        }
    }

    private string _selectedFps;
    public string SelectedFps
    {
        get => _selectedFps;
        set
        {
            this.RaiseAndSetIfChanged(ref _selectedFps, value);
            Logger.Instance().Log($"SelectedFps updated to {value}");
            UpdateYamlConfig(Majestic.VideoFps, value.ToString());
        }
    }
    
    private string _selectedCodec;
    public string SelectedCodec
    {
        get => _selectedCodec;
        set
        {
            this.RaiseAndSetIfChanged(ref _selectedCodec, value);
            Logger.Instance().Log($"SelectedCodec updated to {value}");
            UpdateYamlConfig(Majestic.VideoCodec, value.ToString());
        }
    }
    
    private string _selectedBitrate;
    public string SelectedBitrate
    {
        get => _selectedBitrate;
        set
        {
            this.RaiseAndSetIfChanged(ref _selectedBitrate, value);
            Logger.Instance().Log($"SelectedBitrate updated to {value}");
            UpdateYamlConfig(Majestic.VideoBitrate, value.ToString());
        }
    }

    private string _selectedExposure;
    public string SelectedExposure
    {
        get => _selectedExposure;
        set
        {
            this.RaiseAndSetIfChanged(ref _selectedExposure, value);
            Logger.Instance().Log($"SelectedExposure updated to {value}");
            UpdateYamlConfig(Majestic.IspExposure, value.ToString());
        }
    }

    private string _selectedHue;
    public string SelectedHue
    {
        get => _selectedHue;
        set
        {
            this.RaiseAndSetIfChanged(ref _selectedHue, value);
            Logger.Instance().Log($"SelectedHue updated to {value}");
            UpdateYamlConfig(Majestic.ImageHue, value.ToString());
        }
    }
    
    private string _selectedContrast;
    public string SelectedContrast
    {
        get => _selectedContrast;
        set
        {
            this.RaiseAndSetIfChanged(ref _selectedContrast, value);
            Logger.Instance().Log($"SelectedContrast updated to {value}");
            UpdateYamlConfig(Majestic.ImageContrast, value.ToString());
        }
    }

    private string _selectedSaturation;
    public string SelectedSaturation
    {
        get => _selectedSaturation;
        set
        {
            this.RaiseAndSetIfChanged(ref _selectedSaturation, value);
            Logger.Instance().Log($"SelectedSaturation updated to {value}");
            UpdateYamlConfig(Majestic.ImageSaturation, value.ToString());
        }
    }

    private string _selectedLuminance;
    public string SelectedLuminance
    {
        get => _selectedLuminance;
        set
        {
            this.RaiseAndSetIfChanged(ref _selectedLuminance, value);
            Logger.Instance().Log($"SelectedLuminance updated to {value}");
            UpdateYamlConfig(Majestic.ImageLuminance, value.ToString());
        }
    }

    private string _selectedFlip;
    public string SelectedFlip
    {
        get => _selectedFlip;
        set
        {
            this.RaiseAndSetIfChanged(ref _selectedFlip, value);
            Logger.Instance().Log($"SelectedFlip updated to {value}");
            UpdateYamlConfig(Majestic.ImageFlip, value.ToString());
        }
    }

    private string _selectedMirror;
    public string SelectedMirror
    {
        get => _selectedMirror;
        set
        {
            this.RaiseAndSetIfChanged(ref _selectedMirror, value);
            Logger.Instance().Log($"SelectedMirror updated to {value}");
            UpdateYamlConfig(Majestic.ImageMirror, value.ToString());
        }
    }
    
    
    public CameraSettingsTabViewModel(DeviceConfig deviceConfig,IEventAggregator eventAggregator, MainWindowViewModel mainWindowViewModel)
    {
        InitializeCollections();
        
        _eventAggregator = eventAggregator;
        _eventAggregator.GetEvent<MajesticContentUpdatedEvent>().Subscribe(OnMajesticContentUpdated);
    
        _mainWindowViewModel = mainWindowViewModel;
        
        RestartMajesticCommand = new RelayCommand(() => RestartMajestic(_mainWindowViewModel));
        
        _deviceConfig = deviceConfig;
        _sshClientService = new SshClientService(_eventAggregator);
    }

    private void OnMajesticContentUpdated(MajesticContentUpdatedMessage message)
    {
        var majesticContent = message.Content;
        CanConnect = true;
        ParseYamlConfig(majesticContent);

    }
    
    private void InitializeCollections()
    {
        Resolution = new ObservableCollection<string>
        {
            "1280x720", "1456x816", "1920x1080", "2104x1184", "2208x1248", "2240x1264", "2312x1304",
            "2512x1416", "2560x1440", "2560x1920", "3200x1800", "3840x2160"
        };

        FPS = new ObservableCollection<string>
        {
            "20", "30", "40", "50", "60", "70", "80", "90", "100", "110", "120"
        };

        Codec = new ObservableCollection<string> { "h264", "h265" };
        Bitrate = new ObservableCollection<string> { "1024", "2048", "3072", "4096", "5120", "6144", "7168", "8192", "9216" };
        Exposure = new ObservableCollection<string> { "5", "6", "8", "10", "11", "12", "14", "16", "33", "50" };
        Contrast = new ObservableCollection<string> { "1", "5", "10", "20", "30", "40", "50", "60", "70", "80", "90", "100" };
        Hue = new ObservableCollection<string> { "1", "5", "10", "20", "30", "40", "50", "60", "70", "80", "90", "100" };
        Saturation = new ObservableCollection<string> { "1", "5", "10", "20", "30", "40", "50", "60", "70", "80", "90", "100" };
        Luminance = new ObservableCollection<string> { "1", "5", "10", "20", "30", "40", "50", "60", "70", "80", "90", "100" };
        Flip = new ObservableCollection<string> { "true", "false" };
        Mirror = new ObservableCollection<string> { "true", "false" };
    }
    
        // YAML parsing and updating methods (unchanged)
        private void ParseYamlConfig(string content)
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

            Logger.Instance().Log($"Camera Found {fullKey}: {scalarNode.Value}");
            

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

    public async Task SaveRestartMajesticCommand()
    {
        Logger.Instance().Log("Preparing to Save Majestic file.");
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

            Logger.Instance().Log("YAML file saved and majestic service restarted successfully.");
        }
        catch (Exception ex)
        {
            Logger.Instance().Log($"Failed to save YAML file: {ex.Message}");
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
}