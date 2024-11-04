using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using OpenIPC.ViewModels;

namespace OpenIPC.Views;

public partial class WfbTabView : UserControl
{
    public WfbTabView()
    {
        InitializeComponent();
        
        if (!Design.IsDesignMode)
        {
            DataContext = new WfbTabViewModel();
        }
    }
}