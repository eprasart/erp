﻿using System;
//using System.Windows.Forms;
using System.Collections.Generic;
using ServiceStack.OrmLite;
using ServiceStack.DataAnnotations;


namespace ERP
{
    [Alias("PO_Vendor")]
    class Vendor
    {
        [PrimaryKey]
        [AutoIncrement]
        public long Id { get; set; }
        public string Name_EN { get; set; }
        public string Name_KH { get; set; }
        //[Default(typeof(string), "A")]
        [StringLength(1)]        
        public String Status { get; set; } 
    }

    static class VendorFacade
    {
        public static List<Vendor> Select(string filter = "")
        {
            SqlExpression<Vendor> e = OrmLiteConfig.DialectProvider.SqlExpression<Vendor>();
            e.Where(q => q.Status == StatusType.Active && (q.Name_EN.Contains(filter) || q.Name_KH.Contains(filter))).OrderBy(q => q.Name_EN);
            return Database.Connection.Select<Vendor>(e);
            //MessageBox.Show(Database.Connection.GetLastSql());
        }

        public static void Save(Vendor v)
        {
            if (v.Id == 0)
                Database.Connection.Insert(v);
            else
            {
                
                Database.Connection.Update(v);
            }
        }

        public static Vendor Select(long Id)
        {
            return Database.Connection.SingleById<Vendor>(Id);
        }

        public static void Delete(long Id)
        {
            Database.Connection.UpdateOnly(new Vendor { Status = StatusType.Deleted }, p => p.Status, p => p.Id == Id);
        }
    }
}
