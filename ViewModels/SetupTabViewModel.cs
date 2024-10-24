using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using ReactiveUI;

namespace OpenIPC_Config.ViewModels
{
    public class SetupTabViewModel : ReactiveObject
    {
        
        public ICommand ScriptFilesBackupCommand { get; } = new RelayCommand(ScriptFilesBackup);
        public ICommand ScriptFilesRestoreCommand { get; } = new RelayCommand(ScriptFilesRestore);

        public SetupTabViewModel()
        {
        }

        

        private static void ScriptFilesBackup()
        {
            Logger.Instance().Log("Backup script executed");
        }

        private static void ScriptFilesRestore()
        {
            Logger.Instance().Log("Restore script executed");
        }
    }
}