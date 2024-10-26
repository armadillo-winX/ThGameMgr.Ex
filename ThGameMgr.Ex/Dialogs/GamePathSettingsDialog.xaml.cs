using Microsoft.Win32;
using System.Collections.Generic;
using System.Windows.Controls;

namespace ThGameMgr.Ex.Dialogs
{
    /// <summary>
    /// GamePathSettingsDialog.xaml の相互作用ロジック
    /// </summary>
    public partial class GamePathSettingsDialog : Window
    {
        public GamePathSettingsDialog()
        {
            InitializeComponent();

            List<string> allGamesList = GameIndex.GetAllGamesList();
            foreach (string gameId in allGamesList)
            {
                string gameName = GameIndex.GetGameName(gameId);
                ListBoxItem item = new()
                {
                    Content = $"{gameId}: {gameName}",
                    Uid = gameId
                };
                GamesListBox.Items.Add(item);
            }

            if (GamesListBox.Items.Count > 0)
            {
                GamesListBox.SelectedIndex = 0;
                _ = GamesListBox.Focus();
            }
        }

        private void CloseButtonClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void BrowseButtonClick(object sender, RoutedEventArgs e)
        {
            if (GamesListBox.SelectedIndex >= 0)
            {
                string gameId = ((ListBoxItem)GamesListBox.SelectedItem).Uid;

                string fileFilter;
                if (gameId == "Th06")
                {
                    fileFilter = $"東方紅魔郷.exe;{gameId.ToLower()}*.exe";
                }
                else
                {
                    fileFilter = $"{gameId.ToLower()}*.exe";
                }

                OpenFileDialog openFileDialog = new()
                {
                    Filter = $"{GameIndex.GetGameName(gameId)}実行ファイル|{fileFilter}"
                };

                if (openFileDialog.ShowDialog() == true)
                {
                    string gamePath = openFileDialog.FileName;
                    GamePathBox.Text = gamePath;
                    GameFile.SetGameFilePath(gameId, gamePath);
                }
            }
            else
            {
                MessageBox.Show(this, "ゲームを選択してください。", "ゲームのパス設定",
                    MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
        }

        private void GamesListBoxSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (GamesListBox.SelectedIndex >= 0)
            {
                string gameId = ((ListBoxItem)GamesListBox.SelectedItem).Uid;

                string gamePath = GameFile.GetGameFilePath(gameId);
                GamePathBox.Text = gamePath;
            }
        }
    }
}
