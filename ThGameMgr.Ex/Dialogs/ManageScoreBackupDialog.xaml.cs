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
                        for (int i = backupFiles.Length - 1; i >= 0; i--)
                        {
                            string backupFile = backupFiles[i];
                            string backupFileName = Path.GetFileName(backupFile);
                            BackupListBox.Items.Add(backupFileName);
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

        private void RestoreButtonClick(object sender, RoutedEventArgs e)
        {
            if (BackupListBox.SelectedIndex > -1)
            {
                try
                {
                    string selectedGameName = BackupGameListBox.SelectedItem as string;
                    string gameId = GameIndex.GetGameIdFromGameName(selectedGameName);
                    string backupFile = BackupListBox.SelectedItem as string;

                    MessageBoxResult result = MessageBox.Show(
                        this,
                        $"'{GameIndex.GetGameName(gameId)}' のスコアファイルを、バックアップ '{backupFile}' から復元します。よろしいですか。",
                        "スコアファイルの復元",
                        MessageBoxButton.YesNo, MessageBoxImage.Information);
                    if (result == MessageBoxResult.Yes)
                    {
                        ScoreBackup.Restore(gameId, backupFile);

                        MessageBox.Show(this, "復元しました。", "スコアファイルの復元",
                            MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(this, $"スコアファイルの復元に失敗しました。\n{ex.Message}",
                        "エラー",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show(this, "復元する元のバックアップを選択してください。", "スコアファイルの復元",
                    MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
        }

        private void DeleteButtonClick(object sender, RoutedEventArgs e)
        {
            if (BackupListBox.SelectedIndex > -1)
            {
                try
                {
                    string selectedGameName = BackupGameListBox.SelectedItem as string;
                    string gameId = GameIndex.GetGameIdFromGameName(selectedGameName);
                    string backupFile = BackupListBox.SelectedItem as string;

                    MessageBoxResult result = MessageBox.Show(
                        this, $"'{backupFile}' を削除してもよろしいですか。", "スコアバックアップの削除",
                        MessageBoxButton.YesNo, MessageBoxImage.Information);
                    if (result == MessageBoxResult.Yes)
                    {
                        ScoreBackup.Delete(gameId, backupFile);
                        BackupListBox.Items.Remove(backupFile);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(this, $"バックアップの削除に失敗しました。\n{ex.Message}", 
                        "エラー",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show(this, "削除するバックアップを選択してください。", "スコアバックアップの削除",
                    MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
        }
    }
}
