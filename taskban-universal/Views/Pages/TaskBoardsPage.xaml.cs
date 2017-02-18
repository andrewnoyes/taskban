using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Microsoft.CSharp.RuntimeBinder;
using TamedTasks.ViewModels.Boards;
using TamedTasks.ViewModels.Tasks;
using TamedTasks.Views.Boards;
using TamedTasks.Views.Projects;
using TamedTasks.Views.Tasks;
using Template10.Utils;
using WinRTXamlToolkit.Controls.Extensions;
using Page = Windows.UI.Xaml.Controls.Page;

namespace TamedTasks.Views.Pages
{
    /// <summary>
    /// Contains the projects and associated boards, 
    /// along with the selected board's tasklists.
    /// </summary>
    public sealed partial class TaskBoardsPage : Page
    {
        public TaskBoardsPage()
        {
            InitializeComponent();
            NavigationCacheMode = NavigationCacheMode.Enabled;
            ViewModel.TaskListsChanged += (o, e) => RefreshTaskLists();
        }

        private void RefreshTaskLists()
        {
            TaskHub.Sections.Clear();
            foreach (var taskList in ViewModel.TaskListsViewModels)
            {
                var section = new HubSection()
                {
                    DataContext = taskList,
                    ContentTemplate = TaskListTemplate,
                    Margin = new Thickness(0, 0, 0, 15),
                    CanDrag = true,
                    ManipulationMode = ManipulationModes.TranslateX
                };
                section.DragStarting += SectionOnDragStarting;
                section.DragOver += SectionOnDragOver;
                section.DragEnter += SectionOnDragEnter;
                section.DragLeave += SectionOnDragLeave;
                TaskHub.Sections.Add(section);
            }
        }

        private void SectionOnDragLeave(object sender, DragEventArgs dragEventArgs)
        {
            Debug.Write("");
        }

        private void SectionOnDragEnter(object sender, DragEventArgs dragEventArgs)
        {
            Debug.Write("");
        }

        #region Event handlers
        private async void OnNewTaskList(object sender, RoutedEventArgs e)
        {
            await new NewTaskList { DataContext = ViewModel.AddTaskListViewModel }.ShowAsync();
            AddFlyout.Hide();
        }

        private void OnListDragOver(object sender, DragEventArgs e)
        {
            e.AcceptedOperation = DataPackageOperation.Move;
        }

        private TaskListViewModel _sourceList;
        private TaskListViewModel SourceList
        {
            get { return _sourceList; }
            set
            {
                lock (_listLock)
                {
                    _sourceList = value;
                }
            }
        }
        private readonly object _listLock = new object();

        private async void OnListDrop(object sender, DragEventArgs e)
        {
            var target = (sender as ListView)?.DataContext as TaskListViewModel;
            if (target == null) return;

            if (!e.DataView.Contains(StandardDataFormats.Text)) return;

            var deferral = e.GetDeferral();
            var text = await e.DataView.GetTextAsync();
            var taskIds = text.Split(',');

            foreach (var id in taskIds)
            {
                if (target.TaskItemViewModels.Any(t => t.TaskItem.Id == id))
                    continue; // already exists

                var taskVm = SourceList.TaskItemViewModels.FirstOrDefault(t => t.TaskItem.Id == id);
                if (taskVm == null) continue; // shouldn't happen

                // remove the vm from the source
                SourceList.TaskItemViewModels.Remove(taskVm);

                // update taskListId FK before adding to target list
                taskVm.UpdateTaskListId(target.TaskList.Id);
                target.TaskItemViewModels.Add(taskVm);
            }

            deferral.Complete();
        }

        private void OnDragItemsStarting(object sender, DragItemsStartingEventArgs e)
        {
            var source = (sender as ListView)?.DataContext as TaskListViewModel;
            if (source == null) return;

            SourceList = source;

            var items = (from TaskItemViewModel item
                         in e.Items
                         where item != null
                         select item.TaskItem.Id).ToList();

            e.Data.SetText(string.Join(",", items));
            e.Data.RequestedOperation = DataPackageOperation.Move;
        }


        private HubSection _sourceSection;
        private readonly object _sectionLock = new object();
        private void SectionOnDragStarting(UIElement sender, DragStartingEventArgs args)
        {
            lock (_sectionLock)
            {
                _sourceSection = sender as HubSection;
            }
        }

        private void OnTaskListDrop(object sender, DragEventArgs e)
        {
            var source = TaskHub.Sections.IndexOf(_sourceSection);
            //var target = TaskHub.Sections.IndexOf(sender as HubSection);
        }

        private void SectionOnDragOver(object sender, DragEventArgs e)
        {
            Debug.WriteLine("");
        }

        private async void OnDeleteTaskList(dynamic sender, RoutedEventArgs e)
        {
            var taskListVm = sender.DataContext;
            if (taskListVm == null || taskListVm.DeleteTaskListCommand == null) return;

            var dlg = new ContentDialog
            {
                Title = "Delete Task list",
                Content = "\n" +
                          "Are you sure you want to delete this list? \n\n " +
                          "All tasks will also be deleted.",
                PrimaryButtonText = "DELETE",
                SecondaryButtonText = "CANCEL",
                PrimaryButtonCommand = taskListVm.DeleteTaskListCommand
            };

            await dlg.ShowAsync();
        }

        private async void OnTaskClick(object sender, ItemClickEventArgs e)
        {
            await new TaskItemDetails { DataContext = e.ClickedItem }.ShowAsync();
        }

        private async void OnTaskTitleEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue == null) return;
            if (!Convert.ToBoolean(e.NewValue)) return; // not enabled

            var title = sender as TextBox;
            if (title == null) return;

            await Dispatcher.GetDispatcherWrapper().DispatchAsync(() =>
            {
                title.Focus(FocusState.Keyboard);
            });
        }

        // hack to allow the enter key to save a task item
        private void OnTaskTitleKeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key != VirtualKey.Enter && e.Key != VirtualKey.Escape) return;

            try
            {
                dynamic obj = sender;
                var vm = obj.DataContext as TaskListViewModel;
                if (vm == null) return;

                if (e.Key == VirtualKey.Enter)
                {
                    if (vm.CanSaveTask())
                    {
                        vm.SaveTask();
                    }
                }
                else // escape - toggle the new task
                {
                    vm.AddTaskVisible = false;
                }
            }
            catch (RuntimeBinderException ex)
            {
                Debug.WriteLine($"task title key ex: {ex}");
            }
        }

        private async void OnHighlightClicked(object sender, RoutedEventArgs e)
        {
            try
            {
                var vm = (sender as dynamic).DataContext;
                if (vm == null) return;

                var picker = new ColorPicker.ColorPicker
                {
                    SelectedColor = vm.Background,
                    Margin = new Thickness(0, 10, 0, 0)
                };

                var dlg = new ContentDialog
                {
                    Title = "Change Task List Highlight",
                    Content = picker,
                    PrimaryButtonText = "SAVE",
                    SecondaryButtonText = "CANCEL",
                    Padding = new Thickness(10)
                };

                dlg.PrimaryButtonClick += delegate { vm.ChangeBackgroundColor(picker.SelectedColor); };

                await dlg.ShowAsync();
            }
            catch (RuntimeBinderException ex)
            {
                Debug.WriteLine($"on highlight clicked {ex}");
            }
        }

        private void OnEnter(object sender, PointerRoutedEventArgs e)
        {
            Window.Current.CoreWindow.PointerCursor = new CoreCursor(CoreCursorType.Hand, 0);
        }

        private void OnExit(object sender, PointerRoutedEventArgs e)
        {
            Window.Current.CoreWindow.PointerCursor = new CoreCursor(CoreCursorType.Arrow, 0);
        }

        private async void OnNewProject(object sender, RoutedEventArgs e)
        {
            try
            {
                await new NewProject { DataContext = ViewModel.ProjectViewModel.NewProjectViewModel }.ShowAsync();
                AddFlyout.Hide();
            }
            catch (RuntimeBinderException ex)
            {
                Debug.WriteLine($"add project ex:{ex}");
            }
        }

        private async void OnNewBoard(dynamic sender, RoutedEventArgs e)
        {
            try
            {
                await new UpdateBoard { DataContext = ViewModel.BoardsListViewModel.AddBoardViewModel }.ShowAsync();
                AddFlyout.Hide();
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
                await new UpdateBoard { DataContext = ViewModel.BoardsListViewModel.EditBoardViewModel }.ShowAsync();
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
                var dlg = new ContentDialog
                {
                    Title = "Delete Board",
                    Content = "\n" +
                      "Are you sure you want to delete this board? \n\n" +
                      "All lists and task items on this board will also be deleted.",
                    PrimaryButtonText = "DELETE",
                    SecondaryButtonText = "CANCEL"
                };
                dlg.PrimaryButtonClick += (o, a) => ViewModel.BoardsListViewModel.DeleteSelectedBoard();
                await dlg.ShowAsync();
            }
            catch (RuntimeBinderException ex)
            {
                Debug.WriteLine($"on edit board: {ex}");
            }
        }

        #endregion

        private void OnTaskListSizeChanged(object sender, SizeChangedEventArgs e)
        {
            //(sender as ListView)?.ScrollToBottom(); TODO: this causes weird shadowy, dark spot when the list is rendered
        }

        private async void OnClearList(object sender, RoutedEventArgs e)
        {
            var dlg = new ContentDialog
            {
                Title = "Clear Task List",
                Content = "\nClear the list of all tasks?",
                PrimaryButtonText = "CLEAR",
                SecondaryButtonText = "CANCEL"
            };

            var taskListVm = (sender as Button)?.DataContext as TaskListViewModel;
            if (taskListVm == null) return;

            dlg.PrimaryButtonClick += async (o, a) =>
            {
                await taskListVm.ClearAllTaskItemsAsync();
            };

            await dlg.ShowAsync();
        }

        private void TaskHub_OnDragStarting(UIElement sender, DragStartingEventArgs args)
        {
            Debug.WriteLine("");

        }

        private void TaskHub_OnDragOver(object sender, DragEventArgs e)
        {
            Debug.WriteLine("");

        }
    }
}
