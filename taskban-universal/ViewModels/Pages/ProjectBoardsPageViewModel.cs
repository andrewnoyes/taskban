using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using taskban_universal.ViewModels.Projects;
using TamedTasks.Data;
using TamedTasks.ViewModels.Projects;
using Template10.Mvvm;

namespace taskban_universal.ViewModels.Pages
{
    public class ProjectBoardsPageViewModel : ViewModelBase
    {
        private ObservableCollection<ProjectVm> _projectViewModels;
        public ObservableCollection<ProjectVm> ProjectViewModels
        {
            get
            {
                if (_projectViewModels != null) return _projectViewModels;

                _projectViewModels = new ObservableCollection<ProjectVm>();
                var projects = DbManager.Instance.GetAllProjects();
                foreach (var project in projects)
                {
                    _projectViewModels.Add(new ProjectVm(project));
                }
                return _projectViewModels;
            }
            set { Set(ref _projectViewModels, value); }
        }

        private ProjectVm _selectedProjectViewModel;
        public ProjectVm SelectedProjectViewModel
        {
            get { return _selectedProjectViewModel; }
            set { Set(ref _selectedProjectViewModel, value); }
        }

        public ProjectBoardsPageViewModel()
        {
            if (ProjectViewModels.Count > 0)
            {
                SelectedProjectViewModel = ProjectViewModels[0]; // TODO: temp just for testing
            }
        }


    }
}
