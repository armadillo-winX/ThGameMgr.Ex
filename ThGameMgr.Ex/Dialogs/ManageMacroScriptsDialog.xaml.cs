namespace ThGameMgr.Ex.Dialogs
{
    /// <summary>
    /// ManageMacroScriptsDialog.xaml の相互作用ロジック
    /// </summary>
    public partial class ManageMacroScriptsDialog : Window
    {
        private readonly IUserService _userService;

        public ManageMacroScriptsDialog(IUserService userService)
        {
            InitializeComponent();

            _userService = userService;

            string vfsFilePath = userService.GetCurrentUserMacroVaultArchiveFilePath();
            if (!File.Exists(vfsFilePath))
            {
                try
                {
                    MacroManager macroManager = new(userService);
                    macroManager.CreateVfs();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Failed to create vfs archive: {ex.Message}");
                }
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
                    MacroManager macroManager = new(_userService);
                    macroManager.AddScript("", "", scriptName);

                    MacroScriptEditorDialog macroScriptEditorDialog = new(
                        _userService, scriptName
                        )
                    {
                        Owner = this
                    };

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
    }
}
