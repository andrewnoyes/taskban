using System;
using System.Diagnostics;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Microsoft.CSharp.RuntimeBinder;

namespace TamedTasks.Views.Projects
{
    public sealed partial class ProjectPane : UserControl
    {
        public ProjectPane()
        {
            InitializeComponent();
        }

        private async void OnEditProject(object sender, RoutedEventArgs e)
        {
            dynamic vm = DataContext;
            try
            {
                await new EditProject { DataContext = vm.EditProjectViewModel }.ShowAsync();
            }
            catch (RuntimeBinderException ex)
            {
                Debug.WriteLine($"edit project ex:{ex}");
            }
        }

        private async void OnNewProject(object sender, RoutedEventArgs e)
        {
            dynamic vm = DataContext;
            try
            {
                await new NewProject { DataContext = vm.NewProjectViewModel }.ShowAsync();
            }
            catch (RuntimeBinderException ex)
            {
                Debug.WriteLine($"add project ex:{ex}");
            }
        }
    }
}
