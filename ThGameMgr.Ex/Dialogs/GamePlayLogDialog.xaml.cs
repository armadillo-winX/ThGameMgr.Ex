using System.Collections.ObjectModel;

namespace ThGameMgr.Ex.Dialogs
{
    /// <summary>
    /// GamePlayLogDialog.xaml の相互作用ロジック
    /// </summary>
    public partial class GamePlayLogDialog : Window
    {
        public GamePlayLogDialog()
        {
            InitializeComponent();

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
            string gamePlayLogRecordFile = $"{User.CurrentUserDirectoryPath}\\GamePlayLog.xml";
            if (File.Exists(gamePlayLogRecordFile))
            {
                ObservableCollection<GamePlayLogData> gamePlayLogDataCollection = [];
                gamePlayLogDataCollection = GamePlayLogRecorder.GetGamePlayLogDataCollection();
                GameLogDataGrid.AutoGenerateColumns = false;
                GameLogDataGrid.DataContext = gamePlayLogDataCollection;
            }
        }
    }
}
