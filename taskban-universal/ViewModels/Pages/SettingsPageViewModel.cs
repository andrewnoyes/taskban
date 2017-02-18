using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using TamedTasks.Data;
using TamedTasks.Models.Base;
using TamedTasks.Models.OneNote;
using TamedTasks.Services;
using Template10.Mvvm;
using SettingsService = TamedTasks.Services.SettingsService;

namespace TamedTasks.ViewModels.Pages
{
    public class SettingsPageViewModel : ViewModelBase
    {
        public AccountsPartViewModel AccountsPartViewModel { get; } = new AccountsPartViewModel();
        //public ThemesPartViewModel ThemesPartViewModel { get; } = new ThemesPartViewModel();
        public AboutPartViewModel AboutPartViewModel { get; } = new AboutPartViewModel();
    }

    public class AccountsPartViewModel : ViewModelBase
    {
        private RelayCommand _connectMicrosoftAccount;
        public RelayCommand ConnectMicrosoftCommand
            => _connectMicrosoftAccount ??
               (_connectMicrosoftAccount = new RelayCommand(a => OneNoteManager.Instance.ConnectMicrosoftAccount()));
    }

    public class ImportNotebooksViewModel : ViewModelBase
    {
        public ImportNotebooksViewModel()
        {
            Initialize();
        }

        private async void Initialize()
        {
            IsLoading = true;

            var notebooks = await OneNoteManager.FetchNotebooksAsync();
            Notebooks = new ObservableCollection<Notebook>(notebooks);

            IsLoading = false;
        }

        private bool _isLoading;
        public bool IsLoading
        {
            get { return _isLoading; }
            set { Set(ref _isLoading, value); }
        }

        public OneNoteManager OneNoteManager { get; } = OneNoteManager.Instance;

        private ObservableCollection<Notebook> _notebooks = new ObservableCollection<Notebook>();
        public ObservableCollection<Notebook> Notebooks
        {
            get { return _notebooks; }
            set { Set(ref _notebooks, value); }
        }

        private RelayCommand _selectAll;
        public RelayCommand SelectAll
            => _selectAll ?? (_selectAll = new RelayCommand(a =>
            {
                foreach (var nb in Notebooks) nb.ImportIntoDb = true;
            }));

        private RelayCommand _importNotebooks;
        public RelayCommand ImportNotebooks
            => _importNotebooks ?? (_importNotebooks =
                new RelayCommand(a => ImportNotebooksAsync(), p =>
                {
                    return Notebooks.Any(n => n.ImportIntoDb);
                }));

        public async void ImportNotebooksAsync()
        {
            var notebooks = new List<Notebook>(Notebooks);
            await Task.Run(() => OneNoteManager.ImportNotebooks(notebooks));
        }
    }

    public class ThemesPartViewModel : ViewModelBase
    {
        Services.SettingsService _settings;

        public ThemesPartViewModel()
        {
            if (Windows.ApplicationModel.DesignMode.DesignModeEnabled)
            {
                // designtime
            }
            else
            {
                _settings = SettingsService.Instance;
            }
        }

        public bool UseShellBackButton
        {
            get { return _settings.UseShellBackButton; }
            set { _settings.UseShellBackButton = value; base.RaisePropertyChanged(); }
        }

        public bool UseLightThemeButton
        {
            get { return _settings.AppTheme.Equals(ApplicationTheme.Light); }
            set { _settings.AppTheme = value ? ApplicationTheme.Light : ApplicationTheme.Dark; base.RaisePropertyChanged(); }
        }

        private string _BusyText = "Please wait...";
        public string BusyText
        {
            get { return _BusyText; }
            set
            {
                Set(ref _BusyText, value);
                _ShowBusyCommand.RaiseCanExecuteChanged();
            }
        }

        DelegateCommand _ShowBusyCommand;
        public DelegateCommand ShowBusyCommand
            => _ShowBusyCommand ?? (_ShowBusyCommand = new DelegateCommand(async () =>
            {
                Views.Busy.SetBusy(true, _BusyText);
                await Task.Delay(500);
                Views.Busy.SetBusy(false);
            }, () => !string.IsNullOrEmpty(BusyText)));
    }

    public class AboutPartViewModel : ViewModelBase
    {
        public Uri Logo => Windows.ApplicationModel.Package.Current.Logo;

        public string DisplayName => Windows.ApplicationModel.Package.Current.DisplayName;

        public string Publisher => Windows.ApplicationModel.Package.Current.PublisherDisplayName;

        public string Version
        {
            get
            {
                var v = Windows.ApplicationModel.Package.Current.Id.Version;
                return $"{v.Major}.{v.Minor}.{v.Build}.{v.Revision}";
            }
        }
    }

}

