using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TamedTasks.Data;
using TamedTasks.Models.Common;
using Template10.Mvvm;

namespace TamedTasks.ViewModels.Pages
{
    public class CalendarPageViewModel : ViewModelBase
    {
        private ObservableCollection<TaskItem> _taskItems; 
        public ObservableCollection<TaskItem> TaskItems
        {
            get { return _taskItems;}
            set { Set(ref _taskItems, value); }
        } 

        public CalendarPageViewModel()
        {
            TaskItems = new ObservableCollection<TaskItem>(DbManager.Instance.GetTasksWithDueDates());
        }


    }
}
