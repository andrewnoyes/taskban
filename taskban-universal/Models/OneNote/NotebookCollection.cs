using System.Collections.Generic;
using Newtonsoft.Json;

namespace TamedTasks.Models.OneNote
{
    public class NotebookCollection 
    {
        [JsonProperty("@odata.context")]
        public string ODataContext { get; set; }
        [JsonProperty("value")]
        public IList<Notebook> Notebooks { get; set; }
    }
}
