using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHire.Core.Entities
{
    public class UserSettings
    {
        public string UserId { get; set; } = string.Empty;
        public int DataProviderId { get; set; }

        // Navigation property to DataProvider
        public DataProvider DataProvider { get; set; }
    }
}
