using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace TamedTasks.Models.Base
{
    /// <summary>
    /// Abstract class that implements INotifyPropertyChanged.
    /// </summary>
    public abstract class ObservableObject : INotifyPropertyChanged
    {

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            handler?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
