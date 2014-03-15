using ServiceStack.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP
{
    class Basic
    {
        [AutoIncrement]
        public int Id { get; set; }
        public String Status { get; set; }
        [StringLength(1)]
        public string Lock_By { get; set; }
        public DateTime? Lock_At { get; set; }
        public string Insert_By { get; set; }
        [Default(typeof(DateTime), "CURRENT_TIMESTAMP")]
        public DateTime? Insert_At { get; set; }
        public string Change_By { get; set; }
        public DateTime? Change_At { get; set; }
    }
}
