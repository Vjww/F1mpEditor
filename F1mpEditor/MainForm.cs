using Common.Enums;
using Data.Collections.SavedGame.Team;
using Data.Entities.SavedGame.Team;
using Data.FileConnection;
using System;
using System.Collections;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using F1mpEditor.Properties;

namespace F1mpEditor
{
    public partial class SavedGameEditorForm : Form
    {
        private const int TeamCount = 19;

        public TeamCollection Teams { get; set; }

        public SavedGameEditorForm()
        {
            InitializeComponent();

            var rtfString = string.Empty;
            foreach (var item in BasicDescriptionRichTextBox.Lines)
            {
                rtfString += item + @"\line "; // Add linebreak after each item
            }
            BasicDescriptionRichTextBox.Rtf = rtfString.TrimEnd(@"\line".ToCharArray());
        }

        private void SavedGameEditorForm_Load(object sender, EventArgs e)
        {
            // Set icon
            Icon = Resources.icon1;

#if (!DEBUG)
            {
                // Set form title text
                Text = string.Format("{0} v{1}",
                    Settings.Default.ApplicationName,
                    FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).ProductVersion);
            }
#else
            {
                // Set form title text
                Text = string.Format("{0} v{1}", Settings.Default.ApplicationName,
                    FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).FileVersion);
            }
#endif

            // On initial run
            if (Settings.Default.InitialRun)
            {
                //
            }

            // If game folder has not been set
            if (string.IsNullOrWhiteSpace(Settings.Default.UserGameFolderPath))
            {
                ConfigureGameFolder();
            }

            Settings.Default.InitialRun = false;
            Settings.Default.Save();

            ConfigureControls();
            UpdateSavedGameRadioButtons();
        }

        private void ImportButton_Click(object sender, EventArgs e)
        {
            var fileName = GetSelectedSavedGame();
            var filePath = BuildSavedGameFilePath(fileName);

            SavedGameConnection savedGameConnection = null;
            try
            {
                savedGameConnection = new SavedGameConnection();
                savedGameConnection.Open(filePath, StreamDirectionType.Read);

                // Import from file

                // TODO move player team offset to value mapping???
                var playerTeamId = savedGameConnection.ReadInteger(46344) - 1;
                //var playerTeamMotivation = savedGameConnection.ReadInteger(position + (offset * (playerTeam - 1)));

                Teams = new TeamCollection();
                for (var id = 0; id < TeamCount; id++)
                {
                    var valueMapping = new Data.ValueMapping.SavedGame.Team.Team(id);
                    var team = new Team
                    {
                        Id = id,
                        Name = Encoding.ASCII.GetString(savedGameConnection.ReadByteArray(valueMapping.Name, Data.ValueMapping.SavedGame.Team.Team.NameLength)),

                        Department1Happiness = savedGameConnection.ReadInteger(valueMapping.Department1Happiness),
                        Department1Motivation = savedGameConnection.ReadInteger(valueMapping.Department1Motivation),
                        Department2Happiness = savedGameConnection.ReadInteger(valueMapping.Department2Happiness),
                        Department2Motivation = savedGameConnection.ReadInteger(valueMapping.Department2Motivation),
                        Department3Happiness = savedGameConnection.ReadInteger(valueMapping.Department3Happiness),
                        Department3Motivation = savedGameConnection.ReadInteger(valueMapping.Department3Motivation),
                        Department4Happiness = savedGameConnection.ReadInteger(valueMapping.Department4Happiness),
                        Department4Motivation = savedGameConnection.ReadInteger(valueMapping.Department4Motivation),
                        Department5Happiness = savedGameConnection.ReadInteger(valueMapping.Department5Happiness),
                        Department5Motivation = savedGameConnection.ReadInteger(valueMapping.Department5Motivation)
                    };
                    Teams.Add(team);
                }

                PopulateControls(playerTeamId, Teams);
                MessageBox.Show("Import complete!");
            }
            finally
            {
                savedGameConnection?.Close();
            }
        }

        private void ExportButton_Click(object sender, EventArgs e)
        {
            var fileName = GetSelectedSavedGame();
            var filePath = BuildSavedGameFilePath(fileName);

            SavedGameConnection savedGameConnection = null;
            try
            {
                savedGameConnection = new SavedGameConnection();
                savedGameConnection.Open(filePath, StreamDirectionType.Write);

                // Export to file
                Teams = TeamsDataGridView.DataSource as TeamCollection;
                if (Teams == null)
                {
                    MessageBox.Show("Please import data from a saved game file first before attempting to export.",
                        Settings.Default.ApplicationName, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }

                foreach (var team in Teams)
                {
                    var valueMapping = new Data.ValueMapping.SavedGame.Team.Team(team.Id);
                    savedGameConnection.WriteInteger(valueMapping.Department1Happiness, team.Department1Happiness);
                    savedGameConnection.WriteInteger(valueMapping.Department1Motivation, team.Department1Motivation);
                    savedGameConnection.WriteInteger(valueMapping.Department2Happiness, team.Department2Happiness);
                    savedGameConnection.WriteInteger(valueMapping.Department2Motivation, team.Department2Motivation);
                    savedGameConnection.WriteInteger(valueMapping.Department3Happiness, team.Department3Happiness);
                    savedGameConnection.WriteInteger(valueMapping.Department3Motivation, team.Department3Motivation);
                    savedGameConnection.WriteInteger(valueMapping.Department4Happiness, team.Department4Happiness);
                    savedGameConnection.WriteInteger(valueMapping.Department4Motivation, team.Department4Motivation);
                    savedGameConnection.WriteInteger(valueMapping.Department5Happiness, team.Department5Happiness);
                    savedGameConnection.WriteInteger(valueMapping.Department5Motivation, team.Department5Motivation);
                }

                MessageBox.Show("Export complete!");
            }
            finally
            {
                savedGameConnection?.Close();
            }
        }

        private string BuildSavedGameFileName(int id)
        {
            // Return "SAVE.001" or similar depending on selected file number
            return "SAVE." + id.ToString("000");
        }

        private string BuildSavedGameFilePath(int id)
        {
            return Path.Combine(Settings.Default.UserGameFolderPath,
                Settings.Default.DefaultSavedGameFolderName, BuildSavedGameFileName(id));
        }

        private void ConfigureControls()
        {
            ConfigureDataGridViewControl(TeamsDataGridView);
        }

        private void ConfigureDataGridViewControl(DataGridView control)
        {
            control.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            control.AllowUserToAddRows = false;
            control.AllowUserToDeleteRows = false;
            control.AllowUserToResizeRows = false;
            control.MultiSelect = false;
            control.RowHeadersVisible = false;
        }

        private void ConfigureGameFolder()
        {
            try
            {
                // Prompt the user to select the game folder
                MessageBox.Show(
                    string.Format(
                        "{0} requires you to select the {1} installation folder.{2}{2}" +
                        "Click OK to browse for the {1} installation folder.",
                        Settings.Default.ApplicationName, Settings.Default.GameName, Environment.NewLine),
                    Settings.Default.ApplicationName, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

                // Get game folder from user
                var folderDialogResult = GameFolderBrowserDialog.ShowDialog();

                // If user does not select an installation folder
                if (folderDialogResult != DialogResult.OK)
                {
                    // Set installation folder to default and show message to advise
                    Settings.Default.UserGameFolderPath = Settings.Default.DefaultGameFolderPath;
                    MessageBox.Show(
                        string.Format(
                            "As you did not select an installation folder for {1}, {0} will assume that the game is installed at the following location.{2}{2}" +
                            Settings.Default.DefaultGameFolderPath,
                            Settings.Default.ApplicationName, Settings.Default.GameName, Environment.NewLine),
                        Settings.Default.ApplicationName, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }

                else
                {
                    Settings.Default.UserGameFolderPath = GameFolderBrowserDialog.SelectedPath;
                }

                // Save selected game installation folder
                Settings.Default.Save();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    string.Format(
                        "{0} has encountered an error while attempting to configure the game folder.{1}{1}" + "Error: " +
                        ex.Message + "{1}{1}" + "To resolve this error, try running {0} as an administrator.{1}{1}.",
                        Settings.Default.ApplicationName, Environment.NewLine), Settings.Default.ApplicationName,
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private int GetSelectedSavedGame()
        {
            var selectedRadioButton = FileGroupBox.Controls.OfType<RadioButton>().FirstOrDefault(r => r.Checked);
            if (selectedRadioButton == null)
            {
                throw new Exception("No RadioButton selected.");
            }

            int selectedFileNumber;
            if (!int.TryParse(selectedRadioButton.Text, out selectedFileNumber))
            {
                throw new Exception("Unable to parse RadioButton.Text property to int.");
            }

            return selectedFileNumber;
        }

        private bool IsSavedGameFileExists(int id)
        {
            return File.Exists(BuildSavedGameFilePath(id));
        }

        private void PopulateControls(int playerTeamId, TeamCollection teams)
        {
            var playerTeamRecord = teams.Single(x => x.Id == playerTeamId);
            Player1TeamNameLabel.Text = playerTeamRecord.Name;

            // Populate controls on Basic tab 
            PopulatePlayerTeamValues(HappinessPercentage01Label, HappinessIndicator01Label, Boost01Button, playerTeamRecord.Department1Happiness);
            PopulatePlayerTeamValues(HappinessPercentage02Label, HappinessIndicator02Label, Boost03Button, playerTeamRecord.Department2Happiness);
            PopulatePlayerTeamValues(HappinessPercentage03Label, HappinessIndicator03Label, Boost05Button, playerTeamRecord.Department3Happiness);
            PopulatePlayerTeamValues(HappinessPercentage04Label, HappinessIndicator04Label, Boost07Button, playerTeamRecord.Department4Happiness);
            PopulatePlayerTeamValues(HappinessPercentage05Label, HappinessIndicator05Label, Boost09Button, playerTeamRecord.Department5Happiness);
            PopulatePlayerTeamValues(MotivationPercentage01Label, MotivationIndicator01Label, Boost02Button, playerTeamRecord.Department1Motivation);
            PopulatePlayerTeamValues(MotivationPercentage02Label, MotivationIndicator02Label, Boost04Button, playerTeamRecord.Department2Motivation);
            PopulatePlayerTeamValues(MotivationPercentage03Label, MotivationIndicator03Label, Boost06Button, playerTeamRecord.Department3Motivation);
            PopulatePlayerTeamValues(MotivationPercentage04Label, MotivationIndicator04Label, Boost08Button, playerTeamRecord.Department4Motivation);
            PopulatePlayerTeamValues(MotivationPercentage05Label, MotivationIndicator05Label, Boost10Button, playerTeamRecord.Department5Motivation);

            // Populate controls on Advanced tab
            TeamsDataGridView.DataSource = teams;
        }

        private Color GetIndicatorColor(int value)
        {
            // Return Red for 24 or less
            // Return Orange for 25-49
            // Return Green for 50 or more

            Color result;
            if (value < 25) result = Color.Red;
            else if ((value >= 25) && (value < 50)) result = Color.Orange;
            else result = Color.Green;
            return result;
        }

        private void PopulatePlayerTeamValues(Control percentageLabel, Control indicatorLabel, Control boostButton, int value)
        {
            percentageLabel.Text = value + "%";
            indicatorLabel.BackColor = GetIndicatorColor(value);
            boostButton.Visible = value < 25;
        }

        private void UpdateSavedGameRadioButtons()
        {
            FileRadioButton1.Enabled = IsSavedGameFileExists(1);
            FileRadioButton2.Enabled = IsSavedGameFileExists(2);
            FileRadioButton3.Enabled = IsSavedGameFileExists(3);
            FileRadioButton4.Enabled = IsSavedGameFileExists(4);
            FileRadioButton5.Enabled = IsSavedGameFileExists(5);
            FileRadioButton6.Enabled = IsSavedGameFileExists(6);
            FileRadioButton7.Enabled = IsSavedGameFileExists(7);
            FileRadioButton8.Enabled = IsSavedGameFileExists(8);
            FileRadioButton9.Enabled = IsSavedGameFileExists(9);
            FileRadioButton10.Enabled = IsSavedGameFileExists(10);
        }

        private void OptionsButton_Click(object sender, EventArgs e)
        {
            var form = new OptionsForm();
            form.Show();
        }

        private void BoostButton_Click(object sender, EventArgs e)
        {
            var buttonTag = ((Button)sender).Tag.ToString();
            int boostButtonId;
            if (!int.TryParse(buttonTag, out boostButtonId))
            {
                throw new Exception("Unable to parse Button.Tag property to int.");
            }
            switch (boostButtonId)
            {
                case 1:
                    // Update grid cell to 25 etc.
                    break;
                default:
                    throw new NotImplementedException("Case not implemented in switch statement.");
            }

            // TODO Update datasource
            // TODO Populate controls
        }

        //private void button1_Click(object sender, EventArgs e)
        //{
        //    var connection = new SavedGameConnection();
        //    connection.Open("", StreamDirectionType.Read);
        //
        //    var playerTeam = connection.ReadInteger(46344);
        //    var playerTeamMotivation = connection.ReadInteger(position + (offset * (playerTeam - 1)));
        //
        //}

        /*
        Offset B508 in saved game file is the number of the team selected!
        Note order of teams is not consistant.

        B488 -> B4D0 (19 x 4 bytes)

        1997
        ----
        Team GameOrder ChampOrder1996 Actual96/97
        Benetton   1 3
        Williams   2 1
        Ferrari    3 2
        McLaren    4 4
        Jordan     5 5
        Sauber     6 6 7/7?
        Prost      7 7 6/6?
        Tyrrell    8 9 8/10?
        Arrows     9 10 9/8?
        Minardi   10 11 10/11?
        Stewart   11 8 N/A/9?
        Lola      12 12
        Forti     13 13
        Larousse  14 14
        Lotus     15 15
        Pacific   16 16
        Honda     17 17
        Simtek    18 18
        TNT       19 19
        */
    }
}
