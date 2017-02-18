using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml.Media;
using Microsoft.CSharp.RuntimeBinder;
using TamedTasks.Data;
using TamedTasks.Models.Common;
using Template10.Mvvm;
using static System.String;

namespace TamedTasks.ViewModels.Tasks
{
    public class TaskListViewModel : ViewModelBase
    {
        #region Properties

        private ObservableCollection<TaskItemViewModel> _taskItemViewModels;
        public ObservableCollection<TaskItemViewModel> TaskItemViewModels
        {
            get { return _taskItemViewModels; }
            set
            {
                _taskItemViewModels = value;
                foreach (var vm in _taskItemViewModels)
                {
                    vm.TaskItemUpdated += OnTaskItemUpdated;
                    //vm.TaskItemTest += OnTaskItemTest;
                    vm.TaskItemDeleted += OnTaskItemDeleted;
                    vm.TaskItemCancelled += OnTaskItemCancelled;
                }
                RaisePropertyChanged();
            }
        }

        private SolidColorBrush _background;
        public SolidColorBrush Background
        {
            get { return _background ?? (_background = GetTaskListBrush()); }
            set
            {
                Set(ref _background, value);
                FontColor = GetFontColorBrush();
            }
        }

        private SolidColorBrush _fontColor;
        public SolidColorBrush FontColor
        {
            get { return _fontColor; }
            set { Set(ref _fontColor, value); }
        }

        private bool _addTaskVisible;
        public bool AddTaskVisible
        {
            get { return _addTaskVisible; }
            set
            {
                _addTaskVisible = value;
                RaisePropertyChanged();
            }
        }

        private bool _editTaskListVisible;
        public bool EditTaskListVisible
        {
            get { return _editTaskListVisible; }
            set
            {
                _editTaskListVisible = value;
                RaisePropertyChanged();
            }
        }

        private string _newTaskTitle;
        public string NewTaskTitle
        {
            get { return _newTaskTitle; }
            set
            {
                _newTaskTitle = value;
                RaisePropertyChanged();
                SaveTaskCommand.RaiseCanExecuteChanged();
            }
        }

        private string _title;
        public string Title
        {
            get { return _title; }
            set
            {
                _title = value;
                RaisePropertyChanged();
            }
        }

        private string _updatedTaskListTitle;
        public string UpdatedTaskListTitle
        {
            get
            {
                if (IsNullOrEmpty(_updatedTaskListTitle))
                {
                    _updatedTaskListTitle = Title;
                }
                return _updatedTaskListTitle;
            }
            set
            {
                _updatedTaskListTitle = value;
                RaisePropertyChanged();
            }
        }

        public string Description { get; set; }

        public TaskList TaskList { get; }

        #endregion

        #region Commands

        private DelegateCommand _showTaskCommand;
        public DelegateCommand ShowTaskCommand =>
            _showTaskCommand ?? (_showTaskCommand = new DelegateCommand(() => AddTaskVisible = !AddTaskVisible));

        private DelegateCommand _saveTaskCommand;
        public DelegateCommand SaveTaskCommand =>
            _saveTaskCommand ?? (_saveTaskCommand = new DelegateCommand(SaveTask, CanSaveTask));

        private DelegateCommand _updateTaskListCommand;
        public DelegateCommand UpdateTaskListCommand =>
            _updateTaskListCommand ??
            (_updateTaskListCommand = new DelegateCommand(UpdateTaskList, CanUpdateTaskList));

        private DelegateCommand _deleteTaskListCommand;
        public DelegateCommand DeleteTaskListCommand =>
            _deleteTaskListCommand ??
            (_deleteTaskListCommand = new DelegateCommand(DeleteTaskList, CanDeleteTaskList));

        #endregion

        public EventHandler<TaskListViewModel> TaskListDeleted;

        #region Constructor

        public TaskListViewModel(TaskList taskList)
        {
            TaskList = taskList;
            Title = TaskList.Title;
            Description = TaskList.Description;

            UpdateTaskItems();
        }

        public void ChangeBackgroundColor(SolidColorBrush brush)
        {
            Background = brush;
            var color = brush.Color;
            var c = new byte[4];
            c[0] = color.A;
            c[1] = color.R;
            c[2] = color.G;
            c[3] = color.B;

            TaskList.Color = c;
            DbManager.Instance.UpdateEntity(TaskList);
        }

        #endregion

        #region Public

        public Task ClearAllTaskItemsAsync()
        {
            TaskItemViewModels.Clear();
            return Task.Run(() => DbManager.Instance.DeleteTaskListAndReferences(TaskList, false)); // Keeping the list not its children
        }


        #endregion

        #region Private

        private void UpdateTaskItems()
        {
            TaskItemViewModels = new ObservableCollection<TaskItemViewModel>();
            foreach (var task in DbManager.Instance.GetAllTaskItemsInList(TaskList.Id))
            {
                var vm = GetTaskItemVm(task);
                TaskItemViewModels.Add(vm);
            }
        }

        public void SaveTask()
        {
            var task = new TaskItem
            {
                Title = NewTaskTitle,
                TaskListId = TaskList.Id
            };

            var dbTask = DbManager.Instance.InsertEntity(task);
            var vm = GetTaskItemVm(dbTask);

            TaskItemViewModels.Add(vm);
            NewTaskTitle = Empty;
        }

        public bool CanSaveTask()
        {
            return !IsNullOrEmpty(NewTaskTitle);
        }

        private void UpdateTaskList()
        {
            if (IsNullOrEmpty(UpdatedTaskListTitle) || Equals(Title, UpdatedTaskListTitle)) return;

            Title = UpdatedTaskListTitle;
            TaskList.Title = Title;
            TaskList.Description = Description;
            DbManager.Instance.UpdateEntity(TaskList);

            EditTaskListVisible = false;
        }

        private bool CanUpdateTaskList()
        {
            return !IsNullOrEmpty(UpdatedTaskListTitle);
        }

        private void DeleteTaskList()
        {
            DbManager.Instance.DeleteTaskListAndReferences(TaskList);
            TaskListDeleted?.Invoke(this, this);
        }

        private bool CanDeleteTaskList()
        {
            return TaskList != null;
        }

        private void ResetTaskItemVm(TaskItemViewModel vmToReset)
        {
            var index = TaskItemViewModels.IndexOf(vmToReset);
            if (index < 0) return; // just to be safe

            var task = DbManager.Instance.GetEntityById<TaskItem>(vmToReset.TaskItem.Id);
            var newVm = GetTaskItemVm(task);

            TaskItemViewModels[index] = newVm;
        }

        /// <summary>
        /// Helper method for creating task item view models.
        /// </summary>
        /// <param name="taskItem">The task item to associate with the VM.</param>
        private TaskItemViewModel GetTaskItemVm(TaskItem taskItem)
        {
            var vm = new TaskItemViewModel(taskItem);
            vm.TaskItemUpdated += OnTaskItemUpdated;
            vm.TaskItemDeleted += OnTaskItemDeleted;
            vm.TaskItemCancelled += OnTaskItemCancelled;

            return vm;
        }

        private SolidColorBrush GetTaskListBrush()
        {
            return new SolidColorBrush(Color.FromArgb(255, 224, 224, 224));

            // TODO: removing this for now
            /*
            SolidColorBrush brush;
            var color = TaskList.Color;
            if (color == null || color.Length != 4) // color is stored as argb byte array
            {
                brush = new SolidColorBrush(Color.FromArgb(255, 238, 238, 238));
            }
            else
            {
                brush = new SolidColorBrush(Color.FromArgb(color[0], color[1], color[2], color[3]));
            }

            return brush; */
        }

        // Event handlers

        private void OnTaskItemUpdated(dynamic sender, TaskItem taskItem)
        {
            try
            {
                var index = TaskItemViewModels.IndexOf(sender);
                if (index >= 0)
                {
                    TaskItemViewModels[index] = GetTaskItemVm(taskItem);
                }
            }
            catch (RuntimeBinderException ex)
            {
                Debug.WriteLine($"on task item updated: {ex}");
            }
        }

        private void OnTaskItemTest(object sender, TaskItemViewModel vm)
        {
            var index = TaskItemViewModels.IndexOf(vm);
            if (index >= 0)
            {
                TaskItemViewModels[index] = vm;
            }
        }

        private void OnTaskItemDeleted(object sender, TaskItemViewModel taskItemVm)
        {
            TaskItemViewModels.Remove(taskItemVm);
        }

        private void OnTaskItemCancelled(object sender, EventArgs eventArgs)
        {
            var vm = sender as TaskItemViewModel;
            if (vm == null) return; // just to be safe

            ResetTaskItemVm(vm);
        }

        private SolidColorBrush GetFontColorBrush()
        {
            var avg = (Background.Color.R + Background.Color.G + Background.Color.B) / 3;
            return avg <= 100
                ? new SolidColorBrush(Color.FromArgb(255, 255, 255, 255))
                : new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));
        }

        #endregion
    }
}
