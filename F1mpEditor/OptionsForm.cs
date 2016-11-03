using System;
using System.IO;
using System.Windows.Forms;
using F1mpEditor.Properties;

namespace F1mpEditor
{
    public partial class OptionsForm : Form
    {
        public OptionsForm()
        {
            InitializeComponent();
        }

        private void OptionsForm_Load(object sender, EventArgs e)
        {
            // Set icon
            Icon = Resources.icon1;

            // Set form title text
            Text = $"{Settings.Default.ApplicationName} - Options";

            // Get path
            PathTextBox.Text = Settings.Default.UserGameFolderPath;
        }

        private void ChangeGameFolderButton_Click(object sender, EventArgs e)
        {
            try
            {
                // Configure browser dialog where current path is valid
                if (Directory.Exists(Settings.Default.UserGameFolderPath))
                {
                    GameFolderBrowserDialog.SelectedPath = Settings.Default.UserGameFolderPath;
                }
                else
                {
                    GameFolderBrowserDialog.RootFolder = Environment.SpecialFolder.Desktop;
                }

                // Get game folder from user
                var dialogResult = GameFolderBrowserDialog.ShowDialog();

                // If user selects a folder
                if (dialogResult == DialogResult.OK)
                {
                    // Save selected folder
                    Settings.Default.UserGameFolderPath = GameFolderBrowserDialog.SelectedPath;
                    Settings.Default.Save();

                    // Update controls
                    PathTextBox.Text = Settings.Default.UserGameFolderPath;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"{Settings.Default.ApplicationName} has encountered an error while attempting to update the game folder path.{Environment.NewLine}{Environment.NewLine}Error: {ex.Message}",
                    Settings.Default.ApplicationName, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CloseButton_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
