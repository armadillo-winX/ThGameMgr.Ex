using Microsoft.Win32;

namespace ThGameMgr.Ex.Dialogs
{
    /// <summary>
    /// AddExternalToolDialog.xaml の相互作用ロジック
    /// </summary>
    public partial class AddExternalToolDialog : Window
    {
        public AddExternalToolDialog()
        {
            InitializeComponent();

            _ = ToolPathBox.Focus();
        }

        private void BrowseButtonClick(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new()
            {
                Filter = $"すべての実行ファイル|*.exe;*.bat;*.cmd;*.vbs",
                FileName = ""
            };
            if (openFileDialog.ShowDialog(this) == true)
            {
                string toolPath = openFileDialog.FileName;
                FileVersionInfo fileVersionInfo = FileVersionInfo.GetVersionInfo(toolPath);
                string productName = fileVersionInfo.ProductName;
                if (ToolNameBox.Text.Length == 0 &&
                    !string.IsNullOrEmpty(productName))
                    ToolNameBox.Text = productName;
                ToolPathBox.Text = toolPath;
            }
        }

        private void AddButtonClick(object sender, RoutedEventArgs e)
        {
            try
            {
                string toolName = ToolNameBox.Text;
                string toolPath = ToolPathBox.Text;
                string option = OptionBox.Text;
                bool asAdmin = AsAdminCheckBox.IsChecked == true;
                if (toolName.Length > 0 && toolPath.Length > 0)
                {
                    bool result = ExternalTool.AddExternalTool(toolName, toolPath, option, asAdmin);
                    if (result)
                    {
                        this.DialogResult = true;
                    }
                    else
                    {
                        MessageBox.Show(this, $"外部ツール '{toolName}' は既に存在します。",
                            "外部ツールの追加",
                            MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, "エラー", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
