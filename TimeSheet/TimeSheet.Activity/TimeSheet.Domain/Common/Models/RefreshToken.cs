using System;
using System.Collections.Generic;
using TimeSheet.Domain.Entities;

namespace TimeSheet.Domain.Common.Models
{
    public class RefreshToken
    {
        public Guid id { get; set; }
        public string TokenString { get; set; }
        public DateTime ExpiryDate { get; set; }
        public bool IsRevoked { get; set; }

        // Relationships

        public Guid MemberId { get; set; }
        public virtual Member Member{ get; set; }

        // Contructors

        public RefreshToken() { }
        public RefreshToken(string tokenString, DateTime expiresAt, bool isRevoked)
        {
            TokenString = tokenString;
            ExpiryDate = expiresAt;
            IsRevoked = isRevoked;
        }
    }
}