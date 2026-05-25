using System.Collections.Generic;
using System.Windows.Controls;

namespace ThGameMgr.Ex.Dialogs
{
    /// <summary>
    /// DefaultGameSettingsDialog.xaml の相互作用ロジック
    /// </summary>
    public partial class DefaultGameSettingsDialog : Window
    {
        private readonly IUserService _currentUserService;

        public DefaultGameSettingsDialog(IUserService userService)
        {
            InitializeComponent();
            _currentUserService = userService;

            List<string> enableGamesList = GameIndex.GetEnabledGamesList();

            ComboBoxItem defaultItem = new()
            {
                Content = "前回終了時に選択されていたゲーム",
                Uid = "OnExit"
            };

            DefaultGameConfigComboBox.Items.Add(defaultItem);

            if (enableGamesList.Count > 0)
            {
                Separator separator = new();
                DefaultGameConfigComboBox.Items.Add(separator);

                foreach (string gameId in enableGamesList)
                {
                    ComboBoxItem item = new()
                    {
                        Content = $"{gameId}: {GameIndex.GetGameName(gameId)}",
                        Uid = gameId
                    };
                    DefaultGameConfigComboBox.Items.Add(item);
                }
            }

            string defaultGameSettings;
            try
            {
                SettingsConfigurator settingsConfigurator = new(userService);
                defaultGameSettings = settingsConfigurator.ConfigureDefaultGameSettings();
            }
            catch (Exception)
            {
                defaultGameSettings = string.Empty;
            }

            if (enableGamesList.Contains(defaultGameSettings))
            {
                int index = enableGamesList.IndexOf(defaultGameSettings);
                DefaultGameConfigComboBox.SelectedIndex = index + 2;
            }
            else
            {
                DefaultGameConfigComboBox.SelectedIndex = 0;
            }
        }

        private void OKButtonClick(object sender, RoutedEventArgs e)
        {
            ComboBoxItem? selectedItem = DefaultGameConfigComboBox.SelectedItem as ComboBoxItem;
            if (selectedItem != null)
            {
                string defaultGameId = (string)selectedItem.Uid;
                try
                {
                    SettingsConfigurator.SaveDefaultGameSettings(defaultGameId);
                    this.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(this, $"設定の保存に失敗しました。\n{ex.Message}", "エラー",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                    this.Close();
                }
            }
            else
            {
                MessageBox.Show(this, "既定にするゲームを選択してください。", "既定のゲームを設定する",
                    MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
        }
    }
}
