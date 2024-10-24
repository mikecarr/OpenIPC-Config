using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using OpenIPC_Config.ViewModels;

namespace OpenIPC_Config.Views;

public partial class TelemetryTabView : UserControl
{
    public TelemetryTabView()
    {
        InitializeComponent();
        DataContext = ViewModelLocator.Instance.TelemetryTabViewModel;
    }
}