using System;
using TamedTasks.Models.Base;

namespace TamedTasks.Models.Common
{
    public class Project : DbEntity
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime? StartDate { get; set; } 
        public DateTime? EndDate { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}
