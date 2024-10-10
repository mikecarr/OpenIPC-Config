using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace OpenIPC_Config.ViewModels;

public partial class MainWindowViewModel : INotifyPropertyChanged
{
    // Nullable PropertyChanged event
    public event PropertyChangedEventHandler? PropertyChanged;
    

    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private string _selected58GHzFrequency;
    private string _selected24GHzFrequency;
    private int _selected58GHzPower;
    private int _selected24GHzPower;
    private int _selectedMCSIndex;
    private int _selectedSTBC;
    private int _selectedLDPC;
    private int _selectedFecK;
    private int _selectedFecN;

    
    public ObservableCollection<string> Frequencies58GHz { get; set; } = new ObservableCollection<string>();
    public ObservableCollection<string> Frequencies24GHz { get; set; } = new ObservableCollection<string>();
    public ObservableCollection<int> Power58GHz { get; set; } = new ObservableCollection<int>();
    public ObservableCollection<int> Power24GHz { get; set; } = new ObservableCollection<int>();
    public ObservableCollection<int> MCSIndex { get; set; } = new ObservableCollection<int>();
    public ObservableCollection<int> STBC { get; set; } = new ObservableCollection<int>();
    public ObservableCollection<int> LDPC { get; set; } = new ObservableCollection<int>();
    public ObservableCollection<int> FecK { get; set; } = new ObservableCollection<int>();
    public ObservableCollection<int> FecN { get; set; } = new ObservableCollection<int>();

    public string Selected58GHzFrequency
    {
        get => _selected58GHzFrequency;
        set
        {
            if (_selected58GHzFrequency != value)
            {
                _selected58GHzFrequency = value;
                OnPropertyChanged(nameof(Selected58GHzFrequency));
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
                OnPropertyChanged(nameof(_selected58GHzPower));
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
                OnPropertyChanged(nameof(_selected24GHzPower));
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
                OnPropertyChanged(nameof(_selectedMCSIndex));
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
                OnPropertyChanged(nameof(_selectedSTBC));
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
                OnPropertyChanged(nameof(_selectedLDPC));
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
                OnPropertyChanged(nameof(_selectedFecK));
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
                OnPropertyChanged(nameof(_selectedFecN));
            }
        }
    }
    public MainWindowViewModel()
    {
        // Initialize combo box values
        Frequencies58GHz = new ObservableCollection<string>
        {
            "5180 MHz [36]",
            "5200 MHz [40]",
            "5220 MHz [44]",
            "5240 MHz [48]",
            "5260 MHz [52]",
            "5280 MHz [56]",
            "5300 MHz [60]",
            "5320 MHz [64]",
            "5500 MHz [100]",
            "5520 MHz [104]",
            "5540 MHz [108]",
            "5560 MHz [112]",
            "5580 MHz [116]",
            "5600 MHz [120]",
            "5620 MHz [124]",
            "5640 MHz [128]",
            "5660 MHz [132]",
            "5680 MHz [136]",
            "5700 MHz [140]",
            "5720 MHz [144]",
            "5745 MHz [149]",
            "5765 MHz [153]",
            "5785 MHz [157]",
            "5805 MHz [161]",
            "5825 MHz [165]",
            "5845 MHz [169]",
            "5865 MHz [173]",
            "5885 MHz [177]"        };
        
        Frequencies24GHz = new ObservableCollection<string>
        {
            "2412 MHz [1]",
            "2417 MHz [2]",
            "2422 MHz [3]",
            "2427 MHz [4]",
            "2432 MHz [5]",
            "2437 MHz [6]",
            "2442 MHz [7]",
            "2447 MHz [8]",
            "2452 MHz [9]",
            "2457 MHz [10]",
            "2462 MHz [11]",
            "2467 MHz [12]",
            "2472 MHz [13]",
            "2484 MHz [14]"       };

        
        Power58GHz = new ObservableCollection<int>
        {
            1,
            5,
            10,
            15,
            20,
            25,
            30,
            35,
            40,
            50,
            55,
            60,
            63
        };
        
        // Create a collection for MCS Index values as integers
        MCSIndex = new ObservableCollection<int>
        {
            0, 1, 2, 3, 4, 5, 6, 7, 8, 9,
            10, 11, 12, 13, 14, 15, 16, 17, 18, 19,
            20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31
        };
        
        // Create a collection for the 2.4GHz TX Power values
        Power24GHz = new ObservableCollection<int>
        {
            20,
            25,
            30,
            35,
            40,
            45,
            50,
            55,
            58
        };
        
        // Create a collection for the STBC values
        STBC = new ObservableCollection<int>
        {
            0,1
        };
        
        // Create a collection for the LDPC values
        LDPC = new ObservableCollection<int>
        {
            0,1
        };
        
        // Create a collection for FecK values as integers
        FecK = new ObservableCollection<int>
        {
            0, 1, 2, 3, 4, 5, 6, 7, 8, 9,
            10, 11, 12
        };
        
        // Create a collection for FecN values as integers
        FecN = new ObservableCollection<int>
        {
            0, 1, 2, 3, 4, 5, 6, 7, 8, 9,
            10, 11, 12
        };
        
        // Set defaults
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

}