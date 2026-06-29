using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Velora.Core.Entities
{
    public class DataProvider
    {
        public int Id { get; set; }
        public string Name { get; set; }

        // Navigation property to UserSettings
        public ICollection<UserSettings> UserSettings { get; set; }
    }
}
