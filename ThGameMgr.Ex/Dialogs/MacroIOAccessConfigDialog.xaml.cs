using Microsoft.Win32;
using System.Collections.Generic;

namespace ThGameMgr.Ex.Dialogs
{
    /// <summary>
    /// MacroIOAccessConfigDialog.xaml の相互作用ロジック
    /// </summary>
    public partial class MacroIOAccessConfigDialog : Window
    {
        private readonly MacroManager _macroManager;

        public MacroIOAccessConfigDialog(IUserService userService)
        {
            InitializeComponent();

            MacroManager macroManager = new(userService);
            _macroManager = macroManager;

            try
            {
                List<string> accessableDirectories = macroManager.GetMacroIOAccessConfig();
                foreach (string directory in accessableDirectories)
                {
                    DirectoryListBox.Items.Add(directory);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"使用可能ディレクトリの設定を読み込めませんでした．\n{ex.Message}", "エラー",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                this.Close();
            }
        }

        private void AddButtonClick(object sender, RoutedEventArgs e)
        {
            OpenFolderDialog openFolderDialog = new();
            if (openFolderDialog.ShowDialog() == true)
            {
                DirectoryListBox.Items.Add(openFolderDialog.FolderName);
            }
        }

        private void SaveButtonClick(object sender, RoutedEventArgs e)
        {
            List<string> accessableDirectories = new();
            foreach (var item in DirectoryListBox.Items)
            {
                string? directory = item.ToString();
                if (!string.IsNullOrWhiteSpace(directory))
                {
                    accessableDirectories.Add(directory);
                }
            }

            try
            {
                _macroManager.SaveMacroIOAccessConfig(accessableDirectories);
                this.DialogResult = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, $"設定の保存に失敗しました．\n{ex.Message}", "エラー",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
