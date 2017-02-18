// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

using System;
using System.Diagnostics;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using TamedTasks.ViewModels.Projects;

namespace TamedTasks.Views.Projects
{
    public sealed partial class EditProject 
    {
        public EditProject()
        {
            this.InitializeComponent();
        }

        private void OnCancel(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            Debug.WriteLine("Edit project cancelled");
        }

        private void OnSave(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            var vm = DataContext as EditProjectViewModel;
            vm?.UpdateProject();
        }

        private async void OnDelete(object sender, RoutedEventArgs e)
        {
            const string msg =
            "Are you sure you want to delete this project? \n\n" +
            "All boards, lists, and tasks tied to this project will also be deleted.";
            var dlg = new MessageDialog(msg, "Delete Project");
            dlg.Commands.Add(new UICommand("CANCEL", cmd => Debug.WriteLine("Cancelled deleting project")));
            dlg.Commands.Add(new UICommand("DELETE", cmd => OnDelete() ));
            dlg.CancelCommandIndex = 0;
            await dlg.ShowAsync();
        }

        private void OnDelete()
        {
            var vm = DataContext as EditProjectViewModel;
            vm?.DeleteProject();
            Hide();
        }
    }
}
