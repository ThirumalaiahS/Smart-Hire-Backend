using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Velora.Core.Entities
{
    public class RefreshToken
    {
        public int RefreshTokenId { get; set; }
        public int SysUserId { get; set; }
        public string Token {  get; set; } = string.Empty;        
        public string JwtId { get; set; } = string.Empty;
        public DateTime ExpiresOn { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime? RevokedOn { get; set; }
        public string? CreatedByIp { get; set; }
        public string? RevokedByIp { get; set; }
        public bool IsRevoked { get; set; }
        public bool IsUsed { get; set; }
        public string? ReplacedByToken { get; set; }
        public SystemUser SystemUser { get; set; } = null!;
    }
}
