using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using TamedTasks.Data;
using TamedTasks.Models.Common;
using Template10.Mvvm;

namespace TamedTasks.ViewModels.Tasks
{
    public class TaskItemViewModel : ViewModelBase
    {
        private TaskItem _taskItem;
        public TaskItem TaskItem
        {
            get { return _taskItem; }
            set
            {
                _taskItem = value;
                RaisePropertyChanged();
            }
        }

        private ChecklistViewModel _checklistViewModel;
        public ChecklistViewModel ChecklistViewModel
        {
            get
            {
                return _checklistViewModel ?? (_checklistViewModel = new ChecklistViewModel(TaskItem.Id));
            }
            set
            {
                _checklistViewModel = value;
                RaisePropertyChanged();
            }
        }

        private bool _deleteTaskItem;
        public bool DeleteTaskItem
        {
            get { return _deleteTaskItem; }
            set { Set(ref _deleteTaskItem, value); }
        }

        private bool _editDescription;
        public bool EditDescription
        {
            get { return _editDescription; }
            set { Set(ref _editDescription, value); }
        }

        private DelegateCommand _editCommand;

        public DelegateCommand EditCommand =>
            _editCommand ?? (_editCommand = new DelegateCommand(() => EditDescription = !EditDescription));

        //private DelegateCommand _saveTaskItemCommand;
        //public DelegateCommand SaveTaskItemCommand =>
        //    _saveTaskItemCommand ??
        //    (_saveTaskItemCommand = new DelegateCommand(SaveTaskItem));

        private DelegateCommand _cancelSaveTaskCommand;
        public DelegateCommand CancelSaveTaskCommand =>
            _cancelSaveTaskCommand ??
            (_cancelSaveTaskCommand = new DelegateCommand(ResetChildVms));


        public event EventHandler<TaskItem> TaskItemUpdated;
        public event EventHandler<TaskItemViewModel> TaskItemDeleted;
        public event EventHandler TaskItemCancelled;

        public TaskItemViewModel(TaskItem taskItem)
        {
            TaskItem = taskItem;
        }

        /// <summary>
        /// Update the parent task list ID this task item belongs to.
        /// </summary>
        /// <param name="taskListId">The parent task list ID.</param>
        public void UpdateTaskListId(string taskListId)
        {
            TaskItem.TaskListId = taskListId;
            DbManager.Instance.UpdateEntity(TaskItem);
        }

        public async void SaveTaskItem()
        {
            await Task.Run(() =>
            {
                if (DeleteTaskItem)
                {
                    DbManager.Instance.DeleteTaskItemAndReferences(TaskItem);
                }
                else
                {
                    DbManager.Instance.UpdateEntity(TaskItem);
                    ChecklistViewModel.Save();
                }
            });

            if (DeleteTaskItem)
            {
                TaskItemDeleted?.Invoke(this, this);
            }
            else
            {
                //ResetChildVms();
                ChecklistViewModel = new ChecklistViewModel(TaskItem.Id);
                TaskItemUpdated?.Invoke(this, TaskItem);
            }
        }

        private void ResetChildVms()
        {
            ChecklistViewModel = null;
            TaskItemCancelled?.Invoke(this, EventArgs.Empty);
        }
    }
}
