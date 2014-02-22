using System;
//using System.Windows.Forms;
using System.Collections.Generic;
using ServiceStack.OrmLite;
using ServiceStack.DataAnnotations;


namespace ERP
{
    [Alias("IC_Location")]
    class Location
    {
        [PrimaryKey]
        [AutoIncrement]
        public int Id { get; set; }
        [Required]
        public string Code { get; set; }
        public string Desc_EN { get; set; }
        public string Desc_KH { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public string Fax { get; set; }
        [StringLength(1)]
        public string Type { get; set; }
        [StringLength(2000)]
        public string Note { get; set; }
        [StringLength(1)]
        public String Status { get; set; }  //TODO: try char(1) again
        public string Insert_By { get; set; }
        public DateTime? Insert_At { get; set; }
        public string Update_By { get; set; }
        public DateTime? Update_At { get; set; }
        //[Default(typeof(string), "A")]        
    }

    static class LocationFacade
    {
        public static List<Location> Select(string filter = "")
        {
            SqlExpression<Location> e = OrmLiteConfig.DialectProvider.SqlExpression<Location>();
            e.Where(q => q.Status == StatusType.Active && (q.Code.Contains(filter) || q.Desc_EN.Contains(filter) || q.Desc_KH.Contains(filter)))
                .OrderBy(q => q.Code);
            System.Windows.Forms.MessageBox.Show(e.tosql + "\n" + e.WhereExpression);
            return App.db.Select<Location>(e);
            
            //);
            
        }

        public static void Save(Location v)
        {
            if (v.Id == 0)
                App.db.Insert(v);
            else
                App.db.Update(v);
        }

        public static Location Select(int Id)
        {
            return App.db.SingleById<Location>(Id);
        }

        public static void Delete(int Id)
        {
            App.db.UpdateOnly(new Location { Status = StatusType.Deleted }, p => p.Status, p => p.Id == Id);
            //todo: status code in enum
        }
    }
}
