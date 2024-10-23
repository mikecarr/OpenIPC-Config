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

    private IEventAggregator _eventAggregator;
    
    public string WfbConfContent
    {
        get => _wfbConfContent;
        set
        {
            this.RaiseAndSetIfChanged(ref _wfbConfContent, value);
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
            Console.WriteLine($"SelectedPower updated to {value}");
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
                    case "driver_txpower_override":
                        if (int.TryParse(value, out int parsedPower))
                        {
                            // Ensure the parsed power exists in the collection, or set a fallback
                            if (Power58GHz.Contains(parsedPower))
                            {
                                SelectedPower = parsedPower;
                            }
                            
                        }
                        break;
                    
                }
                

                // Handle parsed data, e.g., store in a dictionary or bind to properties
                Console.WriteLine($"Key: {key}, Value: {value}");
            }
        }
    }
    public ICommand RestartWfbCommand { get; } = new RelayCommand(RestartWfb);
    
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
    public WfbSettingsTabViewModel(IEventAggregator eventAggregator)
    {
        InitializeCollections();
        
        _eventAggregator = eventAggregator;
        _eventAggregator.GetEvent<WfbConfContentUpdatedEvent>().Subscribe(OnWfbConfContentUpdated);
        
    }
    private void OnWfbConfContentUpdated(WfbConfContentUpdatedMessage message)
    {
        WfbConfContent = message.Content;
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
    
        // Set default selected power
        SelectedPower = Power58GHz.First();
        
        // Manually raise property change notification for Power58GHz and SelectedPower
        this.RaisePropertyChanged(nameof(Power58GHz));
        this.RaisePropertyChanged(nameof(SelectedPower));
    }

    private static void RestartWfb()
    {
        Console.WriteLine("RestartWfbCommand executed");
    }


    
}