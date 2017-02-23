using Windows.UI.Xaml;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Template10.Controls;
using Windows.UI.Xaml.Data;
using TamedTasks.Data;
using TamedTasks.Services;
using TamedTasks.Views.Pages;

namespace TamedTasks
{
    /// Documentation on APIs used in this page:
    /// https://github.com/Windows-XAML/Template10/wiki

    [Bindable]
    sealed partial class App : Template10.Common.BootStrapper
    {
        public App()
        {
            InitializeComponent();
            SplashFactory = (e) => new Views.Splash(e);
            #region App settings

            var _settings = SettingsService.Instance;
            RequestedTheme = _settings.AppTheme;
            CacheMaxDuration = _settings.CacheMaxDuration;
            ShowShellBackButton = _settings.UseShellBackButton;

            #endregion
        }

        public override async Task OnInitializeAsync(IActivatedEventArgs args)
        {
            if (Window.Current.Content as ModalDialog == null)
            {
                // create a new frame 
                var nav = NavigationServiceFactory(BackButton.Attach, ExistingContent.Include);

                // create modal root
                Window.Current.Content = new ModalDialog
                {
                    DisableBackButtonWhenModal = true,
                    Content = new Views.Shell(nav),
                    ModalContent = new Views.Busy(),
                };
            }
            await Task.CompletedTask;
        }

        public override async Task OnStartAsync(StartKind startKind, IActivatedEventArgs args)
        {
            // initialize any existing accounts
            //OneNoteManager.Instance.Initialize(); todo: disabling until better implementation

            //await Task.Delay(1000);

            //NavigationService.Navigate(typeof(Views.MainPage)); todo: disabling main page for now -- this will eventually load the last saved state
            NavigationService.Navigate(typeof(ProjectBoardsPage));
            await Task.CompletedTask;
        }

        public override async Task OnSuspendingAsync(object s, SuspendingEventArgs e, bool prelaunchActivated)
        {
            await Task.CompletedTask; // TODO: need a way to notify listeners of app shutdown - this way any content can be updated
        }
    }
}

