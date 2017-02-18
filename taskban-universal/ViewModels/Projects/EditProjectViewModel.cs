using System;
using TamedTasks.Data;
using TamedTasks.Models.Common;
using Template10.Mvvm;

namespace TamedTasks.ViewModels.Projects
{
    public class EditProjectViewModel : ViewModelBase
    {
        private Project _project;
        public Project Project
        {
            get { return _project; }
            set
            {
                Set(ref _project, value);
            }
        }

        private DateTime _start;
        public DateTime Start
        {
            get { return _start; }
            set
            {
                Set(ref _start, value);
            }
        }

        private DateTime _end;
        public DateTime End
        {
            get { return _end; }
            set
            {
                Set(ref _end, value);
            }
        }

        private DateTime _createdDate;
        public DateTime CreatedDate
        {
            get { return _createdDate; }
            set
            {
                Set(ref _createdDate, value);
            }
        }

        public event EventHandler<Project> ProjectUpdated;

        public EditProjectViewModel(Project project)
        {
            Project = project;
            if (Project.DateCreated != null)
            {
                CreatedDate = Project.DateCreated.Value;
            }
        }

        // todo: these can be moved to the commands, and the dialog can bind to them
        public void DeleteProject()
        {
            DbManager.Instance.DeleteEntity(Project);
            ProjectUpdated?.Invoke(this, null);
        }

        public void UpdateProject()
        {
            Project.StartDate = Start;
            Project.EndDate = End;
            var project = DbManager.Instance.UpdateEntity(Project);
            ProjectUpdated?.Invoke(this, project);
        }
    }
}
