using System;
using System.Diagnostics;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Microsoft.CSharp.RuntimeBinder;
using TamedTasks.Models.Common;
using TamedTasks.ViewModels.Boards;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace TamedTasks.Views.Boards
{
    public sealed partial class BoardsList : UserControl
    {
        public BoardsList()
        {
            this.InitializeComponent();
        }

        private async void OnNewBoard(object sender, RoutedEventArgs e)
        {
            try
            {
                dynamic vm = DataContext;
                await new UpdateBoard { DataContext = vm.AddBoardViewModel }.ShowAsync();
            }
            catch (RuntimeBinderException ex)
            {
                Debug.WriteLine($"on new board: {ex}");
            }
        }

        private async void OnEditBoard(object sender, RoutedEventArgs e)
        {
            try
            {
                dynamic vm = DataContext;
                await new UpdateBoard { DataContext = vm.EditBoardViewModel }.ShowAsync();
            }
            catch (RuntimeBinderException ex)
            {
                Debug.WriteLine($"on edit board: {ex}");
            }
        }

        private async void OnDeleteBoard(object sender, RoutedEventArgs e)
        {
            try
            {
                dynamic vm = DataContext;
                var dlg = new ContentDialog
                {
                    Title = "Delete Board",
                    Content = "\n" +
                      "Are you sure you want to delete this board? \n\n" +
                      "All lists and task items on this board will also be deleted.",
                    PrimaryButtonText = "DELETE",
                    SecondaryButtonText = "CANCEL",
                };
                dlg.PrimaryButtonClick += (o, a) => vm.DeleteSelectedBoard();
                await dlg.ShowAsync();
            }
            catch (RuntimeBinderException ex)
            {
                Debug.WriteLine($"on edit board: {ex}");
            }
        }

        private void OnBoardsListRightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            var vm = DataContext as BoardsListViewModel;
            if (vm == null) return;

            try
            {
                dynamic source = e.OriginalSource;
                var board = source.DataContext;

                // check if right-click target is not currently selected board
                if (board != null && board != vm.Board) vm.Board = board;

                BoardsListMenu.ShowAt(BoardsListView, e.GetPosition(BoardsListView));
            }
            catch (RuntimeBinderException ex)
            {
                Debug.WriteLine($"Right tapping boards list {ex}");
            }
        }
    }
}
