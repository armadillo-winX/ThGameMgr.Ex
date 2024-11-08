using System.Windows.Controls;

namespace ThGameMgr.Ex.Dialogs
{
    /// <summary>
    /// ManageScoreBackupDialog.xaml の相互作用ロジック
    /// </summary>
    public partial class ManageScoreBackupDialog : Window
    {
        public ManageScoreBackupDialog()
        {
            InitializeComponent();

            BackupGameListBox.Items.Clear();
            BackupListBox.Items.Clear();

            if (Directory.Exists($"{User.CurrentUserDirectoryPath}\\backup\\"))
            {
                string[] backupDirectories
                    = Directory.GetDirectories(
                        $"{User.CurrentUserDirectoryPath}\\backup\\", "*", SearchOption.TopDirectoryOnly);

                foreach (string backupDirectory in backupDirectories)
                {
                    string directoryName = Path.GetFileName(backupDirectory);
                    BackupGameListBox.Items.Add(GameIndex.GetGameName(directoryName));
                }
            }
        }

        private void BackupGameListBoxSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            BackupListBox.Items.Clear();

            if (BackupGameListBox.Items.Count > 0 && BackupGameListBox.SelectedIndex > -1)
            {
                string selectedGameName = BackupGameListBox.SelectedItem as string;
                string gameId = GameIndex.GetGameIdFromGameName(selectedGameName);

                try
                {
                    string[] backupFiles = ScoreBackup.GetScoreBackupFiles(gameId);
                    if (backupFiles.Length > 0)
                    {
                        int i = backupFiles.Length - 1;
                        while (true)
                        {
                            string backupFile = backupFiles[i];
                            string backupFileName = Path.GetFileName(backupFile);
                            BackupListBox.Items.Add(backupFileName);

                            if (i == 0)
                                break;

                            i--;
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(this, ex.Message, "エラー",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}
