using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using DynamicData.Binding;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;
using OpenIPC_Config.Messages;
using OpenIPC_Config.Models;
using OpenIPC_Config.Services;
using Prism.Events;
using ReactiveUI;

namespace OpenIPC_Config.ViewModels;

public class TelemetryTabViewModel : ViewModelBase
{
    
    private readonly MainWindowViewModel _mainWindowViewModel;
    
    private readonly ISshClientService _sshClientService;

    private IEventAggregator _eventAggregator;
    
    private DeviceConfig _deviceConfig;
    
    public ICommand EnableUART0Command { get; private set; }
    public ICommand DisableUART0Command { get; private set; }
    public ICommand AddMavlinkCommand { get; private set; }
    public ICommand UploadMSPOSDCommand { get; private set; }
    public ICommand UploadINavCommand { get; private set; }
    public ICommand OnBoardRecCommand { get; private set; }
    
    public ICommand SaveAndRestartTelemetryCommand { get; private set; }
    
    // ObservableCollections
    public ObservableCollection<string> SerialPorts { get; set; }
    public ObservableCollection<string> BaudRates { get; set; }
    public ObservableCollection<string> McsIndex { get; set; }
    public ObservableCollection<string> Aggregate { get; set; }
    public ObservableCollection<string> RC_Channel { get; set; }
    public ObservableCollection<string> Router { get; set; }
    
    private string _selectedSerialPort;
    private string _selectedBaudRate;
    private string _selectedMcsIndex;
    private string _selectedAggregate;
    private string _selectedRcChannel;
    private string _selectedRouter;
    
    private bool _isOnboardRecOn;
    private bool _isOnboardRecOff;
    
    private bool _canConnect;
    
    private string _telemetryContent;

    public bool IsOnboardRecOn
    {
        get => _isOnboardRecOn;
        set => this.RaiseAndSetIfChanged(ref _isOnboardRecOn, value);
    }

    // Property to track whether the "OFF" radio button is checked
    public bool IsOnboardRecOff
    {
        get => _isOnboardRecOff;
        set => this.RaiseAndSetIfChanged(ref _isOnboardRecOff, value);
    }
    public bool CanConnect
    {
        get => _canConnect;
        set
        {
            this.RaiseAndSetIfChanged(ref _canConnect, value);
            Logger.Instance().Log($"CanConnect {value}");
        }
    }
    public string SelectedBaudRate
    {
        get => _selectedBaudRate;
        set
        {
            this.RaiseAndSetIfChanged(ref _selectedBaudRate, value);
            Logger.Instance().Log($"SelectedBaudRate updated to {value}");
        }    
    }
    public string SelectedMcsIndex
    {
        get => _selectedMcsIndex;
        set
        {
            this.RaiseAndSetIfChanged(ref _selectedMcsIndex, value);
            Logger.Instance().Log($"SelectedMcsIndex updated to {value}");
        }        
    }
    public string SelectedAggregate
    {
        get => _selectedAggregate;
        set
        {
            this.RaiseAndSetIfChanged(ref _selectedAggregate, value);
            Logger.Instance().Log($"SelectedAggregate updated to {value}");
        }
    }
    public string SelectedRcChannel
    {
        get => _selectedRcChannel;
        set
        {
            this.RaiseAndSetIfChanged(ref _selectedRcChannel, value);
            Logger.Instance().Log($"SelectedRcChannel updated to {value}");
        }        
    }
    public string SelectedRouter
    {
        get => _selectedRouter;
        set
        {
            this.RaiseAndSetIfChanged(ref _selectedRouter, value);
            Logger.Instance().Log($"SelectedRouter updated to {value}");
        }
    }
    public string SelectedSerialPort
    {
        get => _selectedSerialPort;
        set
        {
            this.RaiseAndSetIfChanged(ref _selectedSerialPort, value);
            Logger.Instance().Log($"SelectedSerialPort updated to {value}");
        }
    }
    
    private string TelemetryContent
    {
        get => _telemetryContent;
        set
        {
            this.RaiseAndSetIfChanged(ref _telemetryContent, value);
            CanConnect = true;
            //ParseTelemetryContent();
        }
    }
    
    
    
    public TelemetryTabViewModel(DeviceConfig deviceConfig,IEventAggregator eventAggregator, MainWindowViewModel mainWindowViewModel)
    {
        InitializeCollections();

        _deviceConfig = deviceConfig;
        
        _mainWindowViewModel = mainWindowViewModel;
        
        _eventAggregator = eventAggregator;
        _eventAggregator.GetEvent<TelemetryContentUpdatedEvent>().Subscribe(OnTelemetryContentUpdated);
        
        _deviceConfig = deviceConfig;
        _sshClientService = new SshClientService(_eventAggregator);
        
        EnableUART0Command = new RelayCommand(() => EnableUART0(_mainWindowViewModel));
        DisableUART0Command = new RelayCommand(() => DisableUART0(_mainWindowViewModel));
        OnBoardRecCommand = new RelayCommand(() => OnBoardRec(_mainWindowViewModel));
        AddMavlinkCommand = new RelayCommand(() => AddMavlink(_mainWindowViewModel));
        UploadMSPOSDCommand = new RelayCommand(() => UploadMSPOSD(_mainWindowViewModel));
        UploadINavCommand = new RelayCommand(() => UploadINav(_mainWindowViewModel));
        SaveAndRestartTelemetryCommand = new RelayCommand(() => SaveAndRestartTelemetry(_mainWindowViewModel));
        
    }
    
    private async void SaveAndRestartTelemetry(MainWindowViewModel mainWindowViewModel)
    {
        Logger.Instance().Log("*** TODO : SaveAndRestartTelemetryCommand executed");
        //await SaveSaveAndRestartTelemetryCommand();
        
        string newSerial = SelectedSerialPort;
        string newBaudRate = SelectedBaudRate;
        string newRouter = SelectedRouter;
        string newMcsIndex = SelectedMcsIndex;
        string newAggregate = SelectedAggregate;
        
        
            
        string updatedTelemetryContent = UpdateTelemetryContent(
            newSerial,
            newBaudRate,
            newRouter,
            newMcsIndex,
            newAggregate
            
        );
        
        TelemetryContent = updatedTelemetryContent;

        Logger.Instance().Log($"Uploading new : {OpenIPC.TELEMETRY_FILE_LOC}");
        _sshClientService.UploadFileAsync(_deviceConfig, OpenIPC.WFB_CONF_FILE_LOC, TelemetryContent);

        Logger.Instance().Log($"Restarting Wfb"); 
        _sshClientService.ExecuteCommandAsync(_deviceConfig, DeviceCommands.TelemetryRestartCommand);
    }
    
    private async void OnTelemetryContentUpdated(TelemetryContentUpdatedMessage message)
    {
        TelemetryContent = message.Content;
        CanConnect = true;
        ParseTelemetryContent();
    }
    
    // Method to parse the telemetryContent
    private void ParseTelemetryContent()
    {
        Logger.Instance().Log("Parsing TelemetryContent.");
        
        if (string.IsNullOrEmpty(TelemetryContent))
        {
            return;
        }

        // Logic to parse wfbConfContent, e.g., split by lines or delimiters
        var lines = TelemetryContent.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);

        foreach (var line in lines)
        {
            // Example: Parse key-value pairs
            var parts = line.Split('=');
            if (parts.Length == 2)
            {
                string key = parts[0].Trim();
                string value = parts[1].Trim();
                
                switch(key)
                {
                    case Telemetry.Serial:
                        SelectedSerialPort = value;
                        break;
                    case Telemetry.Baud:
                        SelectedBaudRate = value;
                        break;
                    case Telemetry.Router:
                        SelectedRouter = value;
                        break;
                    case Telemetry.McsIndex:
                        SelectedMcsIndex = value;
                        break;
                    case Telemetry.Aggregate:
                        SelectedAggregate = value;
                        break;
                    // case Telemetry.RcChannel:
                    //     SelectedRcChannel = value;
                    //     break;
                    default:
                        // Handle other key-value pairs
                        Logger.Instance().Log($"Telemetry - Unknown key: {key}, value: {value}");
                        break;

                }
                

                // Handle parsed data, e.g., store in a dictionary or bind to properties
                Logger.Instance().Log($"Telemetry - Key: {key}, Value: {value}");
            }
        }
    }

    private async void DisableUART0(MainWindowViewModel mainWindowViewModel)
    {
        Logger.Instance().Log("DisableUART0Command executed");
        
        _sshClientService.ExecuteCommandAsync(_deviceConfig, DeviceCommands.UART0OffCommand); //await SaveDisableUART0Command();
    }
    private async void EnableUART0(MainWindowViewModel mainWindowViewModel)
    {
        Logger.Instance().Log("EnableUART0Command executed");
        _sshClientService.ExecuteCommandAsync(_deviceConfig, DeviceCommands.UART0OnCommand);

    }
    private async void OnBoardRec(MainWindowViewModel mainWindowViewModel)  
    {
        if (IsOnboardRecOn == true)
        {
            _sshClientService.ExecuteCommandAsync(_deviceConfig, "yaml-cli .records.enabled true");
        }
        else if (IsOnboardRecOff == true)
        {
            _sshClientService.ExecuteCommandAsync(_deviceConfig, "yaml-cli .records.enabled false");
        }
        
    }
    
    private async void AddMavlink(MainWindowViewModel mainWindowViewModel)
    {
        Logger.Instance().Log("AddMavlinkCommand executed");
        _sshClientService.ExecuteCommandAsync(_deviceConfig, TelemetryCommands.Extra);
        _sshClientService.ExecuteCommandAsync(_deviceConfig, DeviceCommands.RebootCommand);
        
    }
    
    private async void UploadMSPOSD(MainWindowViewModel mainWindowViewModel)
    {
        Logger.Instance().Log("UploadMSPOSDCommand executed");
        
        string msposdFile = "msposd";
        
        // Get all files in the binaries folder
        string binariesFolderPath = Path.Combine(Environment.CurrentDirectory, OpenIPC.LOCAL_BINARIES_FOLDER);
        
        var files = Directory.GetFiles(binariesFolderPath).Where(f => f.Contains(msposdFile));

        if (files == null || !files.Any())
        {
            var box = MessageBoxManager
                .GetMessageBoxStandard("File not found!", "File " + msposdFile + " not found!", ButtonEnum.Ok);
            await box.ShowAsync();
            return; 
        }
        else
        
        {
            // killall -q msposd
            await _sshClientService.ExecuteCommandAsync(_deviceConfig, "killall -q msposd");
            // upload msposd
            await _sshClientService.UploadBinaryAsync(_deviceConfig, OpenIPC.REMOTE_BINARIES_FOLDER, "msposd");
            // chmod +x /usr/bin/msposd
            await _sshClientService.ExecuteCommandAsync(_deviceConfig, "chmod +x /usr/bin/msposd");
            
            // upload betaflight fonts
            await _sshClientService.ExecuteCommandAsync(_deviceConfig, $"mkdir {OpenIPC.REMOTE_FONTS_FOLDER}");
            await _sshClientService.UploadBinaryAsync(_deviceConfig, OpenIPC.REMOTE_FONTS_FOLDER, OpenIPC.FileType.BetaFlightFonts,"font.png");
            await _sshClientService.UploadBinaryAsync(_deviceConfig, OpenIPC.REMOTE_FONTS_FOLDER, OpenIPC.FileType.BetaFlightFonts,"font_hd.png");
            
            // upload vtxmenu.ini /etc
            await _sshClientService.UploadBinaryAsync(_deviceConfig, OpenIPC.REMOTE_ETC_FOLDER, "vtxmenu.ini");

            // ensure file is unix formatted
            await _sshClientService.ExecuteCommandAsync(_deviceConfig, "dos2unix /etc/vtxmenu.ini");
            
            // reboot
            await _sshClientService.ExecuteCommandAsync(_deviceConfig, DeviceCommands.RebootCommand);
            
            Thread.Sleep(3000);
            

        }
        
        
    }
    
    private async void UploadINav(MainWindowViewModel mainWindowViewModel)
    {
        Logger.Instance().Log("UploadINavCommand executed");
        // upload betaflight fonts
        await _sshClientService.ExecuteCommandAsync(_deviceConfig, $"mkdir {OpenIPC.REMOTE_FONTS_FOLDER}");
        await _sshClientService.UploadBinaryAsync(_deviceConfig, OpenIPC.REMOTE_FONTS_FOLDER, OpenIPC.FileType.iNavFonts,"font.png");
        await _sshClientService.UploadBinaryAsync(_deviceConfig, OpenIPC.REMOTE_FONTS_FOLDER, OpenIPC.FileType.iNavFonts,"font_hd.png");
    }
    

    private void InitializeCollections()
    {
        SerialPorts = new ObservableCollectionExtended<string> { "/dev/ttyS0", "/dev/ttyS1", "/dev/ttyS2" };
        BaudRates = new ObservableCollectionExtended<string> { "4800", "9600", "19200", "38400", "57600", "115200"  };
        McsIndex = new ObservableCollectionExtended<string> { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9" };
        Aggregate = new ObservableCollectionExtended<string> { "0", "1", "2", "4", "6", "8", "10", "12", "14", "15" };
        RC_Channel = new ObservableCollectionExtended<string> { "0", "1", "2", "3", "4", "5", "6", "7", "8" };   
        Router = new ObservableCollectionExtended<string> { "0", "1", "2" };
        
        
    }
    
    private string UpdateTelemetryContent(
        string newSerial,
        string newBaudRate,
        string newRouter,
        string newMcsIndex,
        string newAggregate
    )
    {
        // Logic to update WfbConfContent with the new values
        var lines = TelemetryContent.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
        var regex = new Regex(@"(frequency|channel|driver_txpower_override|frequency24|txpower|mcsindex|stbc|ldpc|feck|fecN)=.*");
        var updatedContent = regex.Replace(TelemetryContent, match =>
        {
            switch (match.Groups[1].Value)
            {
                case Telemetry.Serial:
                    return $"serial={newSerial}";
                case Telemetry.Baud:
                    return $"baud={newBaudRate}";
                case Telemetry.Router:
                    return $"router={newRouter}";
                case Telemetry.McsIndex:
                    return $"mcsindex={newMcsIndex}";
                case Telemetry.Aggregate:
                    return $"aggregate={newAggregate}";
                
                default:
                    return match.Value;
            }
        });
        return updatedContent;
        
    }
    

    
}