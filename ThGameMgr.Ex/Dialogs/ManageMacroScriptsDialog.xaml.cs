namespace ThGameMgr.Ex.Dialogs
{
    /// <summary>
    /// ManageMacroScriptsDialog.xaml の相互作用ロジック
    /// </summary>
    public partial class ManageMacroScriptsDialog : Window
    {
        private readonly IUserService _userService;

        private readonly MacroManager _macroManager;

        public ManageMacroScriptsDialog(IUserService userService)
        {
            InitializeComponent();

            _userService = userService;
            MacroManager macroManager = new(userService);
            _macroManager = macroManager;

            string vfsFilePath = userService.GetCurrentUserMacroVaultArchiveFilePath();
            if (!File.Exists(vfsFilePath))
            {
                try
                {
                    macroManager.CreateVfs();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Failed to create vfs archive: {ex.Message}");
                }
            }
            else
            {
                LoadScriptList();
            }
        }

        private void LoadScriptList()
        {
            MacroScriptsFileListBox.Items.Clear();
            try
            {
                string[] scriptList = _macroManager.GetScriptList();
                foreach (string script in scriptList)
                {
                    MacroScriptsFileListBox.Items.Add(script);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        private void CreateButtonClick(object sender, RoutedEventArgs e)
        {
            MacroScriptNameDialog macroScriptNameDialog = new()
            {
                Owner = this
            };

            if (macroScriptNameDialog.ShowDialog() == true)
            {
                string scriptName = macroScriptNameDialog.ScriptName;
                try
                {
                    _macroManager.AddScript("writeLine(\"Hello World!\");", "", scriptName);

                    MacroScriptEditorDialog macroScriptEditorDialog = new(
                        _userService, scriptName
                        )
                    {
                        Owner = this
                    };

                    LoadScriptList();
                    macroScriptEditorDialog.Show();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(
                        this, $"スクリプトファイルを追加できませんでした．\n{ex.Message}", "エラー",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void EditButtonClick(object sender, RoutedEventArgs e)
        {
            string? path = MacroScriptsFileListBox.SelectedItem as string;
            if (!string.IsNullOrWhiteSpace(path))
            {
                MacroScriptEditorDialog macroScriptEditorDialog = new(
                    _userService, path
                    )
                {
                    Owner = this
                };
                macroScriptEditorDialog.Show();
            }
            else
            {
                MessageBox.Show(this,
                    "スクリプトを選択してください．", "スクリプトの編集", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
        }

        private void DeleteButtonClick(object sender, RoutedEventArgs e)
        {
            string? path = MacroScriptsFileListBox.SelectedItem as string;
            if (!string.IsNullOrEmpty(path))
            {
                try
                {
                    _macroManager.DeleteScript(path);
                    LoadScriptList();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(this, $"スクリプトの削除に失敗しました．\n{ex.Message}", "エラー",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show(this,
                    "スクリプトを選択してください．", "スクリプトの削除", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
        }
    }
}
