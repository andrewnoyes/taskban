using System;
using System.Globalization;
using TamedTasks.Data;
using TamedTasks.Models.Common;
using Template10.Mvvm;

namespace TamedTasks.ViewModels.Projects
{

    public class NewProjectViewModel : ViewModelBase
    {
        private string _name;
        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                RaisePropertyChanged();
            }
        }

        private DateTime _start;
        public DateTime Start
        {
            get { return _start; }
            set
            {
                _start = value;
                RaisePropertyChanged();
            }
        }

        private DateTime _end;
        public DateTime End
        {
            get { return _end; }
            set
            {
                _end = value;
                RaisePropertyChanged();
            }
        }

        private DelegateCommand _addProjectCommand;
        public DelegateCommand AddProjectCommand =>
            _addProjectCommand ??
            (_addProjectCommand = new DelegateCommand(CreateProject, CanCreateProject));


        public event EventHandler<Project> ProjectAdded;

        public void CreateProject()
        {
            var project = DbManager.Instance.InsertOrUpdateEntity(new Project
            {
                Name = Name,
                StartDate = Start,
                EndDate = End
            });
            
            ProjectAdded?.Invoke(this, project);
        }

        private bool CanCreateProject()
        {
            return !string.IsNullOrEmpty(Name);
        }
    }
}
