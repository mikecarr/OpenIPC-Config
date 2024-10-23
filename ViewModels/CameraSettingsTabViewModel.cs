using System.Collections.ObjectModel;

namespace OpenIPC_Config.ViewModels;

public class CameraSettingsTabViewModel : ViewModelBase
{

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
    
    public CameraSettingsTabViewModel()
    {
        InitializeCollections();
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
}