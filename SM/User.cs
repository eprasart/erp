﻿using System;
//using System.Windows.Forms;
using System.Collections.Generic;
using ServiceStack.OrmLite;
using ServiceStack.DataAnnotations;
using System.Linq;
using System.Data;
using Npgsql;

namespace ERP
{
    [Alias("SmUser")]
    class User
    {
        [AutoIncrement]
        public long Id { get; set; }
        [Required]
        public string Username { get; set; }
        public string FullName { get; set; }
        public string Pwd { get; set; }
        public DateTime PwdChangeOn { get; set; }
        public string PwdChangeForce { get; set; }
        public int TimeLevel { get; set; }        
        public string StartOn { get; set; }
        public string EndOn { get; set; }
        public int Success { get; set; }
        public int Fail { get; set; }
        public string Locked { get; set; }
        public string Right { get; set; }        
        public string SecurityNo { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
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

    static class UserFacade
    {
        public static List<User> Select(string filter = "")
        {
            SqlExpression<User> e = OrmLiteConfig.DialectProvider.SqlExpression<User>();
            e.Where(q => q.Status == StatusType.Active && (q.Username.Contains(filter) || q.FullName.Contains(filter)))
                .OrderBy(q => q.Username);
            //System.Windows.Forms.MessageBox.Show(e.SelectExpression + "\n" + e.WhereExpression);           
            return Database.Connection.Select<User>(e);
        }

        public static DataTable GetDataTable(string filter = "", string status = "")
        {
            var sql = "select id, username, full_name, note from sm_user where 1 = 1";
            if (status.Length > 0)
                sql += " and status = '" + status + "'";
            if (filter.Length > 0)
                sql += " and (username ~* :filter or full_name ~* :filter or note ~* :filter)";
            sql += "\norder by username";
            var cmd = new NpgsqlCommand(sql, new NpgsqlConnection(Database.ConnectionString));
            if (filter.Length > 0)
                cmd.Parameters.AddWithValue(":filter", filter);
            var da = new NpgsqlDataAdapter(cmd);
            var dt = new DataTable();
            da.Fill(dt);
            return dt;
        }

        public static long Save(User m)
        {
            DateTime? ts = Database.GetCurrentTimeStamp();
            long seq = 0;   // New inserted sequence
            var mSave = new User
            {
                Username = m.Username,
                FullName = m.FullName,
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
                Database.Connection.UpdateOnly(mSave, p => new { p.Username, p.FullName, p.Note, ChangeBy = p.ChangeBy, ChangeAt = p.ChangeAt },
                    p => p.Id == m.Id);
                // If record is lock then unlock
                if (IsLocked(m.Id)) ReleaseLock(m.Id);
            }
            return seq;
        }

        public static User Select(long Id)
        {
            return Database.Connection.SingleById<User>(Id);
        }

        public static void SetStatus(long Id, string s)
        {
            DateTime? ts = Database.GetCurrentTimeStamp();
            Database.Connection.UpdateOnly(new User { Status = s, ChangeBy = Login.Username, ChangeAt = ts }, p => new { p.Status, p.ChangeBy, p.ChangeAt }, p => p.Id == Id);
        }

        public static bool IsLocked(long Id)
        {
            return Database.Connection.Exists<User>("Id = @Id and Lock_By = @LockBy", new { Id = Id, LockBy = Login.Username });
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
            Database.Connection.UpdateOnly(new User { LockBy = Login.Username, LockAt = ts }, p => new { p.LockBy, p.LockAt }, p => p.Id == Id);
        }

        public static void ReleaseLock(long Id)
        {
            if (Id == 0) return;
            DateTime ts = Database.GetCurrentTimeStamp();
            Database.Connection.UpdateOnly(new User { LockBy = null }, p => p.LockBy, p => p.Id == Id);
        }

        public static bool IsExist(string Username, long Id = 0)
        {
            return Database.Connection.Exists<User>("Id <> @Id and Username = @Username", new { Id = Id, Username = Username });
        }
    }
}