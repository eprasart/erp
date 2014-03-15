/*TODO: 
 * Prompt deletion => khmer msg
 * Edit cancel confirmation
 * Shortcut key
 * datagridview: remember row index after refresh
 * spliterDistance: save in table by user
 * datagridview sort by column header
 */
using System;
using System.Windows.Forms;

namespace ERP
{
    public partial class frmLocationList : Form
    {
        private int Id = 0;
        private bool bExpand = false;

        public frmLocationList()
        {
            InitializeComponent();
        }

        private void RefreshGrid()
        {
            dgvList.DataSource = LocationFacade.Select(txtSearch.Text);

        }

        private void frmLocationList_Load(object sender, EventArgs e)
        {
            App.InitApp();

            Icon = Properties.Resources.Icon;
            dgvList.ShowLessColumns(true);
            RefreshGrid();

            Text += " v. " + App.version;
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            if (bExpand)
                picExpand_Click(sender, e);
            txtCode.Text = "";
            txtCode.Focus();
            txtDescEN.Text = "";
            txtDescKH.Text = "";
            txtPhone.Text = "";
            txtFax.Text = "";
            txtAddress.Text = "";
            txtNote.Text = "";
            Id = 0;

            // dgvList.Sort(colDescEN, System.ComponentModel.ListSortDirection.Ascending);
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            // TODO: Validation
            Cursor = Cursors.WaitCursor;
            var m = new Location();
            m.Id = Id;
            m.Code = txtCode.Text;
            m.Desc1 = txtDescEN.Text;
            m.Desc2 = txtDescKH.Text;
            m.Address = txtAddress.Text;
            m.Note = txtNote.Text;
            LocationFacade.Save(m);
            RefreshGrid();
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
            if (Id == dgvList.Id)
                return;
            else
                Id = dgvList.Id;
            if (Id == 0) return;
            var m = LocationFacade.Select(Id);
            txtCode.Text = m.Code;
            txtDescEN.Text = m.Desc1;
            txtDescKH.Text = m.Desc2;
            txtAddress.Text = m.Address;
            txtNote.Text = m.Note;
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
            if (MessageBox.Show("Are you sure you want to delete?", "Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == System.Windows.Forms.DialogResult.No) return;
            LocationFacade.SetStatus(Id, StatusType.Deleted);
            RefreshGrid();
            if (dgvList.RowCount == 0) btnNew_Click(sender, e);
        }

        private void btnCopy_Click(object sender, EventArgs e)
        {
            Id = 0;
            txtCode.Focus();
        }

        private void picExpand_Click(object sender, EventArgs e)
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
                splitContainer1.SplitterDistance = 300; // TODO: load from var or db
                splitContainer1.FixedPanel = FixedPanel.Panel1;
                imgExpand.Image = Properties.Resources.Next;
            }
            dgvList.ShowLessColumns(bExpand);
            bExpand = !bExpand;
        }

        private void dgvList_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            //foreach (DataGridViewColumn column in dgvList.Columns)
            //    column.SortMode = DataGridViewColumnSortMode.Automatic;
        }

        private void dgvList_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (bExpand) picExpand_Click(sender, e);
            dgvList_SelectionChanged(sender, e);    // reload data since SelectionChanged will not occured on current row
        }

        private void frmLocationList_FormClosed(object sender, FormClosedEventArgs e)
        {
            App.SaveSettings();
        }

        private void btnActive_Click(object sender, EventArgs e)
        {
            var Id = dgvList.Id;
            if (Id == 0) return;            
            //todo: if A else I
            LocationFacade.SetStatus(Id, StatusType.InActive);
            RefreshGrid();            
        }
    }
}
