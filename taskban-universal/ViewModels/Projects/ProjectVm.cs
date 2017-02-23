using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using taskban_universal.ViewModels.Boards;
using TamedTasks.Data;
using TamedTasks.Models.Common;
using Template10.Mvvm;

namespace taskban_universal.ViewModels.Projects
{
    public class ProjectVm : ViewModelBase
    {
        private Project _project;
        public Project Project
        {
            get { return _project; }
            set { Set(ref _project, value); }
        }

        private ObservableCollection<BoardVm> _boardViewModels;
        public ObservableCollection<BoardVm> BoardViewModels
        {
            get
            {
                if (_boardViewModels != null) return _boardViewModels;

                _boardViewModels = new ObservableCollection<BoardVm>();
                foreach (var board in DbManager.Instance.GetAllBoards(Project.Id))
                    _boardViewModels.Add(new BoardVm(board));

                return _boardViewModels;
            }
            set { Set(ref _boardViewModels, value); }
        }


        public ProjectVm(Project project)
        {
            Project = project;


        }




    }
}
