using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using Windows.UI;
using Windows.UI.Xaml.Media;
using TamedTasks.Data;
using TamedTasks.Models.Common;
using Template10.Mvvm;

namespace TamedTasks.ViewModels.Boards
{
    public class BoardsListViewModel : ViewModelBase
    {
        private Board _board;
        public Board Board
        {
            get { return _board; }
            set
            {
                Set(ref _board, value);
                EditBoardCommand.RaiseCanExecuteChanged();
                Background = Models.Util.GetBoardBrush(value);
                BoardChanged?.Invoke(this, value);
            }
        }

        public EventHandler<Board> BoardChanged;

        private ObservableCollection<Board> _boards;
        public ObservableCollection<Board> Boards
        {
            get { return _boards ?? (_boards = new ObservableCollection<Board>()); }
            set { Set(ref _boards, value); }
        }

        private SolidColorBrush _background;
        public SolidColorBrush Background
        {
            get { return _background ?? (_background = Models.Util.GetBoardBrush(Board)); }
            set { Set(ref _background, value); }
        }

        public Project Project { get; private set; }

        private DelegateCommand _editBoardCommand;
        public DelegateCommand EditBoardCommand =>
            _editBoardCommand ??
            (_editBoardCommand = new DelegateCommand(() =>
                Debug.WriteLine("edit board command..."),
                IsBoardSelected));

        public BoardsListViewModel(Project project)
        {
            if (project == null)
            {
                throw new ArgumentException(nameof(project));
            }

            UpdateBoards(project);
        }

        public void UpdateBoards(Project project)
        {
            if (project == null) return; // todo: should be handled prior to this call

            Project = project;
            Boards = new ObservableCollection<Board>(DbManager.Instance.GetAllBoards(Project.Id));

            if (Boards.Count > 0) Board = Boards[0]; // todo: temp
        }

        public void DeleteSelectedBoard()
        {
            DbManager.Instance.DeleteBoardAndReferences(Board);
            var index = Boards.IndexOf(Board);
            if (index < 0) return; // todo: shouldn't happen, throw index ex

            Boards.RemoveAt(index);

            if (Boards.Count <= 0) return;

            var newIndex = index + 1 < Boards.Count ? index + 1 : index - 1;
            if (newIndex >= 0 && newIndex < Boards.Count)
            {
                Board = Boards[newIndex];
            }
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

            Board.Color = c;
            DbManager.Instance.UpdateEntity(Board);
        }

        public BoardViewModel AddBoardViewModel => GetBoardViewModel();
        public BoardViewModel EditBoardViewModel => GetBoardViewModel(true);

        #region Private

        private bool IsBoardSelected()
        {
            return Board != null;
        }

        private BoardViewModel GetBoardViewModel(bool isEditMode = false)
        {
            BoardViewModel vm;

            if (Board != null && isEditMode)
            {
                vm = new BoardViewModel(Project.Id, Project.Name, Board);
            }
            else
            {
                vm = new BoardViewModel(Project.Id, Project.Name);
            }

            vm.BoardUpdated += OnBoardUpdated;
            return vm;
        }

        private SolidColorBrush GetBoardBrush()
        {
            SolidColorBrush brush;
            var color = Board.Color;
            if (color == null || color.Length != 4) // color is stored as argb byte array
            {
                brush = new SolidColorBrush(Color.FromArgb(255, 238, 238, 238));
            }
            else
            {
                brush = new SolidColorBrush(Color.FromArgb(color[0], color[1], color[2], color[3]));
            }

            return brush;
        }

        private void OnBoardUpdated(object sender, Board board)
        {
            var current = Boards.FirstOrDefault(b => b.Id == board.Id);
            if (current != null)
            {
                var index = Boards.IndexOf(current);
                if (index >= 0)
                {
                    Boards[index] = board;
                }
            }
            else
            {
                Boards.Add(board);
            }

            Board = board;
            Background = GetBoardBrush();
        }

        #endregion
    }
}
