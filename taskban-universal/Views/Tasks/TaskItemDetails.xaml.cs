using System.Diagnostics;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Microsoft.CSharp.RuntimeBinder;
using taskban_universal.ViewModels.Tasks;
using TamedTasks.Models.Common;
using TamedTasks.ViewModels.Tasks;
using WinRTXamlToolkit.Controls.Extensions;

namespace TamedTasks.Views.Tasks
{
    public sealed partial class TaskItemDetails
    {
        private TaskItemVm _viewModel;
        private TaskItemVm ViewModel => _viewModel ?? (_viewModel = DataContext as TaskItemVm);

        public TaskItemDetails()
        {
            InitializeComponent();
        }

        private void OnChecklistItemKeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key != VirtualKey.Enter) return;

            try
            {
                var checkVm = ViewModel.ChecklistViewModel;
                if (!checkVm.CanAddChecklistItem()) return;

                checkVm.AddChecklistItem();
                Checklist?.ScrollToBottom();
            }
            catch (RuntimeBinderException ex)
            {
                Debug.WriteLine($"checklist enter key ex: {ex}");
            }
        }

        private void OnToggleChecklist(object sender, RoutedEventArgs e)
        {
            if (ToggleChecklist?.IsChecked != null && ToggleChecklist?.IsChecked.Value == true)
            {
                ChecklistInput?.Focus(FocusState.Keyboard);
            }
        }

        private async void OnDeleteChecklistItem(object sender, RoutedEventArgs e)
        {
            try
            {
                dynamic obj = sender;
                var cItem = obj.DataContext as ChecklistItem;
                if (cItem == null) return;

                await ViewModel.ChecklistViewModel.DeleteChecklistItemAsync(cItem);
            }
            catch (RuntimeBinderException ex)
            {
                Debug.WriteLine($"checklist item delete: {ex}");
            }
        }

        private void OnCloseDetails(object sender, RoutedEventArgs e)
        {
            Hide();
        }

        private void OnDetailsClosing(ContentDialog sender, ContentDialogClosingEventArgs args)
        {
            ViewModel.SaveTaskItem();
        }
        
        private void OnChecklistComplete(object sender, RoutedEventArgs e)
        {
            Debug.Write("wtf");
        }

        private void UIElement_OnKeyDown(object sender, KeyRoutedEventArgs e)
        {
            e.Handled = true;
        }
    }
}
