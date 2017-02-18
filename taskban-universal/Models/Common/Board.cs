using TamedTasks.Models.Base;

namespace TamedTasks.Models.Common
{
    public class Board : ObservableDbEntity
    {
        /// <summary>
        /// Title of this board.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Optional description for the board.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Foreign key for project this board belongs to.
        /// </summary>
        public string ProjectId { get; set; }

        /// <summary>
        /// Background color of board.
        /// </summary>
        public byte[] Color { get; set; }

        public override string ToString()
        {
            return Title;
        }
    }
}
