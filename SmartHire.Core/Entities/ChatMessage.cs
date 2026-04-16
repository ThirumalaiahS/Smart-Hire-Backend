using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHire.Core.Entities
{
    public class ChatMessage
    {
        public int Id { get; set; }
        public string UserId { get; set; } = string.Empty;
        public AppUser User { get; set; } = null!;
        public string Role { get; set; } = "user";
        public string Content { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
