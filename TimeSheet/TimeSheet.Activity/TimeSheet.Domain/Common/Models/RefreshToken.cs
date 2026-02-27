using System;
using System.Collections.Generic;

namespace TimeSheet.Domain.Common.Models
{
    public class RefreshToken
    {
        public Guid id { get; set; }
        public string RefreshToken { get; set; }
        public DateTime ExpiryDate { get; set; }
        public bool IsRevoced { get; set; }

        // Relationships

        public Guid MemberId { get; set; }
        public virtual Member Member{ get; set; }

        // Contructors

        public RefreshToken() { }
        public RefreshToken(string refreshToken, DateTime expiresAt, bool isRevoced)
        {
            RefreshToken = refreshToken;
            ExpiryDate = expiresAt;
            IsRevoced = isRevoced;
        }
    }
}