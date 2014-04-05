using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ERP
{
    public partial class frmMsg : Form
    {
        public frmMsg()
        {
            InitializeComponent();
        }

        public frmMsg(string msg)
            : this()
        {
            txtMsg.Text = msg;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            DialogResult = System.Windows.Forms.DialogResult.OK;
        }

    }
}
