﻿using System;
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

        public string Title
        {
            set { Text = value; }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(txtMsg.Rtf);
            DialogResult = System.Windows.Forms.DialogResult.OK;
        }

    }
}
