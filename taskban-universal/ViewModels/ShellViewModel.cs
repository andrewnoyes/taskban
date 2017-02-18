using TamedTasks.Data;
using TamedTasks.Models.Base;
using Template10.Mvvm;

namespace TamedTasks.ViewModels
{
    public class ShellViewModel : ViewModelBase
    {
        private RelayCommand _sync;
        public RelayCommand Sync
            => _sync ?? (_sync = new RelayCommand(
                a => OneNoteManager.Instance.SyncAsync(),
                p => !OneNoteManager.Instance.IsSyncing));
    }
}
