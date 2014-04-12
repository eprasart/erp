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
        public String Status { get; set; } 
        public string Insert_By { get; set; }
        public DateTime Insert_At { get; set; }
        public string Lock_By { get; set; }
        public DateTime Lock_At { get; set; }
        public string Change_By { get; set; }
        public DateTime Change_At { get; set; }
    }

    static class CategoryFacade
    {
        public static List<Location> Select(string filter = "")
        {
            SqlExpression<Location> e = OrmLiteConfig.DialectProvider.SqlExpression<Location>();
            e.Where(q => q.Status == StatusType.Active && (q.Code.Contains(filter) || q.Desc1.Contains(filter) || q.Desc2.Contains(filter)))
                .OrderBy(q => q.Code);
            return Database.Connection.Select<Location>(e);
        }

        public static void Save(Location m)
        {
            if (m.Id == 0)
            {
                Database.Connection.Insert(m);
                //Database.Connection.Insert(new Location { Code = m.Code, Status = m.Status, Insert_By = Login.Username });
            }
            else
                Database.Connection.Update(m);
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
