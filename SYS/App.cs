/*TODO:
 * Option to auto switch keyboard to KH when in Unicode box
 * Log in db and file
 */
using System;
using System.IO;
using System.Windows.Forms;
using System.Data;
using ServiceStack.OrmLite;
using System.Reflection;

namespace ERP
{
    public static class App
    {
        public static string ConnectionString = ""; //Data Source=localhost:1521/xe;User ID=erp;Password=erp;Unicode=True";
        public static OrmLiteConnectionFactory dbFactory = null;
        public static IDbConnection db = null;

        public static Setting setting = new Setting();
        public static String version;

        public static void InitApp()
        {
            LoadSettings();
            try
            {                             
                dbFactory = new OrmLiteConnectionFactory(ConnectionString, OracleDialect.Provider);
                db = dbFactory.OpenDbConnection();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                Application.Exit();
                return;
            }
            PrepareDatabase();
            SetVersion();

            Login.Company_Id = 1;
            Login.Username = "Visal";
        }

        private static void SetVersion()
        {
            version = Common.RemoveLastDotZero( Assembly.GetEntryAssembly().GetName().Version.ToString());
        }

        public static void PrepareDatabase()
        {
            db.CreateTableIfNotExists(typeof(Category), typeof(Vendor), typeof(Location));
            //db.CreateTableIfNotExists<Vendor>();
            //db.CreateTableIfNotExists<Location>();
        }

        public static void LoadSettings()
        {
            setting.Path = Path.Combine(Application.StartupPath, "setting.ini");
            ConnectionString = setting.Get("ConnectionString", @"Data Source=localhost:1521/xe;User ID=erp;Password=erp;Unicode=True");
        }

        public static void SaveSettings()
        {
            setting.Save();
        }
    }
}
