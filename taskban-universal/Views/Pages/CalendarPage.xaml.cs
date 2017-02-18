using System;
using Windows.UI.Xaml.Controls;
using TamedTasks.ViewModels.Pages;
using Template10.Utils;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace TamedTasks.Views.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class CalendarPage : Page
    {
        private CalendarPageViewModel _viewModel;
        public CalendarPage()
        {
            this.InitializeComponent();
            _viewModel = new CalendarPageViewModel();
            DataContext = _viewModel;

            Initialize();
        }

        private void Initialize()
        {
        }
    }
}
