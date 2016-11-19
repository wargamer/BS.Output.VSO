using BS.Output.VSO.Properties;
using System;
using System.Windows.Forms;
using BS.Output.VSO.Models;

namespace BS.Output.VSO
{
    public partial class EditBugDetailsForm : Form
    {
        public EditBugDetailsForm()
        {
            Options = new BugDetails();
            InitializeComponent();
        }

        public BugDetails Options { get; }

        private void btnOk_Click(object sender, EventArgs e)
        {
            if (!Options.Validate())
            {
                MessageBox.Show(Resources.Please_fill_in_all_fields);
                return;
            }   

            DialogResult = DialogResult.OK;
            Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
        
        private void txtTitle_TextChanged(object sender, EventArgs e)
        {
            Options.Title = txtTitle.Text;
        }

        private void txtReproSteps_TextChanged(object sender, EventArgs e)
        {
            Options.ReproSteps = txtReproSteps.Text;
        }
    }
}
