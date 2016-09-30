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
            Text = string.Format(Text, Settings.Default.ApplicationName) + " - Options";

            // Get path
            PathTextBox.Text = Settings.Default.UserGameFolderPath;
        }

        private void ChangeGameFolderButton_Click(object sender, EventArgs e)
        {
            try
            {
                // Configure browser dialog where current path is valid
                if (Directory.Exists(Settings.Default.UserGameFolderPath))
                    GameFolderBrowserDialog.SelectedPath = Settings.Default.UserGameFolderPath;
                else
                    GameFolderBrowserDialog.RootFolder = Environment.SpecialFolder.Desktop;

                // Get game folder from user
                var folderDialogResult = GameFolderBrowserDialog.ShowDialog();

                // If user selects a folder
                if (folderDialogResult == DialogResult.OK)
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
                    string.Format(
                        "{0} has encountered an error while attempting to update the game folder path.{1}{1}Error: {2}",
                        Settings.Default.ApplicationName, Environment.NewLine, ex.Message),
                    Settings.Default.ApplicationName, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CloseButton_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
