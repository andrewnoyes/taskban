using TamedTasks.Models.Base;

namespace TamedTasks.Models.Common
{
    public class TaskList : ObservableDbEntity
    {
        /// <summary>
        /// Title for this task list.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Optional description for list.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// The index of the tasklist in the board.
        /// </summary>
        public int Order { get; set; }

        /// <summary>
        /// Background color displayed in board.
        /// </summary>
        public byte[] Color { get; set; }

        /// <summary>
        /// Foreign key for board this list belongs to.
        /// </summary>
        public string BoardId { get; set; }
    }
}
