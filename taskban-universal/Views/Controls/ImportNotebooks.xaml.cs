using TamedTasks.ViewModels;
using TamedTasks.ViewModels.Pages;

namespace TamedTasks.Views.Controls
{
    public sealed partial class ImportNotebooks
    {
        public ImportNotebooks()
        {
            InitializeComponent();
            DataContext = new ImportNotebooksViewModel();
        }
    }
}
