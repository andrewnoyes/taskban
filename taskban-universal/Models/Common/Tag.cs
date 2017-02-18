using Windows.UI.Xaml.Controls;
using TamedTasks.Models.Base;

namespace TamedTasks.Models.Common
{
    public class Tag : ObservableDbEntity
    {
        public string DisplayName { get; set; }

        public string HexColor { get; set; }

        public Image Symbol { get; set; } // TODO: need to figure out a good way to store this

        /// <summary>
        /// The name of the UWP Symbol Icon, if picked from that collection.
        /// </summary>
        public string SymbolName { get; set; }
    }
}
