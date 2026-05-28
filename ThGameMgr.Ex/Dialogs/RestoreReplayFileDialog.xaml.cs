namespace ThGameMgr.Ex.Dialogs
{
    /// <summary>
    /// RestoreReplayFileDialog.xaml の相互作用ロジック
    /// </summary>
    public partial class RestoreReplayFileDialog : Window
    {
        private string GameId { get; set; }

        private string ReplayBackupFilePath { get; set; }

        public RestoreReplayFileDialog(
            string gameId, string replayBackupFilePath
            )
        {
            InitializeComponent();

            this.GameId = gameId;
            this.ReplayBackupFilePath = replayBackupFilePath;

            AllowOverwriteCheckBox.IsChecked = true;

            try
            {
                ReplayFileBackupInfo backupInfo = ReplayBackup.GetReplayBackupFileInfo(this.ReplayBackupFilePath);
                NameBox.Text = Path.GetFileNameWithoutExtension(backupInfo.SourceReplayFilePath);
            }
            catch (Exception)
            {
            }
        }

        private void RestoreButtonClick(object sender, RoutedEventArgs e)
        {
            if (NameBox.Text.Length > 0)
            {
                string fileName = $"{NameBox.Text}.rpy";
                string replayDirectory = ReplayFile.GetReplayDirectory(this.GameId);
                try
                {
                    ReplayFileBackupInfo backupInfo = ReplayBackup.GetReplayBackupFileInfo(this.ReplayBackupFilePath);
                    string? sourceReplayDirectory = Path.GetDirectoryName(backupInfo.SourceReplayFilePath);

                    if (sourceReplayDirectory != replayDirectory)
                    {
                        MessageBoxResult result = MessageBox.Show(this,
                            $"リプレイバックアップファイルの作成元ディレクトリと復元先のディレクトリが一致しません。\n" +
                            $"バックアップ作成元: {sourceReplayDirectory}\n" +
                            $"復元先: {replayDirectory}\n" +
                            $"このまま続行してもよろしいですか？",
                            "リプレイファイルの復元",
                            MessageBoxButton.OKCancel, MessageBoxImage.Exclamation);
                        if (result == MessageBoxResult.Cancel)
                            return;
                    }

                    string outputFilepath = Path.Combine(replayDirectory, fileName);
                    if (File.Exists(outputFilepath) &&
                        AllowOverwriteCheckBox.IsChecked == false)
                    {
                        MessageBox.Show(this,
                            $"リプレイファイル '{fileName}' は既に存在します。", "リプレイファイルのバックアップ",
                            MessageBoxButton.OK, MessageBoxImage.Exclamation);
                        return;
                    }

                    ReplayBackup.ExtractBackupFile(this.ReplayBackupFilePath, outputFilepath);
                    MessageBox.Show(this, "復元しました。", "リプレイファイルのバックアップ",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                    this.DialogResult = true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(this, $"リプレイファイルの復元に失敗しました。\n{ex.Message}",
                        "エラー", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show(this, "ファイル名を空にできません。", "リプレイファイルの復元",
                    MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
        }
    }
}
