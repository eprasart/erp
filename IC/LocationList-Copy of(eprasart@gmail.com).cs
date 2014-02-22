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
            App.PrepareDatabase();

            Icon = Properties.Resources.Icon;
            dgvList.ShowLessColumns(true);
            RefreshGrid();
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            if (bExpand)
                imgExpand_Click(sender, e);
            txtCode.Text = "";
            txtCode.Focus();
            txtDescEN.Text = "";
            txtDescKH.Text = "";
            txtPhone.Text = "";
            txtFax.Text = "";
            txtAddress.Text = "";
            txtNote.Text = "";
            Id = 0;

            dgvList.Sort(colDescEN, System.ComponentModel.ListSortDirection.Ascending);
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            // TODO: Validation
            var m = new Location();
            m.Id = Id;
            m.Code = txtCode.Text;
            m.Desc_EN = txtDescEN.Text;
            m.Desc_KH = txtDescKH.Text;
            m.Phone = txtPhone.Text;
            m.Fax = txtFax.Text;
            m.Address = txtAddress.Text;
            m.Note = txtNote.Text;
            m.Status = StatusType.Active;
            LocationFacade.Save(m);
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

        private void dgvList_SelectionChanged(object sender, EventArgs e)
        {
            if (Id == dgvList.Id)
                return;
            else
                Id = dgvList.Id;
            if (Id == 0) return;
            var m = LocationFacade.Select(Id);
            txtCode.Text = m.Code;
            txtDescEN.Text = m.Desc_EN;
            txtDescKH.Text = m.Desc_KH;
            txtPhone.Text = m.Phone;
            txtFax.Text = m.Fax;
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
            LocationFacade.Delete(Id);
            RefreshGrid();
            if (dgvList.RowCount == 0) btnNew_Click(sender, e);
        }

        private void btnCopy_Click(object sender, EventArgs e)
        {
            Id = 0;
            txtCode.Focus();
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
            dgvList_SelectionChanged(sender, e);    // reload data since SelectionChanged will not occured on current row
        }
    }
}
