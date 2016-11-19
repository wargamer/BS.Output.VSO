﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using BS.Output.VSO.Models;
using BS.Output.VSO.Properties;
using BS.Output.VSO.Services;

namespace BS.Output.VSO
{
    public partial class EditOutputSettingsForm : Form
    {
        public EditOutputSettingsForm(VSOOutput output)
        {
            InitializeComponent();
            Output = output;

            txtUrl.Text = output.URL?.ToString();
            txtOutputName.Text = output.Name;
            Load += OnLoad;
        }
        
        public VSOOutput Output { get; set; }

        private void btnOk_Click(object sender, EventArgs e)
        {
            if (!ValidateUrl())
                return;

            VSOClient client;
            if (!TryGetClient(out client))
                return;

            string selectedProject = lbProjects.SelectedItem?.ToString();

            if (string.IsNullOrEmpty(selectedProject))
            {
                MessageBox.Show(Resources.Please_select_a_valid_project);
                return;
            }

            Output.Name = string.IsNullOrWhiteSpace(txtOutputName.Text)
                ? Output.Name
                : txtOutputName.Text;

            Output.ProjectName = selectedProject;
            Output.IterationName = lbIterations.SelectedItem?.ToString();

            DialogResult = DialogResult.OK;
            Close();
        }
        
        private async void OnLoad(object sender, EventArgs eventArgs)
        {
            if (Output.URL != null)
            {
                await GetProjects();
            }

            Load -= OnLoad;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
        
        private async void btnGetProjects_Click(object sender, EventArgs e)
        {
            await GetProjects();
        }

        private static void SetDataSourceAndSelectedItem(ComboBox comboxBox, IList<string> allItems, string selectedItem)
        {
            allItems.Insert(0, string.Empty);
            comboxBox.DataSource = allItems;

            if (string.IsNullOrEmpty(selectedItem))
                return;

            List<string> items = comboxBox.DataSource as List<string>;

            var itemInList = items
                ?.FirstOrDefault(p => p.Equals(selectedItem, StringComparison.OrdinalIgnoreCase));
            if (itemInList != null)
                comboxBox.SelectedItem = itemInList;
        }

        private async void lbProjects_SelectedIndexChanged(object sender, EventArgs e)
        {
            await GetIterations();
        }
        
        private async Task GetProjects()
        {
            VSOClient client;
            if (!ValidateUrl() || !TryGetClient(out client))
            {
                lbProjects.DataSource = new List<string>();
                lbIterations.DataSource = new List<string>();
                return;
            }

            btnGetProjects.Enabled = false;
            lbProjects.Enabled = false;
            lbIterations.Enabled = false;
            string oldText = btnGetProjects.Text;
            btnGetProjects.Text = Resources.Loading;

            try
            {
                var projects = (await client.GetProjects()).ToList();

                lbProjects.SelectedIndexChanged -= lbProjects_SelectedIndexChanged;

                string selectedItem = lbProjects.SelectedItem?.ToString() ?? Output.ProjectName;
                SetDataSourceAndSelectedItem(lbProjects, projects, selectedItem);

                await GetIterations();

                lbProjects.SelectedIndexChanged += lbProjects_SelectedIndexChanged;
            }
            finally
            {
                btnGetProjects.Text = oldText;
                btnGetProjects.Enabled = true;
                lbProjects.Enabled = true;
                lbIterations.Enabled = true;
            }
        }

        private async Task GetIterations()
        {
            string selectedProject = lbProjects.SelectedItem?.ToString();

            if (string.IsNullOrEmpty(selectedProject))
            {
                lbIterations.DataSource = new List<string>();
                return;
            }

            lbIterations.Enabled = false;

            try
            {
                VSOClient client;
                if (!TryGetClient(out client))
                    return;
                
                var iterations = (await client.GetIterations(selectedProject)).ToList();
                string selectedItem = lbIterations.SelectedItem?.ToString() ?? Output.IterationName;
                SetDataSourceAndSelectedItem(lbIterations, iterations, selectedItem);
            }
            finally
            {
                lbIterations.Enabled = true;
            }
        }

        private bool ValidateUrl()
        {
            Uri uriResult;
            bool isValid = Uri.TryCreate(txtUrl.Text, UriKind.Absolute, out uriResult)
                && uriResult.Scheme == Uri.UriSchemeHttps;

            if (isValid)
            {
                Output.URL = new Uri(txtUrl.Text);
            }
            else
            {
                MessageBox.Show(Resources.Invalid_URL);
            }

            return isValid;
        }

        private bool TryGetClient(out VSOClient client)
        {
            client = new VSOClient(Output);
            if (!client.Connect())
            {
                MessageBox.Show(Resources.Something_went_wrong_while_attempting_to_connect_to_VSO);
                return false;
            }

            return true;
        }
    }
}
