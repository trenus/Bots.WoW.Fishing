using System;
using System.Windows.Forms;

namespace WoW.Fishing
{
    public partial class frmMain : Form
    {
        public const string Title = "WoW Fishbot";

        Manager m = new Manager();

        public frmMain()
        {
            InitializeComponent();

            this.Text = Title;
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            Manager.RefreshLure = mnuRefreshLure.Checked;
            RefreshImages();
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            m.StartFishing();
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            m.StopFishing();
        }

        private void mnuExit_Click(object sender, EventArgs e)
        {
            btnExit_Click(sender, e);
        }

        private void mnuSaveCursors_Click(object sender, EventArgs e)
        {
            m.SaveCursors();
            RefreshImages();
        }

        private void mnuRefreshLure_Click(object sender, EventArgs e)
        {
            Manager.RefreshLure = mnuRefreshLure.Checked;
        }

        private void RefreshImages()
        {
            imgDefault.Image = Manager.DefaultCursor;
            imgTarget.Image = Manager.TargetCursor;
        }
    }
}
