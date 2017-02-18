using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using TamedTasks.Data;
using TamedTasks.Models.Common;
using TamedTasks.ViewModels.Boards;
using TamedTasks.ViewModels.Projects;
using TamedTasks.ViewModels.Tasks;
using Template10.Mvvm;

namespace TamedTasks.ViewModels.Pages
{
    public class TaskBoardsPageViewModel : ViewModelBase
    {
        #region Properties

        private ObservableCollection<TaskListViewModel> _taskListsViewModels;
        public ObservableCollection<TaskListViewModel> TaskListsViewModels
        {
            get { return _taskListsViewModels; }
            set { Set(ref _taskListsViewModels, value); }
        }

        private ProjectViewModel _projectViewModel;
        public ProjectViewModel ProjectViewModel
        {
            get
            {
                if (_projectViewModel != null) return _projectViewModel;

                _projectViewModel = new ProjectViewModel();
                _projectViewModel.PropertyChanged += OnProjectChanged;

                if (_projectViewModel.Projects.Any())
                {
                    // todo: temp to avoid clicking
                    _projectViewModel.Project = _projectViewModel.Projects[0];
                }
                return _projectViewModel;
            }
        }

        private BoardsListViewModel _boardsListViewModel;
        public BoardsListViewModel BoardsListViewModel
        {
            get { return _boardsListViewModel ?? (_boardsListViewModel = GetBoardsListViewModel()); }
            set { Set(ref _boardsListViewModel, value); }
        }

        public AddTaskListViewModel AddTaskListViewModel
        {
            get
            {
                var vm = new AddTaskListViewModel(BoardsListViewModel.Board.Id, BoardsListViewModel.Board.Title);
                vm.NewTaskListAdded += OnNewTaskListAdded;
                return vm;
            }
        }

        public event EventHandler TaskListsChanged;

        #endregion

        #region Private 

        private BoardsListViewModel GetBoardsListViewModel()
        {
            var vm = new BoardsListViewModel(ProjectViewModel.Project);
            //vm.PropertyChanged += OnBoardsListChanged;
            vm.BoardChanged += OnBoardChanged;
            return vm;
        }

        private async void UpdateTaskLists()
        {
            TaskListsViewModels = new ObservableCollection<TaskListViewModel>();

            var taskLists = new List<TaskList>();
            //await Task.Run(() =>
            //{
                taskLists = DbManager.Instance.GetAllTaskLists(BoardsListViewModel.Board?.Id);
            //});

            foreach (var taskList in taskLists) 
            {
                TaskListsViewModels.Add(GetTaskListVm(taskList));
            }

            TaskListsChanged?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Helper method to register task list deleted event handler.
        /// </summary>
        private TaskListViewModel GetTaskListVm(TaskList taskList)
        {
            var vm = new TaskListViewModel(taskList);
            vm.TaskListDeleted += OnTaskListDeleted;
            return vm;
        }

        // Event handlers

        private void OnProjectChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            BoardsListViewModel.UpdateBoards(ProjectViewModel.Project);
        }

        private void OnBoardsListChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            //UpdateTaskLists();
        }

        private void OnBoardChanged(object sender, Board board)
        {
            UpdateTaskLists();
        }

        private void OnNewTaskListAdded(object sender, TaskList taskList)
        {
            TaskListsViewModels.Add(GetTaskListVm(taskList));
            TaskListsChanged?.Invoke(this, EventArgs.Empty); // have to invoke this event here for the view to update
        }

        private void OnTaskListDeleted(object sender, TaskListViewModel taskListVm)
        {
            if (!TaskListsViewModels.Contains(taskListVm)) return;

            TaskListsViewModels.Remove(taskListVm);
            TaskListsChanged?.Invoke(this, EventArgs.Empty);
        }

        #endregion
    }
}
