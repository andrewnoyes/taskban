using TamedTasks.Models.Base;

namespace TamedTasks.Models.Common
{
    public class ChecklistItem : ObservableDbEntity
    {
        /// <summary>
        /// Contents of this item.
        /// </summary>
        public string Contents { get; set; }

        /// <summary>
        /// Gets/sets whether item is complete.
        /// </summary>
        private bool _isComplete;
        public bool IsComplete
        {
            get { return _isComplete;}
            set
            {
                _isComplete = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Foreign key for the task item this checklist item belongs to.
        /// </summary>
        public string TaskItemId { get; set; }
    }
}
