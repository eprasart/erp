using System;
//using System.Windows.Forms;
using System.Collections.Generic;
using ServiceStack.OrmLite;
using ServiceStack.DataAnnotations;


namespace ERP
{
    [Alias("IC_Category")]
    class Category
    {
        [AutoIncrement]
        public int Id { get; set; }
        public int Company_Id { get; set; }
        [Required]
        public string Code { get; set; }
        public string Desc1 { get; set; }
        public string Desc2 { get; set; }
        [StringLength(4000)]
        public string Note { get; set; }
        [StringLength(1)]
        public String Status { get; set; }  //TODO: try char(1) again
        public string Insert_By { get; set; }
        public DateTime Insert_At { get; set; }
        public string Lock_By { get; set; }
        public DateTime Lock_At { get; set; }
        public string Change_By { get; set; }
        public DateTime Change_At { get; set; }
    }

    static class LocationFacade
    {
        public static List<Location> Select(string filter = "")
        {
            SqlExpression<Location> e = OrmLiteConfig.DialectProvider.SqlExpression<Location>();
            e.Where(q => q.Status == StatusType.Active && (q.Code.Contains(filter) || q.Desc1.Contains(filter) || q.Desc2.Contains(filter)))
                .OrderBy(q => q.Code);
            return App.db.Select<Location>(e);
        }

        public static void Save(Location m)
        {
            if (m.Id == 0)
            {
                App.db.Insert(m);
                //App.db.Insert(new Location { Code = m.Code, Status = m.Status, Insert_By = Login.Username });
            }
            else
                App.db.Update(m);
            System.Windows.Forms.MessageBox.Show(App.db.GetLastSql());

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
