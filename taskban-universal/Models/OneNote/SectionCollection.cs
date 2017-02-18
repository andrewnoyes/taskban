using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using SQLite.Net.Attributes;
using TamedTasks.Models.Base;

namespace TamedTasks.Models.OneNote
{

    public class ParentNotebook
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("self")]
        public string Self { get; set; }
    }

    public class Section : ObservableDbEntity
    {
        [JsonProperty("isDefault")]
        public bool IsDefault { get; set; }

        [JsonProperty("pagesUrl")]
        public string PagesUrl { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("createdBy")]
        public string CreatedBy { get; set; }

        [JsonProperty("lastModifiedBy")]
        public string LastModifiedBy { get; set; }

        [JsonProperty("lastModifiedTime")]
        public DateTime LastModifiedTime { get; set; }

        [JsonProperty("createdTime")]
        public DateTime CreatedTime { get; set; }

        [JsonProperty("self")]
        public string Self { get; set; }

        [JsonProperty("parentNotebook@odata.context")]
        public string ParentNotebookODataContext { get; set; }

        [JsonProperty("parentNotebook"), Ignore]
        public ParentNotebook ParentNotebook { get; set; }

        public string ParentNotebookId { get; set; }

        [JsonProperty("parentSectionGroup@odata.context")]
        public string ParentSectionGroupODataContext { get; set; }

        [JsonProperty("parentSectionGroup"), Ignore]
        public object ParentSectionGroup { get; set; }

        public int ParentSectionGroupId { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }

    public class SectionCollection
    {
        [JsonProperty("@odata.context")]
        public string ODataContext { get; set; }
        [JsonProperty("value")]
        public IList<Section> Sections { get; set; }
    }



}
