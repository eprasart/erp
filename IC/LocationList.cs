/*TODO: 
 * msg => English and/or Khmer (use both font in rtf to make it render nice)
 * spliterDistance: save in table by user
 * datagridview sort by column header
 * show/search inactive
*/
using System;
using System.Windows.Forms;

namespace ERP
{
    public partial class frmLocationList : Form
    {
        private long Id = 0;
        private int rowIndex = 0;
        private bool isExpand = false;
        private bool isDirty = false;

        public frmLocationList()
        {
            InitializeComponent();
        }

        private void RefreshGrid(long seq = 0)
        {
            if (dgvList.RowCount > 0) rowIndex = dgvList.CurrentRow.Index;
            dgvList.DataSource = LocationFacade.Select(txtSearch.Text);
            if (dgvList.RowCount > 0)
                if (seq == 0)
                {
                    if (rowIndex >= dgvList.RowCount) rowIndex = dgvList.RowCount - 1;
                    dgvList.CurrentCell = dgvList[1, rowIndex];
                }
                else
                    foreach (DataGridViewRow row in dgvList.Rows)
                        if ((long)row.Cells[0].Value == seq)
                        {
                            Id = (int)seq;
                            dgvList.CurrentCell = dgvList[1, row.Index];
                            break;
                        }
            LoadData();
        }

        private void LockControls(bool l = true)
        {
            foreach (Control c in splitContainer1.Panel2.Controls)
            {
                if (c is TextBox)
                    ((TextBox)c).ReadOnly = l;
                else if (c is ComboBox)
                    ((ComboBox)c).Enabled = !l;
            }
            btnNew.Enabled = l;
            btnCopy.Enabled = l;
            //btnUnlock.Enabled = !l;
            btnSave.Enabled = !l;
            btnSaveNew.Enabled = !l;
            btnActive.Enabled = l;
            btnDelete.Enabled = l;
            splitContainer1.Panel1.Enabled = l;
            btnUnlock.Text = l ? "Un&lock" : "Cance&l";
            btnUnlock.ToolTipText = btnUnlock.Text + " (Ctrl+C)";
        }

        private bool IsValidated()
        {
            if (txtCode.Text.Trim().Length == 0)
            {
                Common.ShowMsg("Code cannot be empty.", "Save");
                txtCode.Focus();
                return false;
            }
            return true;
        }

        private void LoadData()
        {
            var Id = dgvList.Id;
            if (Id == 0) return;
            var m = LocationFacade.Select(Id);
            txtCode.Text = m.Code;
            txtDescEN.Text = m.Desc1;
            txtDescKH.Text = m.Desc2;
            txtAddress.Text = m.Address;
            txtNote.Text = m.Note;

            LockControls();
        }

        private void frmLocationList_Load(object sender, EventArgs e)
        {
            App.InitApp();

            Icon = Properties.Resources.Icon;
            dgvList.ShowLessColumns(true);
            RefreshGrid();

            Text += " v. " + App.version;

            LockControls();
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            if (isExpand) picExpand_Click(sender, e);
            txtCode.Text = "";
            txtCode.Focus();
            txtDescEN.Text = "";
            txtDescKH.Text = "";
            txtPhone.Text = "";
            txtFax.Text = "";
            txtAddress.Text = "";
            txtNote.Text = "";
            if (dgvList.RowCount > 0)
                dgvList.CurrentRow.Selected = false;
            Id = 0;
            LockControls(false);
            if (dgvList.RowCount > 0) rowIndex = dgvList.CurrentRow.Index;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (!IsValidated()) return;
            Cursor = Cursors.WaitCursor;
            var m = new Location();
            m.Id = Id;
            m.Code = txtCode.Text;
            m.Desc1 = txtDescEN.Text;
            m.Desc2 = txtDescKH.Text;
            m.Address = txtAddress.Text;
            m.Note = txtNote.Text;
            long seq = LocationFacade.Save(m);
            if (dgvList.RowCount > 0) rowIndex = dgvList.CurrentRow.Index;
            RefreshGrid(seq);
            LockControls();
            Cursor = Cursors.Default;
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

        private void dgvList_SelectionChanged(object sender, EventArgs e)
        {
            LoadData();
        }

        private void btnSaveNew_Click(object sender, EventArgs e)
        {
            btnSave_Click(sender, e);
            btnNew_Click(sender, e);
            //todo: code not clear
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            var Id = dgvList.Id;
            if (Id == 0) return;
            //todo: check if locked
            if (MessageBox.Show("Are you sure you want to delete?", "Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == System.Windows.Forms.DialogResult.No) return;
            LocationFacade.SetStatus(Id, StatusType.Deleted);
            RefreshGrid();
            if (dgvList.RowCount == 0) btnNew_Click(sender, e);
        }

        private void btnCopy_Click(object sender, EventArgs e)
        {
            Id = 0;
            txtCode.Focus();
            LockControls(false);
        }

        private void picExpand_Click(object sender, EventArgs e)
        {
            splitContainer1.IsSplitterFixed = !isExpand;
            if (!isExpand)
            {
                splitContainer1.SplitterDistance = splitContainer1.Size.Width;
                splitContainer1.FixedPanel = FixedPanel.Panel2;
                imgExpand.Image = Properties.Resources.Back;
            }
            else
            {
                splitContainer1.SplitterDistance = 300; // TODO: load from var or db
                splitContainer1.FixedPanel = FixedPanel.Panel1;
                imgExpand.Image = Properties.Resources.Next;
            }
            dgvList.ShowLessColumns(isExpand);
            isExpand = !isExpand;
        }

        private void dgvList_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (isExpand) picExpand_Click(sender, e);
            dgvList_SelectionChanged(sender, e);    // reload data since SelectionChanged will not occured on current row
        }

        private void frmLocationList_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (btnUnlock.Text == "Cance&l")
                btnUnlock_Click(null, null);
            App.SaveSettings(); // Move to main form instead
        }

        private void btnActive_Click(object sender, EventArgs e)
        {
            var Id = dgvList.Id;
            if (Id == 0) return;
            var lInfo = LocationFacade.GetLockInfo(Id);
            if (lInfo.IsLocked)
            {
                string msg = "Account is currently locked by '" + lInfo.LockBy + "' since '" + lInfo.LockAt + "'";
                new frmMsg(msg).ShowDialog();
                return;
            }
            //todo: check if locked
            //todo: if A else I
            LocationFacade.SetStatus(Id, StatusType.InActive);
            RefreshGrid();
        }

        private void btnUnlock_Click(object sender, EventArgs e)
        {
            Id = dgvList.Id;
            // Cancel
            if (btnUnlock.Text == "Cance&l")
            {
                if (isDirty)
                {
                    //todo: reload orginal data (if dirty)

                }
                LockControls(true);
                //dgvList.CurrentCell = dgvList[1, rowIndex];
                LocationFacade.ReleaseLock(dgvList.Id);
                if (dgvList.RowCount>0 && !dgvList.CurrentRow.Selected) 
                    dgvList.CurrentRow.Selected = true;
                return;
            }

            // Unlock
            if (Id == 0) return;
            var lInfo = LocationFacade.GetLockInfo(Id);

            if (lInfo.IsLocked) //if (LocationFacade.IsLocked(dgvList.Id))    // Check if record is locked
            {
                string msg = "Account is currently locked by '" + lInfo.LockBy + "' since '" + lInfo.LockAt + "'";
                new frmMsg(msg).ShowDialog();
                return;
            }
            LockControls(false);
            LocationFacade.Lock(dgvList.Id);
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            switch (keyData)
            {
                case Keys.Control | Keys.N:
                    if (btnNew.Enabled) btnNew_Click(null, null);
                    break;
                case Keys.Control | Keys.Y:
                    if (btnCopy.Enabled) btnCopy_Click(null, null);
                    break;
                case Keys.Control | Keys.L:
                    if (btnUnlock.Enabled) btnUnlock_Click(null, null);
                    break;
                case Keys.Control | Keys.S:
                    if (btnSave.Enabled) btnSave_Click(null, null);
                    break;
                case Keys.Control | Keys.W:
                    if (btnSaveNew.Enabled) btnSaveNew_Click(null, null);
                    break;
                case Keys.Control | Keys.E:
                    if (btnActive.Enabled) btnActive_Click(null, null);
                    break;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void dgvList_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                e.SuppressKeyPress = true;
                if (btnDelete.Enabled) btnDelete_Click(null, null);
            }
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            lblClear.Enabled = (txtSearch.Text.Length > 0);
        }
    }
}
