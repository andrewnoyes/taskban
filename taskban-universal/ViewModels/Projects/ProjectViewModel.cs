using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using TamedTasks.Data;
using TamedTasks.Models.Base;
using TamedTasks.Models.Common;
using Template10.Mvvm;

namespace TamedTasks.ViewModels.Projects
{
    public class ProjectViewModel : ViewModelBase
    {
        private ObservableCollection<Project> _projects;
        public ObservableCollection<Project> Projects
        {
            get
            {
                return _projects
                    ?? (_projects = new ObservableCollection<Project>(DbManager.Instance.GetAllProjects()));
            }
            set { Set(ref _projects, value); }
        }

        private Project _project;
        public Project Project
        {
            get { return _project; }
            set { Set(ref _project, value); }
        }

        public EditProjectViewModel EditProjectViewModel
        {
            get
            {
                var vm = new EditProjectViewModel(_project);
                vm.ProjectUpdated += OnProjectUpdated;
                return vm;
            }
        }

        public NewProjectViewModel NewProjectViewModel
        {
            get
            {
                var vm = new NewProjectViewModel();
                vm.ProjectAdded += OnProjectAdded;
                return vm;
            }
        }

        private RelayCommand _editProjectCommand;
        public RelayCommand EditProjectCommand =>
            _editProjectCommand ??
            (_editProjectCommand = new RelayCommand(o => Debug.WriteLine("Test"), p => Project != null));

        private void OnProjectUpdated(object sender, Project project)
        {
            if (_project != null)
            {
                var index = Projects.IndexOf(_project);
                if (index >= 0) // should always be >= 0
                {
                    if (project == null) // project was deleted
                    {
                        Projects.RemoveAt(index);
                        if (Projects.Count > 0)
                        {
                            var next = index > 0 ? index - 1 : index + 1;
                            project = Projects[next]; // set it to next item in list
                        }
                    }
                    else
                    {
                        Projects[index] = project;
                    }
                }
            }

            Project = project;
        }

        private void OnProjectAdded(object sender, Project project)
        {
            Projects.Add(project);
            Project = project;
        }
    }
}
