using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TimeSheet.Domain.Interfaces;

namespace TimeSheet.Domain.Interfaces
{
    public interface ISoftDelete
    {
        public bool IsDeleted { get; set; }
    
        public void Undo()
        {
            IsDeleted = false;
        }
    }
}