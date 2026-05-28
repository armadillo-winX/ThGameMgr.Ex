using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;

namespace ThGameMgr.Ex.Dialogs
{
    /// <summary>
    /// ManageReplayFileBackupDialog.xaml の相互作用ロジック
    /// </summary>
    public partial class ManageReplayFileBackupDialog : Window
    {
        private readonly IUserService _currentUserService;

        private string GameId { get; set; }

        private string[] ReplayBackupFiles { get; set; }

        public ManageReplayFileBackupDialog(
            IUserService userService,
            string gameId
            )
        {
            InitializeComponent();

            _currentUserService = userService;
            this.GameId = gameId;
            this.ReplayBackupFiles = [];

            DescriptionBlock.Text = $"{GameIndex.GetGameName(gameId)} のリプレイファイルバックアップ";

            string replayBackupDirectory = Path.Combine(userService.GetCurrentUserReplayBackupDirectory(), gameId);
            if (Directory.Exists(replayBackupDirectory))
            {
                try
                {
                    string[] backupFiles =
                        Directory.GetFiles(replayBackupDirectory, "*.trpb", SearchOption.TopDirectoryOnly);
                    this.ReplayBackupFiles = backupFiles;
                    foreach (string backupFile in backupFiles)
                    {
                        ReplayFileBackupInfo replayFileBackupInfo =
                            ReplayBackup.GetReplayBackupFileInfo(backupFile);
                        if (replayFileBackupInfo.GameId == this.GameId)
                            BackupFilesDataGrid.Items.Add(replayFileBackupInfo);
                    }
                }
                catch (Exception)
                {
                    ShowErrorMessage("リプレイバックアップファイル一覧の取得に失敗しました。");
                }
            }
        }

        private void ShowErrorMessage(string message)
        {
            System.Media.SystemSounds.Hand.Play();

            ErrorImage.Source =
                    new BitmapImage(
                        new Uri("pack://application:,,,/ThGameMgr.Ex;component/Images/ErrorIcon32x32.png")
                    );
            MessageBlock.Text = message;
            RestoreButton.IsEnabled = false;
            BackupFilesDataGrid.IsEnabled = false;
            BlurEffect blurEffect = new()
            {
                Radius = 7,
                KernelType = KernelType.Gaussian
            };

            BackupFilesDataGrid.Effect = blurEffect;
        }

        private void RestoreButtonClick(object sender, RoutedEventArgs e)
        {
            if (BackupFilesDataGrid.SelectedIndex > -1 &&
                BackupFilesDataGrid.SelectedIndex < this.ReplayBackupFiles.Length)
            {
                int index = BackupFilesDataGrid.SelectedIndex;
                string replayBackupFile = this.ReplayBackupFiles[index];
                RestoreReplayFileDialog restoreReplayFileDialog = new(
                    this.GameId, replayBackupFile
                    )
                {
                    Owner = this
                };
                restoreReplayFileDialog.ShowDialog();
            }
        }
    }
}
