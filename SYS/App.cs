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


        public static Setting setting = new Setting();
        public static String version;

        public static void InitApp()
        {
            LoadSettings();
            try
            {
                Database.Factory = new OrmLiteConnectionFactory(Database.ConnectionString, PostgreSqlDialect.Provider);
                Database.Connection = Database.Factory.OpenDbConnection();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                Application.Exit();
                return;
            }
            Database.PrepareDatabase();
            SetVersion();

            Login.Company_Id = 1;
            Login.Username = "Visal";
        }

        private static void SetVersion()
        {
            version = Common.RemoveLastDotZero(Assembly.GetEntryAssembly().GetName().Version.ToString());
        }

        public static void LoadSettings()
        {
            setting.Path = Path.Combine(Application.StartupPath, "setting.ini");
            Database.ConnectionString = setting.Get("ConnectionString", @"server=localhost;database=erp;uid=erp;pwd=erp");
        }

        public static void SaveSettings()
        {
            setting.Save();
        }
    }
}
