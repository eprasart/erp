using System;
//using System.Windows.Forms;
using System.Collections.Generic;
using ServiceStack.OrmLite;
using ServiceStack.DataAnnotations;

namespace ERP
{
    [Alias("SM_Company")]
    class Company
    {
        [AutoIncrement] // ??
        public int Id { get; set; }
    }
}
