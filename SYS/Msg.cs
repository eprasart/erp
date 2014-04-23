using System;
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

        public string Title
        {
            set { Text = value; }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            //Clipboard.SetText(txtMsg.Rtf);
            DialogResult = DialogResult.OK;
        }

    }
}
