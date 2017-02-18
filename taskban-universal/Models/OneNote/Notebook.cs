using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Newtonsoft.Json;
using SQLite.Net.Attributes;
using TamedTasks.Models.Base;
using TamedTasks.Properties;

namespace TamedTasks.Models.OneNote
{
    public class OneNoteClientUrl 
    {
        [JsonProperty("href")]
        public string Href { get; set; }
    }

    public class OneNoteWebUrl 
    {
        [JsonProperty("href")]
        public string Href { get; set; }
    }

    public class Links 
    {
        [JsonProperty("oneNoteClientUrl")]
        public OneNoteClientUrl OneNoteClientUrl { get; set; }
        [JsonProperty("oneNoteWebUrl")]
        public OneNoteWebUrl OneNoteWebUrl { get; set; }
    }

    public class Notebook : ObservableDbEntity, INotifyPropertyChanged
    {
        [JsonProperty("isDefault")]
        public bool IsDefault { get; set; }

        [JsonProperty("userRole")]
        public string UserRole { get; set; }

        [JsonProperty("isShared")]
        public bool IsShared { get; set; }

        [JsonProperty("sectionsUrl")]
        public string SectionsUrl { get; set; }

        [JsonProperty("sectionGroupsUrl")]
        public string SectionGroupsUrl { get; set; }

        [JsonProperty("links"), Ignore]
        public Links Links { get; set; }

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

        private bool _importIntoDb;

        public bool ImportIntoDb
        {
            get {return _importIntoDb;}
            set
            {
                _importIntoDb = value;
                OnPropertyChanged();
            }
        }

        public override string ToString()
        {
            return Name;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
