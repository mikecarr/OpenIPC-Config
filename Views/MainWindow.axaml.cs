using System.Diagnostics;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Diagnostics;
using Avalonia.Interactivity;
using OpenIPC_Config.Services;
using OpenIPC_Config.ViewModels;

namespace OpenIPC_Config.Views;

public partial class MainWindow : Window
{
    public MainWindow(ViewModelLocator viewModelLocator)
    {
        InitializeComponent();
        
        var settings = SettingsManager.LoadSettings();
        
        var viewModel = new MainWindowViewModel(settings, viewModelLocator.EventAggregator);
        DataContext = viewModel; // Set the DataContext
        
        // Attach the developer tools
        this.AttachDevTools();
        
        this.WindowState = WindowState.Normal; // Ensure the window is normal
        this.Activate(); // Activate the window
    }

    

    private async void btnRestartWFB_Click(object? sender, RoutedEventArgs e)
    {
        // string externFile = "extern.sh";
        // Logger.Instance().Log("RestartWFB Button clicked");
        //
        // // Check if the file exists
        // if (!System.IO.File.Exists(externFile))
        // {
        //     var box = MessageBoxManager
        //         .GetMessageBoxStandard("Caption", "File " + externFile + " not found!", ButtonEnum.Ok);
        //     await box.Show();
        //     return;
        // }

        // Validate the IP address (implement this method as needed)
        // if (IsValidIP(txtIP.Text))
        // {
        //     // Create a new process to run the external script
        //     using (Process process = new Process())
        //     {
        //         process.StartInfo.UseShellExecute = false;
        //         process.StartInfo.FileName = externFile;
        //         process.StartInfo.Arguments = $"rswfb {txtIP.Text} {txtPassword.Text}";
        //         process.StartInfo.RedirectStandardOutput = true;
        //         process.Start();
        //         string output = await process.StandardOutput.ReadToEndAsync();
        //         Logger.Instance().Log(output); // Log output if needed
        //     }
        // }
        // else
        // {
        //     MessageBoxManager.GetMessageBoxStandard("Error", "Please enter a valid IP address").Show();
        // }
    }
}
