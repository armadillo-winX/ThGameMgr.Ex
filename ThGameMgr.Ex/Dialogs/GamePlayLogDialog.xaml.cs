using System.Collections.ObjectModel;

namespace ThGameMgr.Ex.Dialogs
{
    /// <summary>
    /// GamePlayLogDialog.xaml の相互作用ロジック
    /// </summary>
    public partial class GamePlayLogDialog : Window
    {
        private readonly IUserService _currentUserService;

        public GamePlayLogDialog(IUserService userService)
        {
            InitializeComponent();
            _currentUserService = userService;

            try
            {
                ViewGamePlayLogData();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"ゲーム実行履歴の取得に失敗しました。\n{ex.Message}",
                    "エラー",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ViewGamePlayLogData()
        {
            string gamePlayLogRecordFile = _currentUserService.GetCurrentUserGamePlayLogRecordFilePath();
            if (File.Exists(gamePlayLogRecordFile))
            {
                ObservableCollection<GamePlayLogData> gamePlayLogDataCollection = [];
                GamePlayLogRecorder gamePlayLogRecorder = new(_currentUserService);
                gamePlayLogDataCollection = gamePlayLogRecorder.GetGamePlayLogDataCollection();
                GameLogDataGrid.AutoGenerateColumns = false;

                for (int i = gamePlayLogDataCollection.Count - 1; i >= 0; i--)
                {
                    GameLogDataGrid.Items.Add(gamePlayLogDataCollection[i]);
                }
            }
        }
    }
}
