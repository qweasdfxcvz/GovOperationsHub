using System;
using System.Collections.Generic;
using System.Text;

namespace GovOperationsHub.Domain.Identity
{
    public class UserSession
    {
        public int Id { get; set; }
        public string UserId { get; set; } = string.Empty; // سيرتبط بـ IdentityUser
        public string Jti { get; set; } = string.Empty; // JWT Token Id
        public string RefreshToken { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime ExpiresAt { get; set; }
        public bool IsRevoked { get; set; }
        public string IpAddress { get; set; } = string.Empty;
        public string UserAgent { get; set; } = string.Empty;

        public bool IsActive => !IsRevoked && ExpiresAt > DateTime.UtcNow;
    }
}
