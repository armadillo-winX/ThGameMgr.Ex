using Microsoft.Win32;
using System.Windows.Controls;

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

            this.GameId = ScoreData.GameId;
            GameNameBlock.Text = GameIndex.GetGameName(this.GameId);

            LevelFilterComboBox.SelectedIndex = 0;
            if (this.GameId == GameIndex.Th07)
                LevelFilterComboBox.Items.Add(new ComboBoxItem() { Content = "Phantasm"});

            PlayerFilterComboBox.Items.Add(new ComboBoxItem() { Content = "All" });
            string[] players = GamePlayers.GetGamePlayers(this.GameId).Split(',');
            foreach (string player in players)
            {
                PlayerFilterComboBox.Items.Add(new ComboBoxItem() { Content = player });
            }

            PlayerFilterComboBox.SelectedIndex = 0;
        }

        private void ExportButtonClick(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(this.GameId))
            {
                if (PlayerFilterComboBox.SelectedIndex >= 0 && LevelFilterComboBox.SelectedIndex >= 0)
                {
                    ComboBoxItem selectedPlayerItem = PlayerFilterComboBox.SelectedItem as ComboBoxItem;
                    ComboBoxItem selectedLevelItem = LevelFilterComboBox.SelectedItem as ComboBoxItem;

                    ScoreFilter scoreFilter = new()
                    {
                        Player = selectedPlayerItem.Content.ToString(),
                        Level = selectedLevelItem.Content.ToString()
                    };

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
                            ScoreData.ExportToTextFile(
                                outputPath, OutputUnTriedCardDataCheckBox.IsChecked == true, scoreFilter, CommentBox.Text
                                );

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
                    MessageBox.Show(this, "出力する自機または難易度が指定されていません。", "スコアデータをエクスポート",
                        MessageBoxButton.OK, MessageBoxImage.Exclamation);
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
