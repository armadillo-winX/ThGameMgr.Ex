namespace ThGameMgr.Ex.Dialogs
{
    /// <summary>
    /// CreateReplayFileBackupDialog.xaml の相互作用ロジック
    /// </summary>
    public partial class CreateReplayFileBackupDialog : Window
    {
        private readonly IUserService _currentUserService;

        public string GameId { get; set; }

        public string ReplayFilePath { get; set; }

        public CreateReplayFileBackupDialog(
            IUserService userService,
            string gameId,
            string replayFilePath)
        {
            InitializeComponent();

            _currentUserService = userService;
            this.GameId = gameId;
            this.ReplayFilePath = replayFilePath;

            string replayFileName = Path.GetFileName(replayFilePath);
            NameBox.Text = $"{replayFileName} のバックアップ";
        }

        private void CreateBackupButtonClick(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(NameBox.Text))
            {
                string backupName = NameBox.Text;
                string backupRootDirectory = _currentUserService.GetCurrentUserReplayBackupDirectory();
                string backupDirectory = Path.Combine(backupRootDirectory, this.GameId);
                DateTime now = DateTime.Now;
                ReplayFileBackupInfo replayFileBackupInfo = new()
                {
                    GameId = this.GameId,
                    GameName = GameNameIndex.GetGameNameFromId(this.GameId),
                    SourceReplayFilePath = this.ReplayFilePath,
                    BackupName = backupName,
                    Timestamp = now.ToString("yyyy/MM/dd hh:mm:s"),
                    Comment = CommentBox.Text,
                    ApplicationName = VersionInfo.AppName
                };

                try
                {
                    ReplayBackup.MakeReplayBackupFile(
                        now.ToString("yyyy-MM-dd_hh-mm-ss-fffffff"),
                        replayFileBackupInfo,
                        backupDirectory
                        );
                    this.DialogResult = true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(this, $"バックアップの作成に失敗しました。\n{ex.Message}",
                        "エラー", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show(this, "バックアップの名前は空にできません。", "リプレイファイルのバックアップ",
                    MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
        }
    }
}
