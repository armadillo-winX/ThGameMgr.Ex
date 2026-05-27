namespace ThGameMgr.Ex.Dialogs
{
    /// <summary>
    /// ManageReplayFileBackupDialog.xaml の相互作用ロジック
    /// </summary>
    public partial class ManageReplayFileBackupDialog : Window
    {
        private readonly IUserService _currentUserService;

        private string GameId { get; set; }

        public ManageReplayFileBackupDialog(
            IUserService userService,
            string gameId
            )
        {
            InitializeComponent();

            _currentUserService = userService;
            this.GameId = gameId;

            DescriptionBlock.Text = $"{GameIndex.GetGameName(gameId)} のリプレイファイルバックアップ";

            string replayBackupDirectory = Path.Combine(userService.GetCurrentUserReplayBackupDirectory(), gameId);
            if (Directory.Exists(replayBackupDirectory))
            {
                string[] backupFiles =
                Directory.GetFiles(replayBackupDirectory, "*.trpb", SearchOption.TopDirectoryOnly);
                foreach (string backupFile in backupFiles)
                {
                    ReplayFileBackupInfo replayFileBackupInfo =
                        ReplayBackup.GetReplayBackupFileInfo(backupFile);
                    if (replayFileBackupInfo.GameId == this.GameId)
                        BackupFilesDataGrid.Items.Add(replayFileBackupInfo);
                }
            }
        }
    }
}
