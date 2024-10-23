using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using OpenIPC_Config.ViewModels;

namespace OpenIPC_Config.Views;

public partial class SetupTabView : UserControl
{
    public SetupTabView()
    {
        InitializeComponent();
        DataContext = new SetupTabViewModel(); // Set the ViewModel as DataContext
    }
}