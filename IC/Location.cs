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
        [AutoIncrement]
        public int Id { get; set; }
        public int Company_Id { get; set; }
        [Required]
        public string Code { get; set; }
        public string Desc1 { get; set; }
        public string Desc2 { get; set; }
        public string Address { get; set; }
        [StringLength(1)]
        public String Status { get; set; }
        [StringLength(2000)]
        public string Note { get; set; }
        public string Insert_By { get; set; }
        public DateTime? Insert_At { get; set; }
        public string Lock_By { get; set; }
        public DateTime? Lock_At { get; set; }
        public string Change_By { get; set; }
        public DateTime? Change_At { get; set; }
    }

    static class LocationFacade
    {
        public static List<Location> Select(string filter = "")
        {
            SqlExpression<Location> e = OrmLiteConfig.DialectProvider.SqlExpression<Location>();
            e.Where(q => q.Status == StatusType.Active && (q.Code.Contains(filter) || q.Desc1.Contains(filter) || q.Desc2.Contains(filter)))
                .OrderBy(q => q.Code);
            //System.Windows.Forms.MessageBox.Show(e.SelectExpression + "\n" + e.WhereExpression);
            return Database.Connection.Select<Location>(e);

            //);
        }

        public static void Save(Location m)
        {
            DateTime? ts = Database.GetCurrentTimeStamp();
            if (m.Id == 0)
                Database.Connection.Insert(m);
            else
            {
                //Database.Connection.Update(m);
                var mUpdate = new Location
                {
                    Code = m.Code,
                    Desc1 = m.Desc1,
                    Desc2 = m.Desc2,
                    Address = m.Address,
                    Note = m.Note,
                    Change_By = m.Change_By,
                    Change_At = m.Change_At
                };                
                Database.Connection.UpdateOnly(mUpdate, obj => new { obj.Code, obj.Desc1, obj.Desc2, obj.Address, obj.Note, obj.Change_By, obj.Change_At },
                    obj => obj.Id == m.Id);
            }
            System.Windows.Forms.MessageBox.Show(Database.Connection.GetLastSql());
        }

        public static Location Select(int Id)
        {
            return Database.Connection.SingleById<Location>(Id);
        }

        public static void Delete(int Id)
        {
            Database.Connection.UpdateOnly(new Location { Status = StatusType.Deleted }, p => p.Status, p => p.Id == Id);            
        }
    }
}
