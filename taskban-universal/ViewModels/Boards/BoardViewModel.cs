using System;
using Windows.UI.Xaml.Media;
using TamedTasks.Models;
using TamedTasks.Data;
using TamedTasks.Models.Common;
using Template10.Mvvm;

namespace TamedTasks.ViewModels.Boards
{
    public class BoardViewModel : ViewModelBase
    {
        private string _title;
        public string Title
        {
            get { return _title; }
            set
            {
                _title = value;
                RaisePropertyChanged();
                SaveBoardCommand.RaiseCanExecuteChanged();
            }
        }

        public string Description { get; set; }
        public string ProjectId { get; private set; }
        public string ProjectName { get; private set; }

        private DelegateCommand _saveBoardCommand;
        public DelegateCommand SaveBoardCommand =>
            _saveBoardCommand ??
            (_saveBoardCommand = new DelegateCommand(SaveBoard, IsBoardValid));

        public event EventHandler<Board> BoardUpdated;

        private Board _board;

        private SolidColorBrush _background;
        public SolidColorBrush Background
        {
            get { return _background ?? (_background = Models.Util.GetBoardBrush(_board)); }
            set { Set(ref _background, value); }
        }

        public BoardViewModel(string projectId, string projectName, Board board = null)
        {
            ProjectId = projectId;
            ProjectName = projectName;
            if (board == null) return;

            _board = board;
            Title = _board.Title;
            Description = _board.Description;
        }

        private void SaveBoard()
        {
            Board board;

            if (_board != null)
            {
                _board.Title = Title;
                _board.Description = Description;
                _board.ProjectId = ProjectId;
                board = _board;
            }
            else
            {
                board = new Board
                {
                    Title = Title,
                    Description = Description,
                    ProjectId = ProjectId
                };
            }

            board.Color = Models.Util.ConvertBrushToByteArray(Background);
            BoardUpdated?.Invoke(this, DbManager.Instance.InsertOrUpdateEntity(board));
        }

        private bool IsBoardValid()
        {
            return !string.IsNullOrEmpty(Title);
        }
    }
}
