using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP
{
    class Common
    {
        public static string RemoveLastDotZero(string v)
        {
            string s = v;
            if (v.EndsWith(".0"))
                return RemoveLastDotZero(v.Substring(0, v.Length - 2));
            else
                return s;
        }

        public static void ShowMsg(string msg, string title)
        {
            var fMsg = new frmMsg(msg);
            fMsg.Title = title;
            fMsg.ShowDialog();
        }
    }


}
