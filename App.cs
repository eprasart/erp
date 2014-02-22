/*TODO:
 * Option to auto switch keyboard to KH when in Unicode box
 * Log in db and file
 */
using System;
using System.Data;
using ServiceStack.OrmLite;

namespace ERP
{
    public static class App
    {
        public static string ConnectionString = "Data Source=localhost:1521/xe;User ID=erp;Password=erp;Unicode=True";

        public static OrmLiteConnectionFactory dbFactory = new OrmLiteConnectionFactory(ConnectionString, OracleDialect.Provider);

        public static IDbConnection db = dbFactory.OpenDbConnection();

        public static  void PrepareDatabase()
        {
            db.CreateTableIfNotExists<Vendor>();
            db.CreateTableIfNotExists<Location>();
        }
    }
}
