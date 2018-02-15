using BS.Output.VSO.Models;
using BS.Output.VSO.Properties;
using BS.Output.VSO.Services;
using BS.Output.VSO.Tools;
using System;
using System.Linq;
using System.Windows.Forms;

namespace BS.Output.VSO
{
    public partial class EditBugDetailsForm : Form
    {
        public EditBugDetailsForm(VSOOutput output)
        {
            Options = new BugDetails();
            InitializeComponent();

            if (string.IsNullOrEmpty(output.BuildDefinitionName))
            {
                lbBuilds.Hide();
                lblBuild.Hide();
            }
            else
            {
                GetBuilds(output);
            }
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

        private void lbBuilds_SelectedIndexChanged(object sender, EventArgs e)
        {
            Options.Build = lbBuilds.SelectedItem?.ToString();
        }

        private async void GetBuilds(VSOOutput output)
        {
            var client = new VSOClient(output);
            if (!await client.Connect())
            {
                MessageBox.Show(Resources.Something_went_wrong_while_attempting_to_connect_to_VSO);
                return;
            }

            var builds = await client.GetBuilds(output.ProjectName, output.BuildDefinitionName);
            var list = builds.OrderByDescending(b => b, new VersionNumberComparer()).ToList();
            list.Insert(0, string.Empty);
            lbBuilds.DataSource = list;
        }
    }
}
