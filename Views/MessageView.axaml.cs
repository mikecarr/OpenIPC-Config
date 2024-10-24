using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using OpenIPC_Config.ViewModels;

namespace OpenIPC_Config.Views;

public partial class MessageView : UserControl
{
    public MessageView()
    {
        InitializeComponent();
        DataContext = new MessageViewModel();

    }
}