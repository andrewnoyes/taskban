using TamedTasks.Models.Base;

namespace TamedTasks.Models.OneNote
{
    public class PageContent : ObservableDbEntity
    {
        public string Html { get; set; }

        public string PageId { get; set; }
    }
}
