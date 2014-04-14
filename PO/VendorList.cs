using System;
using System.Windows.Forms;

namespace ERP
{
    public partial class frmVendorList : Form
    {
        private long Id = 0;
        private bool bExpand = false;

        public frmVendorList()
        {
            InitializeComponent();
        }

        private void RefreshGrid()
        {
            dgvList.DataSource = VendorFacade.Select(txtSearch.Text);
        }

        private void frmVendor_Load(object sender, EventArgs e)
        {
            Icon = Properties.Resources.Icon;             
            RefreshGrid();            
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            txtNameEN.Text = "";
            txtNameEN.Focus();
            txtNameKH.Text = "";
            Id = 0;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {            
            var v = new Vendor();
            v.Id = Id;
            v.Name_EN = txtNameEN.Text;
            v.Name_KH = txtNameKH.Text;
            v.Status = StatusType.Active;
            VendorFacade.Save(v);
            RefreshGrid();
        }

        private void txtSearch_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                RefreshGrid();
            }
        }

        private void lblClear_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            txtSearch.Text = "";
            RefreshGrid();
        }

        private void lblRefresh_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Cursor = Cursors.WaitCursor;
            RefreshGrid();
            Cursor = Cursors.Default;
        }

        private void dgvVendor_SelectionChanged(object sender, EventArgs e)
        {
            if (Id == dgvList.Id)
                return;
            else
                Id = dgvList.Id;
            if (Id == 0) return;
            var v = VendorFacade.Select(Id);
            txtNameEN.Text = v.Name_EN;
            txtNameKH.Text = v.Name_KH;
        }

        private void btnSaveNew_Click(object sender, EventArgs e)
        {
            btnSave_Click(sender, e);
            btnNew_Click(sender, e);
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            var Id = dgvList.Id;
            if (Id == 0) return;
            VendorFacade.Delete(Id);
            RefreshGrid();
            if (dgvList.RowCount == 0) btnNew_Click(sender, e);
        }

        private void btnCopy_Click(object sender, EventArgs e)
        {
            Id = 0;
            txtNameEN.Focus();
        }

        private void imgExpand_Click(object sender, EventArgs e)
        {
            splitContainer1.IsSplitterFixed = !bExpand;
            if (!bExpand)
            {
                splitContainer1.SplitterDistance = splitContainer1.Size.Width;
                splitContainer1.FixedPanel = FixedPanel.Panel2;
                imgExpand.Image = Properties.Resources.Back;
            }
            else
            {
                splitContainer1.SplitterDistance = 300;
                splitContainer1.FixedPanel = FixedPanel.Panel1;
                imgExpand.Image = Properties.Resources.Next;
            }
            bExpand = !bExpand;
        }
    }
}
