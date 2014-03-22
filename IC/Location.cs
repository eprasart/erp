using System;
//using System.Windows.Forms;
using System.Collections.Generic;
using ServiceStack.OrmLite;
using ServiceStack.DataAnnotations;
using System.Linq;


namespace ERP
{
    [Alias("IcLocation")]
    class Location
    {
        [AutoIncrement]
        public int Id { get; set; }
        public int CompanyId { get; set; }
        [Required]
        public string Code { get; set; }
        public string Desc1 { get; set; }
        public string Desc2 { get; set; }
        public string Address { get; set; }
        [StringLength(2000)]
        public string Note { get; set; }
        [StringLength(1)]
        //[Default(typeof(string), "A")]
        public String Status { get; set; }
        public string LockBy { get; set; }
        public DateTime? LockAt { get; set; }
        public string InsertBy { get; set; }
        [Default(typeof(DateTime), "CURRENT_TIMESTAMP")]
        public DateTime? InsertAt { get; set; }
        public string ChangeBy { get; set; }
        public DateTime? ChangeAt { get; set; }
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
            var mSave = new Location
            {
                Code = m.Code,
                Desc1 = m.Desc1,
                Desc2 = m.Desc2,
                Address = m.Address,
                Note = m.Note,
            };
            if (m.Id == 0)
            {
                mSave.Status = StatusType.Active;
                mSave.InsertBy = Login.Username;
                mSave.InsertAt = ts;
                Database.Connection.Insert(mSave);
                //Database.Connection.Update<Location>( set: "Insert_At = {0}".Params("CURRENT_TIMESTAMP"), where: "Id = {0}".Params(Id));
                //Database.Connection.InsertOnly(mSave, ev => ev.Insert(p => new { p.Code, p.Desc1, p.Desc2, p.Address, p.Note, p.Status, p.Insert_By, p.Insert_At }));
            }
            else
            {
                mSave.ChangeBy = Login.Username;
                mSave.ChangeAt = ts;
                Database.Connection.UpdateOnly(mSave, p => new { p.Code, p.Desc1, p.Desc2, p.Address, p.Note, Change_By = p.ChangeBy, Change_At = p.ChangeAt },
                    p => p.Id == m.Id);
                //Database.Connection.Delete<Location>(where: "Age = {0}".Params(27));
            }
            //System.Windows.Forms.MessageBox.Show(Database.Connection.GetLastSql());
        }

        public static Location Select(int Id)
        {
            return Database.Connection.SingleById<Location>(Id);
        }

        public static void SetStatus(int Id, string s)
        {
            Database.Connection.UpdateOnly(new Location { Status = s }, p => p.Status, p => p.Id == Id);
        }
    }
}
