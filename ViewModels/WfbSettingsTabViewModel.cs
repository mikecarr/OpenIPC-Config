using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using OpenIPC_Config.Messages;
using OpenIPC_Config.Models;
using OpenIPC_Config.Services;
using Prism.Events;
using ReactiveUI;

namespace OpenIPC_Config.ViewModels;

public class WfbSettingsTabViewModel : ReactiveObject
{
    private string _wfbConfContent;
    private readonly MainWindowViewModel _mainWindowViewModel;
    
    

    private IEventAggregator _eventAggregator;
    
    //public bool CanConnect { get; set; }

    // private ObservableCollection<bool> _canConnect;
    // public ObservableCollection<bool> CanConnect
    // {
    //     get => _canConnect;
    //     set
    //     {
    //         this.RaiseAndSetIfChanged(ref _canConnect, value);
    //     }
    // }
    private bool _canConnect;

    public bool CanConnect
    {
        get => _canConnect;
        set { this.RaiseAndSetIfChanged(ref _canConnect, value); 
        Logger.Instance.Log($"CanConnect {value}");
    }
}

    private string WfbConfContent
    {
        get => _wfbConfContent;
        set
        {
            this.RaiseAndSetIfChanged(ref _wfbConfContent, value);
            CanConnect = true;
            ParseWfbConfContent();
        }
    }
    
    private ObservableCollection<int> _power58GHz;
    public ObservableCollection<int> Power58GHz
    {
        get => _power58GHz;
        set
        {
            this.RaiseAndSetIfChanged(ref _power58GHz, value);  
            Console.WriteLine($"Power58 updated to {value}");
        } 
    }

    private int _selectedPower;
    public int SelectedPower
    {
        get => _selectedPower;
        set
        {
            this.RaiseAndSetIfChanged(ref _selectedPower, value);
            Logger.Instance.Log($"SelectedPower (5.8) updated to {value}");
        }
    }
    
    
    private int _selectedPower24GHz;
    public int SelectedPower24GHz
    {
        get => _selectedPower24GHz;
        set
        {
            this.RaiseAndSetIfChanged(ref _selectedPower24GHz, value);
            Logger.Instance.Log($"SelectedPower (2.4) updated to {value}");
        }
    }
    
    private int _selectedLdpc;
    public int SelectedLdpc
    {
        get => _selectedLdpc;
        set
        {
            this.RaiseAndSetIfChanged(ref _selectedLdpc, value);
            Logger.Instance.Log($"SelectedLdpc updated to {value}");
        }
    }
    
    private int _selectedStbc;
    public int SelectedStbc
    {
        get => _selectedStbc;
        set
        {
            this.RaiseAndSetIfChanged(ref _selectedStbc, value);
            Logger.Instance.Log($"SelectedStbc updated to {value}");
        }
    }
    
    private int _selectedMcsIndex;
    public int SelectedMcsIndex
    {
        get => _selectedMcsIndex;
        set
        {
            this.RaiseAndSetIfChanged(ref _selectedMcsIndex, value);
            Logger.Instance.Log($"SelectedMcsIndex updated to {value}");
        }
    }
    
    private int _selectedFecK;
    public int SelectedFecK
    {
        get => _selectedFecK;
        set
        {
            this.RaiseAndSetIfChanged(ref _selectedFecK, value);
            Logger.Instance.Log($"SelectedFecK updated to {value}");
        }
    }
    
    private int _selectedFecN;
    public int SelectedFecN
    {
        get => _selectedFecN;
        set
        {
            this.RaiseAndSetIfChanged(ref _selectedFecN, value);
            Logger.Instance.Log($"SelectedFecN updated to {value}");
        }
    }
    
    private string _selectedFrequency58String;
    public string SelectedFrequency58String
    {
        get => _selectedFrequency58String;
        set => this.RaiseAndSetIfChanged(ref _selectedFrequency58String, value);
    }
    
    private string _selectedFrequency24String;
    public string SelectedFrequency24String
    {
        get => _selectedFrequency58String;
        set => this.RaiseAndSetIfChanged(ref _selectedFrequency24String, value);
    }
    private int _selectedFrequency58;
    public int SelectedFrequency58
    {
        get => _selectedFrequency58;
        set
        {
            this.RaiseAndSetIfChanged(ref _selectedFrequency58, value);
    
            // Parse the selected frequency value
            if (_58frequencyMapping.TryGetValue(value, out string frequency58String))
            {
                SelectedFrequency58String = frequency58String;
            }
            else
            {
                // Handle unknown frequency value
            }
        }
    }

    // Method to parse the wfbConfContent
    private void ParseWfbConfContent()
    {
        Logger.Instance.Log("Parsing wfbConfContent.");
        
        if (string.IsNullOrEmpty(WfbConfContent))
        {
            return;
        }

        // Logic to parse wfbConfContent, e.g., split by lines or delimiters
        var lines = WfbConfContent.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);

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
                    case Wfb.Frequency:
                        if (int.TryParse(value, out int frequency))
                        {
                            string frequencyString;
                            if (_58frequencyMapping.TryGetValue(frequency, out frequencyString))
                            {
                                SelectedFrequency58String = frequencyString;
                            }
                            else if (_24frequencyMapping.TryGetValue(frequency, out frequencyString))
                            {
                                SelectedFrequency24String = frequencyString;
                            }
                            else
                            {
                                // Handle unknown frequency value
                            }
                        }
                        break;
                    case Wfb.DriverTxpowerOverride:
                        if (int.TryParse(value, out int parsedPower))
                        {
                            // Ensure the parsed power exists in the collection, or set a fallback
                            if (Power58GHz.Contains(parsedPower))
                            {
                                SelectedPower = parsedPower;
                            }
                            
                        }
                        break;
                    case Wfb.Ldpc:
                        if (int.TryParse(value, out int parsedLdpc))
                        {
                            // Ensure the parsed power exists in the collection, or set a fallback
                            if (LDPC.Contains(parsedLdpc))
                            {
                                SelectedLdpc = parsedLdpc;
                            }
                            
                        }
                        break;
                    case Wfb.Stbc:
                        if (int.TryParse(value, out int parsedStbc))
                        {
                            // Ensure the parsed power exists in the collection, or set a fallback
                            if (STBC.Contains(parsedStbc))
                            {
                                SelectedStbc = parsedStbc;
                            }
                            
                        }
                        break;
                    case Wfb.Txpower:
                        if (int.TryParse(value, out int parsedTxpower))
                        {
                            // Ensure the parsed power exists in the collection, or set a fallback
                            if (Power24GHz.Contains(parsedTxpower))
                            {
                                SelectedPower24GHz = parsedTxpower;
                            }
                            else
                            {
                                Power24GHz.Add(parsedTxpower);
                                SelectedPower24GHz = parsedTxpower;
                            }
                            
                        }
                        break;
                    case Wfb.McsIndex:
                        if (int.TryParse(value, out int parsedMcsIndex))
                        {
                            // Ensure the parsed power exists in the collection, or set a fallback
                            if (MCSIndex.Contains(parsedMcsIndex))
                            {
                                SelectedMcsIndex = parsedMcsIndex;
                            }
                            
                        }
                        break;
                    case Wfb.FecK:
                        if (int.TryParse(value, out int parsedFecK))
                        {
                            // Ensure the parsed power exists in the collection, or set a fallback
                            if (FecK.Contains(parsedFecK))
                            {
                                SelectedFecK = parsedFecK;
                            }
                            
                        }
                        break;
                    case Wfb.FecN:
                        if (int.TryParse(value, out int parsedFecN))
                        {
                            // Ensure the parsed power exists in the collection, or set a fallback
                            if (FecN.Contains(parsedFecN))
                            {
                                SelectedFecN = parsedFecN;
                            }
                            
                        }
                        break;
                    case Wfb.Channel:
                        if (int.TryParse(value, out int channel))
                        {
                            if (_58frequencyMapping.TryGetValue(channel, out string frequency58String))
                            {
                                SelectedFrequency58String = frequency58String;
                            }
                            else if (_24frequencyMapping.TryGetValue(channel, out string frequency24String))
                            {
                                SelectedFrequency24String = frequency24String;
                            }
                            else
                            {
                                // Handle unknown channel value
                            }
                        }
                        break;

                }
                

                // Handle parsed data, e.g., store in a dictionary or bind to properties
                Logger.Instance.Log($"Key: {key}, Value: {value}");
            }
        }
    }
    //public ICommand RestartWfbCommand { get; } = new RelayCommand(RestartWfb);
    public ICommand RestartWfbCommand { get; private set; }
    
    // ObservableCollections
    public ObservableCollection<string> Frequencies58GHz { get; set; }
    public ObservableCollection<string> Frequencies24GHz { get; set; }
    //public ObservableCollection<int> Power58GHz { get; set; }
    public ObservableCollection<int> Power24GHz { get; set; }
    public ObservableCollection<int> MCSIndex { get; set; }
    public ObservableCollection<int> STBC { get; set; } 
    public ObservableCollection<int> LDPC { get; set; } 
    public ObservableCollection<int> FecK { get; set; } 
    public ObservableCollection<int> FecN { get; set; } 
    
    
    
    // Dictionaries
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
        { 177, "5885 MHz [177]" }
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
    
    public void Handle(WfbConfContentUpdatedMessage message)
    {
        WfbConfContent = message.Content;
    }
    public WfbSettingsTabViewModel(IEventAggregator eventAggregator, MainWindowViewModel mainWindowViewModel)
    {
        InitializeCollections();
    
        _eventAggregator = eventAggregator;
        _eventAggregator.GetEvent<WfbConfContentUpdatedEvent>().Subscribe(OnWfbConfContentUpdated);
    
        _mainWindowViewModel = mainWindowViewModel;
        
        RestartWfbCommand = new RelayCommand(() => RestartWfb(_mainWindowViewModel));
    }
    private void OnWfbConfContentUpdated(WfbConfContentUpdatedMessage message)
    {
        WfbConfContent = message.Content;
        
        //CanConnect = true;
        
        ParseWfbConfContent();
    }

    
    private void InitializeCollections()
    {
        // Convert the dictionary values to an ObservableCollection for binding
        Frequencies58GHz = new ObservableCollection<string>(_58frequencyMapping.Values);
        Frequencies24GHz = new ObservableCollection<string>(_24frequencyMapping.Values);
        
        Power58GHz = new ObservableCollection<int> { 1, 5, 10, 15, 20, 25 };
        Power24GHz = new ObservableCollection<int> { 20, 25, 30, 35, 40 };
        MCSIndex = new ObservableCollection<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31};
        STBC = new ObservableCollection<int> { 0, 1 };
        LDPC = new ObservableCollection<int> { 0, 1 };
        FecK = new ObservableCollection<int> { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12 };
        FecN = new ObservableCollection<int> { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12 };

        _canConnect = false;


    }

    private static void RestartWfb(MainWindowViewModel mainWindowViewModel)
    {
        Logger.Instance.Log("*** TODO : RestartWfbCommand executed");
        // Access the CanConnect property from the MainWindowViewModel instance
        //_canConnect = mainWindowViewModel.CanConnect;
    }


    
}