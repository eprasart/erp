using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP
{
    class LockInfo
    {
        public int Id { get; set; }
        public string LockBy { get; set; }
        public DateTime? LockAt { get; set; }
        public bool IsLocked
        {
            get { return LockBy != null || LockBy.Length == 0; }
        }
    }
}
