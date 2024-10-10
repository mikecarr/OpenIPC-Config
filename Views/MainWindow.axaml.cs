using Avalonia.Controls;
using Avalonia.Interactivity;
using OpenIPC_Config.ViewModels;

using MsBox.Avalonia;
using MsBox.Avalonia.Enums;

namespace OpenIPC_Config.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        
        InitializeComponent();
        DataContext = new MainWindowViewModel(); // Set the DataContext
        
        // Set the logs TextBox and ScrollViewer for the Logger
        Logger.Instance.SetLogComponents(LogsTextBox, LogsScrollViewer);
        
        this.WindowState = WindowState.Normal; // Ensure the window is normal
        this.Activate(); // Activate the window
    }

    
    
    private void btnRestartWFB_Click(object? sender, RoutedEventArgs e)
    {
        
        string externFile = "extern.sh";
        Logger.Instance.Log("RestartWFB Button clicked");
        
        // Check if the file exists
        if (!System.IO.File.Exists(externFile))
        {
            var box = MessageBoxManager
                .GetMessageBoxStandard("Caption", "File " + externFile + " not found!", ButtonEnum.Ok);
            
            // MessageBox.Show("File " + externFile + " not found!");
            return;
        }

        // Validate the IP address
        // if (IsValidIP(txtIP.Text))
        // {
        //     // Create a new process to run the external batch file
        //     using (Process process = new Process())
        //     {
        //         process.StartInfo.UseShellExecute = false;
        //         process.StartInfo.FileName = externFile;
        //         process.StartInfo.Arguments = "rswfb " + string.Format("{0}", txtIP.Text) + " " + txtPassword.Text;
        //         process.StartInfo.RedirectStandardOutput = false;
        //         process.Start();
        //     }
        // }
        // else
        // {
        //     MessageBox.Show("Please enter a valid IP address");
        // }
    }

}