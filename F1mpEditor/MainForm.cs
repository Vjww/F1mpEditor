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
    public partial class MainForm : Form
    {
        /* Team order/indices in the game
         * ------------------------------
         * Offset B508 in saved game file is the number of the team selected (for player 1).
         * 
         * Teams offsets B488 -> B4D0 (19 x 4 bytes). Note order of teams is not consistant.
         * 
         * 1997
         * ----
         * Team      GameIndexOrder GameChampOrder1996 ActualEnd96/End97
         * Benetton   1               3                   3 /  3         // Not sure why 1st
         * Williams   2               1                   1 /  1
         * Ferrari    3               2                   2 /  2
         * McLaren    4               4                   4 /  4
         * Jordan     5               5                   5 /  5
         * Sauber     6               6                   7 /  7         // Not sure why 6th
         * Prost      7               7                   6 /  6         // Not sure why 7th
         * Tyrrell    8               9                   8 / 10
         * Arrows     9              10                   9 /  8
         * Minardi   10              11                  10 / 11
         * Stewart   11               8                 N/A /  9         // Suspect 11th as new entry into sport for 1997
         * Lola      12              12
         * Forti     13              13
         * Larousse  14              14
         * Lotus     15              15
         * Pacific   16              16
         * Honda     17              17
         * Simtek    18              18
         * TNT       19              19
         */

        private const int TeamCount = 19;

        public MainForm()
        {
            InitializeComponent();
        }

        public void PerformRefresh()
        {
            UpdateSavedGameRadioButtons();
            ConfigureSavedGameControls();
        }

        private static string GetApplicationVersion()
        {
#if (!DEBUG)
            {
                return System.Deployment.Application.ApplicationDeployment.IsNetworkDeployed
                    ? System.Deployment.Application.ApplicationDeployment.CurrentDeployment.CurrentVersion.ToString(2)
                    : FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).ProductVersion;
            }
#else
            {
                return FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).FileVersion;
            }
#endif
        }

        private void SavedGameEditorForm_Load(object sender, EventArgs e)
        {
            // Set icon
            Icon = Resources.icon1;

            // Set form title text
            Text = $"{Settings.Default.ApplicationName} v{GetApplicationVersion()}";

            // Convert lines in control to rtf
            var rtfString = string.Empty;
            foreach (var item in BasicDescriptionRichTextBox.Lines)
            {
                // Add linebreak after each item
                rtfString += item + @"\line ";
            }
            BasicDescriptionRichTextBox.Rtf = rtfString.TrimEnd(@"\line".ToCharArray());

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

            UpdateSavedGameRadioButtons();
            ConfigureControls();
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
                var teams = new TeamCollection();
                for (var id = 0; id < TeamCount; id++)
                {
                    var valueMapping = new Data.ValueMapping.SavedGame.Team.Team(id);
                    var team = new Team
                    {
                        Id = id,
                        Name = Encoding.ASCII.GetString(savedGameConnection.ReadByteArray(valueMapping.Name, Data.ValueMapping.SavedGame.Team.Team.NameLength)),

                        Department1Motivation = savedGameConnection.ReadInteger(valueMapping.Department1Motivation),
                        Department1Happiness = savedGameConnection.ReadInteger(valueMapping.Department1Happiness),
                        Department2Motivation = savedGameConnection.ReadInteger(valueMapping.Department2Motivation),
                        Department2Happiness = savedGameConnection.ReadInteger(valueMapping.Department2Happiness),
                        Department3Motivation = savedGameConnection.ReadInteger(valueMapping.Department3Motivation),
                        Department3Happiness = savedGameConnection.ReadInteger(valueMapping.Department3Happiness),
                        Department4Motivation = savedGameConnection.ReadInteger(valueMapping.Department4Motivation),
                        Department4Happiness = savedGameConnection.ReadInteger(valueMapping.Department4Happiness),
                        Department5Motivation = savedGameConnection.ReadInteger(valueMapping.Department5Motivation),
                        Department5Happiness = savedGameConnection.ReadInteger(valueMapping.Department5Happiness)
                    };
                    teams.Add(team);
                }

                var player1TeamId = savedGameConnection.ReadInteger(Data.ValueMapping.SavedGame.Player.Player.GetPlayer1TeamIdOffset()) - 1;
                var player1TeamRecord = teams.Single(x => x.Id == player1TeamId);

                // Populate controls
                PopulateHeaderControls(player1TeamId, player1TeamRecord.Name);
                PopulateBasicControls(player1TeamRecord);
                PopulateAdvancedControls(teams);

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
                var teams = TeamsDataGridView.DataSource as TeamCollection;
                if (teams == null)
                {
                    MessageBox.Show("Please import data from a saved game file first before attempting to export.",
                        Settings.Default.ApplicationName, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }

                foreach (var team in teams)
                {
                    var valueMapping = new Data.ValueMapping.SavedGame.Team.Team(team.Id);
                    savedGameConnection.WriteInteger(valueMapping.Department1Motivation, team.Department1Motivation);
                    savedGameConnection.WriteInteger(valueMapping.Department1Happiness, team.Department1Happiness);
                    savedGameConnection.WriteInteger(valueMapping.Department2Motivation, team.Department2Motivation);
                    savedGameConnection.WriteInteger(valueMapping.Department2Happiness, team.Department2Happiness);
                    savedGameConnection.WriteInteger(valueMapping.Department3Motivation, team.Department3Motivation);
                    savedGameConnection.WriteInteger(valueMapping.Department3Happiness, team.Department3Happiness);
                    savedGameConnection.WriteInteger(valueMapping.Department4Motivation, team.Department4Motivation);
                    savedGameConnection.WriteInteger(valueMapping.Department4Happiness, team.Department4Happiness);
                    savedGameConnection.WriteInteger(valueMapping.Department5Motivation, team.Department5Motivation);
                    savedGameConnection.WriteInteger(valueMapping.Department5Happiness, team.Department5Happiness);
                }

                MessageBox.Show("Export complete!");
            }
            finally
            {
                savedGameConnection?.Close();
            }
        }

        private void OptionsButton_Click(object sender, EventArgs e)
        {
            try
            {
                // Hide parent form and show child form
                var parentForm = this;
                var childForm = new OptionsForm();
                childForm.Show(parentForm);
                parentForm.Hide();
                childForm.FormClosing += delegate { parentForm.Show(); parentForm.PerformRefresh(); };
            }
            catch (Exception)
            {
                Close();
            }
        }

        private void BoostButton_Click(object sender, EventArgs e)
        {
            var buttonTag = ((Button)sender).Tag.ToString();
            int boostButtonId;
            if (!int.TryParse(buttonTag, out boostButtonId))
            {
                throw new Exception("Unable to parse Button.Tag property to int.");
            }

            // Boost value in data source for player 1 team
            var player1TeamId = (int)Player1TeamNameLabel.Tag;
            var teams = (TeamCollection)TeamsDataGridView.DataSource;

            switch (boostButtonId)
            {
                // Cases for Motivation
                case 1:
                    teams.Single(x => x.Id == player1TeamId).Department1Motivation = 25;
                    break;
                case 3:
                    teams.Single(x => x.Id == player1TeamId).Department2Motivation = 25;
                    break;
                case 5:
                    teams.Single(x => x.Id == player1TeamId).Department3Motivation = 25;
                    break;
                case 7:
                    teams.Single(x => x.Id == player1TeamId).Department4Motivation = 25;
                    break;
                case 9:
                    teams.Single(x => x.Id == player1TeamId).Department5Motivation = 25;
                    break;

                // Cases for Happiness
                case 2:
                    teams.Single(x => x.Id == player1TeamId).Department1Happiness = 25;
                    break;
                case 4:
                    teams.Single(x => x.Id == player1TeamId).Department2Happiness = 25;
                    break;
                case 6:
                    teams.Single(x => x.Id == player1TeamId).Department3Happiness = 25;
                    break;
                case 8:
                    teams.Single(x => x.Id == player1TeamId).Department4Happiness = 25;
                    break;
                case 10:
                    teams.Single(x => x.Id == player1TeamId).Department5Happiness = 25;
                    break;

                default:
                    throw new NotImplementedException("Case not implemented in switch statement.");
            }

            PopulateHeaderControls(player1TeamId, teams.Single(x => x.Id == player1TeamId).Name);
            PopulateBasicControls(teams.Single(x => x.Id == player1TeamId));
        }

        private static string BuildSavedGameFileName(int id)
        {
            // Return "SAVE.001" or similar depending on selected file number
            return "SAVE." + id.ToString("000");
        }

        private static string BuildSavedGameFilePath(int id)
        {
            return Path.Combine(Settings.Default.UserGameFolderPath, Settings.Default.DefaultSavedGameFolderName, BuildSavedGameFileName(id));
        }

        private void ConfigureControls()
        {
            // Configure
            ConfigureSavedGameControls();
            ConfigureDataGridViewControl(TeamsDataGridView);
        }

        private void ConfigureSavedGameControls()
        {
            // If at least one radio buttons are enabled, enable Import/Export buttons, else disable
            var isOneOrMoreSavedGamesAvailable = (FileGroupBox.Controls.OfType<RadioButton>().FirstOrDefault(r => r.Enabled) != null);
            ImportButton.Enabled = isOneOrMoreSavedGamesAvailable;
            ExportButton.Enabled = isOneOrMoreSavedGamesAvailable;

            // Deselect all radio buttons
            if (!isOneOrMoreSavedGamesAvailable)
            {
                foreach (var radioButton in FileGroupBox.Controls.OfType<RadioButton>())
                {
                    radioButton.Checked = false;
                }
            }

            // Select first radio button that is enabled
            if (isOneOrMoreSavedGamesAvailable)
            {
                FileGroupBox.Controls.OfType<RadioButton>()
                    .OrderBy(x => Convert.ToInt32(x.Text))
                    .First(r => r.Enabled)
                    .Select();
            }
        }

        private static void ConfigureDataGridViewControl(DataGridView control)
        {
            control.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            control.AllowUserToAddRows = false;
            control.AllowUserToDeleteRows = false;
            control.AllowUserToResizeRows = false;
            control.MultiSelect = false;
            control.RowHeadersVisible = false;

            // Read-Only
            foreach (DataGridViewColumn column in control.Columns)
            {
                column.ReadOnly = true;
            }
        }

        private void ConfigureGameFolder()
        {
            try
            {
                // Prompt the user to select the game folder
                MessageBox.Show(
                    string.Format("{0} requires you to select the {1} installation folder.{2}{2}Click OK to browse for the {1} installation folder.",
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
                        string.Format("As you did not select an installation folder for {1}, {0} will assume that the game is installed at the following location.{2}{2}{3}",
                            Settings.Default.ApplicationName, Settings.Default.GameName, Environment.NewLine, Settings.Default.DefaultGameFolderPath),
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
                        "{0} has encountered an error while attempting to configure the game folder.{1}{1}" + "Error: {2}{1}{1}To resolve this error, try running {0} as an administrator.",
                        Settings.Default.ApplicationName, Environment.NewLine, ex.Message),
                    Settings.Default.ApplicationName, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private static Color GetIndicatorColor(int value)
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

        private static bool IsSavedGameFileExists(int id)
        {
            return File.Exists(BuildSavedGameFilePath(id));
        }

        private void PopulateAdvancedControls(IEnumerable teams)
        {
            TeamsDataGridView.DataSource = teams;
        }

        private void PopulateBasicControls(ITeam record)
        {
            PopulatePlayerTeamValues(MotivationPercentage01Label, MotivationIndicator01Label, Boost01Button, record.Department1Motivation);
            PopulatePlayerTeamValues(MotivationPercentage02Label, MotivationIndicator02Label, Boost03Button, record.Department2Motivation);
            PopulatePlayerTeamValues(MotivationPercentage03Label, MotivationIndicator03Label, Boost05Button, record.Department3Motivation);
            PopulatePlayerTeamValues(MotivationPercentage04Label, MotivationIndicator04Label, Boost07Button, record.Department4Motivation);
            PopulatePlayerTeamValues(MotivationPercentage05Label, MotivationIndicator05Label, Boost09Button, record.Department5Motivation);
            PopulatePlayerTeamValues(HappinessPercentage01Label, HappinessIndicator01Label, Boost02Button, record.Department1Happiness);
            PopulatePlayerTeamValues(HappinessPercentage02Label, HappinessIndicator02Label, Boost04Button, record.Department2Happiness);
            PopulatePlayerTeamValues(HappinessPercentage03Label, HappinessIndicator03Label, Boost06Button, record.Department3Happiness);
            PopulatePlayerTeamValues(HappinessPercentage04Label, HappinessIndicator04Label, Boost08Button, record.Department4Happiness);
            PopulatePlayerTeamValues(HappinessPercentage05Label, HappinessIndicator05Label, Boost10Button, record.Department5Happiness);
        }

        private void PopulateHeaderControls(int player1TeamId, string player1TeamName)
        {
            Player1TeamNameLabel.Tag = player1TeamId;
            Player1TeamNameLabel.Text = player1TeamName;
        }

        private static void PopulatePlayerTeamValues(Control percentageLabel, Control indicatorLabel, Control boostButton, int value)
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
    }
}
