using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP
{
    class Lock
    {
        public int Id { get; set; }
        public string LockBy { get; set; }
        public string LockAt { get; set; }
        public bool IsLocked
        {
            get { return LockBy.Length > 0; }
        }
    }
}
