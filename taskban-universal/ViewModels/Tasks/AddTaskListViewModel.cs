using System;
using TamedTasks.Data;
using TamedTasks.Models.Common;
using Template10.Mvvm;

namespace TamedTasks.ViewModels.Tasks
{
    public class AddTaskListViewModel : ViewModelBase
    {
        #region Props/Vars

        private string _title = string.Empty;
        public string Title
        {
            get { return _title; }
            set
            {
                _title = value;
                AddTaskListCommand.RaiseCanExecuteChanged();
            }
        }

        public string Description { get; set; }

        public string BoardTitle { get; private set; }

        private readonly string _boardId;

        public EventHandler<TaskList> NewTaskListAdded;

        #endregion

        #region Commands

        private DelegateCommand _addTaskListCommand;
        public DelegateCommand AddTaskListCommand =>
            _addTaskListCommand ?? (_addTaskListCommand = new DelegateCommand(CreateTaskList, CanCreateTaskList));

        #endregion

        public AddTaskListViewModel(string boardId, string boardTitle)
        {
            _boardId = boardId;
            BoardTitle = boardTitle;
        }

        private void CreateTaskList()
        {
            var list = DbManager.Instance.InsertEntity(
                new TaskList
                {
                    BoardId = _boardId,
                    Description = Description,
                    Title = Title
                });

            NewTaskListAdded?.Invoke(this, list);
        }

        private bool CanCreateTaskList()
        {
            return !string.IsNullOrEmpty(Title);
        }
    }
}
