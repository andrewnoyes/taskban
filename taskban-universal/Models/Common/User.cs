using SQLite.Net.Attributes;

namespace TamedTasks.Models.Common
{
    // TODO: add user's app preferences
    public class User 
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public long HasMicrosoftAccountConnected { get; set; }
    }
}
