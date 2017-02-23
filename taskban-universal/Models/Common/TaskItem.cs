using System.ComponentModel.DataAnnotations.Schema;
using TamedTasks.Models.Base;

namespace TamedTasks.Models.Common
{
    public class TaskItem : ObservableDbEntity
    {
        public enum Progress
        {
            Backlog = 0,
            InProgress = 1,
            Complete = 2
        }

        /// <summary>
        /// Contains either decoded HTML string from OneNote page or user input.
        /// </summary>
        private string _title;
        public string Title
        {
            get { return _title;}
            set { _title = value; OnPropertyChanged(); }
        }

        // todo: probably not needed
        public string Contents { get; set; }

        public string HtmlContent { get; set; }

        private string _description;
        public string Description
        {
            get { return _description; }
            set { _description = value; OnPropertyChanged(); }
        }

        public Progress TaskProgress { get; set; }

        /// <summary>
        /// Foreign key for parent PageContent.
        /// </summary>
        public string PageContentId { get; set; }

        /// <summary>
        /// Foreign key for parent TaskItem (then this is subtask).
        /// </summary>
        public string TaskItemId { get; set; }

        /// <summary>
        /// Foreign key for task list this item belongs to.
        /// </summary>
        public string TaskListId { get; set; }
    }
}
