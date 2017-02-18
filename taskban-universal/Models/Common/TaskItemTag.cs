using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TamedTasks.Models.Base;

namespace TamedTasks.Models.Common
{
    /// <summary>
    /// Associative entity to handle m-m relationship b/w TaskItem and Tag.
    /// </summary>
    public class TaskItemTag : DbEntity
    {
        public string TaskItemId { get; set; }
        public string TagId { get; set; }
    }
}
