using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using SQLite.Net.Attributes;
using TamedTasks.Models.Base;

namespace TamedTasks.Models.OneNote
{
    public class ParentSection
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("self")]
        public string Self { get; set; }
    }

    public class Page : ObservableDbEntity
    {
        [JsonProperty("title")]
        public string Title { get; set; }
        [JsonProperty("createdByAppId")]
        public string CreatedByAppId { get; set; }
        [JsonProperty("links"), Ignore]
        public Links Links { get; set; }
        [JsonProperty("contentUrl")]
        public string ContentUrl { get; set; }
        [JsonProperty("lastModifiedTime")]
        public DateTime LastModifiedTime { get; set; }
        [JsonProperty("level")]
        public int Level { get; set; }
        [JsonProperty("order")]
        public int Order { get; set; }
        [JsonProperty("createdTime")]
        public DateTime CreatedTime { get; set; }
        [JsonProperty("self")]
        public string Self { get; set; }
        [JsonProperty("parentSection@odata.context")]
        public string ParentSectionODataContext { get; set; }
        [JsonProperty("parentSection"), Ignore]
        public ParentSection ParentSection { get; set; }
        public string ParentSectionId { get; set; }

        public override string ToString()
        {
            return Title;
        }
    }

    public class PageCollection
    {
        [JsonProperty("@odata.context")]
        public string ODataContext { get; set; }
        [JsonProperty("value")]
        public IList<Page> Pages { get; set; }
        [JsonProperty("@odata.nextLink")]
        public string ODataNextLink { get; set; }
    }


}
