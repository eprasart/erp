using System;
//using System.Windows.Forms;
using System.Collections.Generic;
using ServiceStack.OrmLite;
using ServiceStack.DataAnnotations;
using System.Linq;
using System.Data;
using Npgsql;

namespace ERP
{
    [Alias("IcLocation")]
    class Location
    {
        [AutoIncrement]
        public long Id { get; set; }
        public int CompanyId { get; set; }
        [Required]
        public string Code { get; set; }
        public string Desc1 { get; set; }
        public string Desc2 { get; set; }
        public string Address { get; set; }
        public string Note { get; set; }
        public String Status { get; set; }
        public string LockBy { get; set; }
        public DateTime? LockAt { get; set; }
        public string InsertBy { get; set; }
        [Default(typeof(DateTime), "now()")]
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
        }

        public static DataTable GetDataTable(string filter = "", string status = "")
        {
            var sql = "select id, code, desc1, desc2, address from ic_location where 1 = 1";
            if (status.Length > 0)
                sql += " and status = '" + status + "'";
            if (filter.Length > 0)
                sql += " and (code ~* :filter or desc1 ~* :filter or desc2 ~* :filter or address ~* :filter or note ~* :filter)";
            sql += "\norder by code";
            var cmd = new NpgsqlCommand(sql, new NpgsqlConnection(Database.ConnectionString));
            if (filter.Length > 0)
                cmd.Parameters.AddWithValue(":filter", filter);
            var da = new NpgsqlDataAdapter(cmd);
            var dt = new DataTable();
            da.Fill(dt);
            return dt;
        }

        public static long Save(Location m)
        {
            DateTime? ts = Database.GetCurrentTimeStamp();
            long seq = 0;   // New inserted sequence
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
                seq = Database.Connection.Insert(mSave, true);
            }
            else
            {
                mSave.ChangeBy = Login.Username;
                mSave.ChangeAt = ts;
                Database.Connection.UpdateOnly(mSave, p => new { p.Code, p.Desc1, p.Desc2, p.Address, p.Note, ChangeBy = p.ChangeBy, ChangeAt = p.ChangeAt },
                    p => p.Id == m.Id);
                // If record is lock then unlock
                if (IsLocked(m.Id)) ReleaseLock(m.Id);
            }
            return seq;
        }

        public static Location Select(long Id)
        {
            return Database.Connection.SingleById<Location>(Id);
        }

        public static void SetStatus(long Id, string s)
        {
            DateTime? ts = Database.GetCurrentTimeStamp();
            Database.Connection.UpdateOnly(new Location { Status = s, ChangeBy = Login.Username, ChangeAt = ts }, p => new { p.Status, p.ChangeBy, p.ChangeAt }, p => p.Id == Id);
        }

        public static bool IsLocked(long Id)
        {
            return Database.Connection.Exists<Location>("Id = @Id and Lock_By = @LockBy", new { Id = Id, LockBy = Login.Username });
        }

        public static LockInfo GetLockInfo(long Id)
        {
            var m = Select(Id);
            var l = new LockInfo();
            l.Id = Id;
            l.LockBy = m.LockBy;
            l.LockAt = m.LockAt;
            return l;
        }

        public static void Lock(long Id)
        {
            DateTime ts = Database.GetCurrentTimeStamp();
            Database.Connection.UpdateOnly(new Location { LockBy = Login.Username, LockAt = ts }, p => new { p.LockBy, p.LockAt }, p => p.Id == Id);
        }

        public static void ReleaseLock(long Id)
        {
            if (Id == 0) return;
            DateTime ts = Database.GetCurrentTimeStamp();
            Database.Connection.UpdateOnly(new Location { LockBy = null }, p => p.LockBy, p => p.Id == Id);
        }

        public static bool IsExist(string Code, long Id = 0)
        {
            return Database.Connection.Exists<Location>("Id <> @Id and Code = @Code", new { Id = Id, Code = Code });
        }
    }
}