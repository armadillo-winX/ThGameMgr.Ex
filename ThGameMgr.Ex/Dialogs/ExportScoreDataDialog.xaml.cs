using Microsoft.Win32;

namespace ThGameMgr.Ex.Dialogs
{
    /// <summary>
    /// ExportScoreDataDialog.xaml の相互作用ロジック
    /// </summary>
    public partial class ExportScoreDataDialog : Window
    {
        private string? GameId { get; set; }

        public ExportScoreDataDialog()
        {
            InitializeComponent();
        }

        private void ExportButtonClick(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(this.GameId))
            {
                SaveFileDialog saveFileDialog = new()
                {
                    FileName = $"{GameIndex.GetGameName(this.GameId)}スコアデータ.txt",
                    Filter = "テキストファイル|*.txt|すべてのファイル|*.*"
                };

                if (saveFileDialog.ShowDialog() == true)
                {
                    string outputPath = saveFileDialog.FileName;
                    try
                    {
                        ScoreData.ExportToTextFile(outputPath);
                        MessageBox.Show(this, $"エクスポートしました。", "スコアデータをエクスポート",
                            MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(this, $"エクスポートに失敗しました。\n{ex.Message}", "エラー",
                            MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show(this, "出力するゲームの種類が不明です。", "スコアデータのエクスポート",
                    MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
        }
    }
}
