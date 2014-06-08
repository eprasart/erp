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
    public partial class frmUserList : Form
    {
        private long Id = 0;
        private int rowIndex = 0;
        private bool isExpand = false;
        private bool isDirty = false;

        public frmUserList()
        {
            InitializeComponent();
        }

        private string GetStatus()
        {
            var status = "";
            if (mnuShowA.Checked && !mnuShowI.Checked)
                status = "A";
            else if (mnuShowI.Checked && !mnuShowA.Checked)
                status = "I";
            return status;
        }

        private void RefreshGrid(long seq = 0)
        {
            if (dgvList.RowCount > 0) rowIndex = dgvList.CurrentRow.Index;

            dgvList.DataSource = UserFacade.GetDataTable(txtSearch.Text, GetStatus());
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

        private void SetStatus(string stat)
        {
            if (stat == "A")
            {
                btnActive.Text = "Inactiv&e";
                if (btnActive.Image != Properties.Resources.Inactive)
                    btnActive.Image = Properties.Resources.Inactive;
            }
            else
            {
                btnActive.Text = "Activ&e";
                if (btnActive.Image != Properties.Resources.Active)
                    btnActive.Image = Properties.Resources.Active;
            }
        }

        private bool IsValidated()
        {
            string Code = txtUsernane.Text.Trim();
            if (Code.Length == 0)
            {
                Common.ShowMsg("Code cannot be empty.", "Save");
                txtUsernane.Focus();
                return false;
            }
            if (UserFacade.IsExist(Code, Id))
            {
                Common.ShowMsg("Code already exists. Code must be unique.", "Save");
                txtUsernane.Focus();
                txtUsernane.SelectAll();
                return false;
            }
            //todo: prevent duplicate
            return true;
        }

        private void LoadData()
        {
            var Id = dgvList.Id;
            if (Id == 0) return;
            var m = UserFacade.Select(Id);
            txtUsernane.Text = m.Username;
            txtFullName.Text = m.FullName;
            txtPhone.Text = m.Phone;
            txtEmail.Text = m.Email;
            txtPwd.Text = m.Pwd;
            dtpStart.Checked = (m.StartOn != null);
            if (dtpStart.Checked) dtpStart.Value = (DateTime)m.StartOn;
            dtpEnd.Checked = (m.EndOn != null);
            if (dtpEnd.Checked) dtpEnd.Value = (DateTime)m.EndOn;
            txtNote.Text = m.Note;
            SetStatus(m.Status);
            LockControls();
        }

        private void frmUserList_Load(object sender, EventArgs e)
        {
            App.Init();

            Icon = Properties.Resources.Icon;
            dgvList.ShowLessColumns(true);
            RefreshGrid();

            Text += " v. " + App.version;

            LockControls();
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            if (isExpand) picExpand_Click(sender, e);
            txtUsernane.Text = "";
            txtUsernane.Focus();
            txtFullName.Text = "";
            txtPwd.Text = "";
            txtPhone.Text = "";
            txtEmail.Text = "";
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
            var m = new User();
            m.Id = Id;
            m.Username = txtUsernane.Text.Trim();
            m.FullName = txtFullName.Text;
            m.Pwd = txtPwd.Text;
            m.Phone = txtPhone.Text;
            m.Email = txtEmail.Text;
            m.Note = txtNote.Text;
            long seq = UserFacade.Save(m);
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
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            var Id = dgvList.Id;
            if (Id == 0) return;

            // If locked
            var lInfo = UserFacade.GetLockInfo(Id);
            string msg = "";
            if (lInfo.IsLocked)
            {
                msg = "Record cannot be deleted because it is currently locked by '" + lInfo.LockBy + "' since '" + lInfo.LockAt + "'";
                new frmMsg(msg).ShowDialog();
                return;
            }
            // Delete
            msg = "Are you sure you want to delete?";
            if (MessageBox.Show(msg, "Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == System.Windows.Forms.DialogResult.No)
                return;
            UserFacade.SetStatus(Id, StatusType.Deleted);
            RefreshGrid();
            if (dgvList.RowCount == 0) btnNew_Click(sender, e);
        }

        private void btnCopy_Click(object sender, EventArgs e)
        {
            Id = 0;
            txtUsernane.Focus();
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

        private void frmUserList_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (btnUnlock.Text == "Cance&l")
                btnUnlock_Click(null, null);
            App.SaveSettings(); // Move to main form instead
        }

        private void btnActive_Click(object sender, EventArgs e)
        {
            var Id = dgvList.Id;
            if (Id == 0) return;
            var lInfo = UserFacade.GetLockInfo(Id);
            if (lInfo.IsLocked)
            {
                string msg = "Account is currently locked by '" + lInfo.LockBy + "' since '" + lInfo.LockAt + "'" +
                    "\nទិន្នន័យ​នេះ​កំពុង​ប្រើ​ប្រាស់​ដោយ '" + lInfo.LockBy + "' តាំងពី '" + lInfo.LockAt + "'";
                new frmMsg(msg).ShowDialog();
                return;
            }
            UserFacade.SetStatus(Id, btnActive.Text.StartsWith("I") ? StatusType.InActive : StatusType.Active);
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
                UserFacade.ReleaseLock(dgvList.Id);
                if (dgvList.RowCount > 0 && !dgvList.CurrentRow.Selected)
                    dgvList.CurrentRow.Selected = true;
                return;
            }

            // Unlock
            if (Id == 0) return;
            var lInfo = UserFacade.GetLockInfo(Id);

            if (lInfo.IsLocked) // Check if record is locked
            {
                string msg = "Account is currently locked by '" + lInfo.LockBy + "' since '" + lInfo.LockAt + "'";
                new frmMsg(msg).ShowDialog();
                return;
            }
            LockControls(false);
            UserFacade.Lock(dgvList.Id);
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

        private void mnuShow_CheckedChanged(object sender, EventArgs e)
        {
            if (!mnuShowA.Checked && !mnuShowI.Checked)
                mnuShowA.Checked = true;
            RefreshGrid();
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            mnuShow.Show(lblFilter, 0, 15);
        }

        private void mnuShow_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {

        }

        private void mnuShowA_Click(object sender, EventArgs e)
        {

        }
    }
}
