using System;
using Newtonsoft.Json;
using SQLite.Net.Attributes;

namespace TamedTasks.Models.Base
{
    public abstract class DbEntity
    {
        [JsonProperty("id"), PrimaryKey]
        public string Id { get; set; }

        public string CreatedDate { get; set; } // todo: this is being replaced with below

        public DateTime? DateCreated { get; set; }

        public DateTime? LastModified { get; set; }
        
        /// <summary>
        /// Gets/sets the entity's order (position) within a parent container;
        /// </summary>
        public int Order { get; set; }
    }
}
