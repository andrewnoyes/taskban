using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using taskban_universal.ViewModels.Boards;
using TamedTasks.Data;
using TamedTasks.Models.Common;
using Template10.Mvvm;

namespace taskban_universal.ViewModels.Tasks
{
    public class TaskListVm : ViewModelBase
    {
        #region Properties

        private TaskList _taskList;
        public TaskList TaskList
        {
            get { return _taskList; }
            set { Set(ref _taskList, value); }
        }

        private ObservableCollection<TaskItemVm> _taskItemViewModels;
        public ObservableCollection<TaskItemVm> TaskItemViewModels
        {
            get
            {
                if (_taskItemViewModels != null) return _taskItemViewModels;

                _taskItemViewModels = new ObservableCollection<TaskItemVm>();
                foreach (var taskItem in DbManager.Instance.GetAllTaskItemsInList(TaskList.Id))
                    _taskItemViewModels.Add(new TaskItemVm(taskItem));

                return _taskItemViewModels;
            }
            set { Set(ref _taskItemViewModels, value); }
        }


        private bool _addTaskVisible;
        public bool AddTaskVisible
        {
            get { return _addTaskVisible; }
            set { Set(ref _addTaskVisible, value); }
        }

        private bool _editTaskListVisible;
        public bool EditTaskListVisible
        {
            get { return _editTaskListVisible; }
            set { Set(ref _editTaskListVisible, value); }
        }

        private string _newTaskTitle;
        public string NewTaskTitle
        {
            get { return _newTaskTitle; }
            set
            {
                Set(ref _newTaskTitle, value);
                SaveTaskCommand.RaiseCanExecuteChanged();
            }
        }

        #endregion

        //#region Commands

        private DelegateCommand _showTaskCommand;
        public DelegateCommand ShowTaskCommand =>
            _showTaskCommand ?? (_showTaskCommand = new DelegateCommand(() => AddTaskVisible = !AddTaskVisible));

        private DelegateCommand _saveTaskCommand;
        public DelegateCommand SaveTaskCommand =>
            _saveTaskCommand ?? (_saveTaskCommand = new DelegateCommand(SaveTask, CanSaveTask));

        //private DelegateCommand _updateTaskListCommand;
        //public DelegateCommand UpdateTaskListCommand =>
        //    _updateTaskListCommand ??
        //    (_updateTaskListCommand = new DelegateCommand(UpdateTaskList, CanUpdateTaskList));

        //private DelegateCommand _deleteTaskListCommand;
        //public DelegateCommand DeleteTaskListCommand =>
        //    _deleteTaskListCommand ??
        //    (_deleteTaskListCommand = new DelegateCommand(DeleteTaskList, CanDeleteTaskList));

        //#endregion

        public TaskListVm(TaskList taskList)
        {
            TaskList = taskList;
        }

        #region Public

        public void ClearTaskItems()
        {
            TaskItemViewModels.Clear();
            DbManager.Instance.DeleteTaskListAndReferences(TaskList, false);
        }

        public void SaveTask()
        {
            var task = new TaskItem
            {
                Title = NewTaskTitle,
                TaskListId = TaskList.Id
            };

            var dbTask = DbManager.Instance.InsertEntity(task);
            TaskItemViewModels.Add(new TaskItemVm(dbTask));
            NewTaskTitle = string.Empty;
        }

        public bool CanSaveTask()
        {
            return !string.IsNullOrEmpty(NewTaskTitle);
        }

        #endregion

    }
}
