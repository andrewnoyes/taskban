using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TamedTasks.Data;
using TamedTasks.Models.Common;
using TamedTasks.ViewModels.Tasks;
using Template10.Mvvm;

namespace taskban_universal.ViewModels.Tasks
{
    public class TaskItemVm : ViewModelBase
    {
        #region Properties

        private TaskItem _taskItem;
        public TaskItem TaskItem
        {
            get { return _taskItem; }
            set { Set(ref _taskItem, value); }
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

        #endregion

        #region Commands

        private DelegateCommand _editCommand;
        public DelegateCommand EditCommand =>
            _editCommand ?? (_editCommand = new DelegateCommand(() => EditDescription = !EditDescription));

        #endregion

        #region Constructor

        public TaskItemVm(TaskItem taskItem)
        {
            TaskItem = taskItem;
        }

        #endregion

        #region Public

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

            //if (DeleteTaskItem)
            //{
            //    TaskItemDeleted?.Invoke(this, this);
            //}
            //else
            //{
            //    //ResetChildVms();
            //    ChecklistViewModel = new ChecklistViewModel(TaskItem.Id);
            //    TaskItemUpdated?.Invoke(this, TaskItem);
            //}
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

        #endregion
    }
}
