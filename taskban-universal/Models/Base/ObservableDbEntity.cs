using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace TamedTasks.Models.Base
{
    public abstract class ObservableDbEntity : DbEntity, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            handler?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
