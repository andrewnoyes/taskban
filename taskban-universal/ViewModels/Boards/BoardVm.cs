using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media;
using taskban_universal.ViewModels.Tasks;
using TamedTasks.Data;
using TamedTasks.Models.Common;
using Template10.Mvvm;

namespace taskban_universal.ViewModels.Boards
{
    public class BoardVm : ViewModelBase
    {
        private Board _board;
        public Board Board
        {
            get { return _board; }
            set { Set(ref _board, value); }
        }

        private SolidColorBrush _background;
        public SolidColorBrush Background
        {
            get { return _background ?? (_background = TamedTasks.Models.Util.GetBoardBrush(Board)); }
            set { Set(ref _background, value); }
        }


        private ObservableCollection<TaskListVm> _taskListViewModels;
        public ObservableCollection<TaskListVm> TaskListViewModels
        {
            get
            {
                if (_taskListViewModels != null) return _taskListViewModels;

                _taskListViewModels = new ObservableCollection<TaskListVm>();
                foreach (var taskList in DbManager.Instance.GetAllTaskLists(Board.Id))
                    _taskListViewModels.Add(new TaskListVm(taskList));

                return _taskListViewModels;
            }
            set { Set(ref _taskListViewModels, value); }
        }


        public BoardVm(Board board)
        {
            Board = board;
        }




    }
}
