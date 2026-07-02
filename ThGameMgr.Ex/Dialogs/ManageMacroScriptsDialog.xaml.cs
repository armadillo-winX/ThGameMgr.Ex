namespace ThGameMgr.Ex.Dialogs
{
    /// <summary>
    /// ManageMacroScriptsDialog.xaml の相互作用ロジック
    /// </summary>
    public partial class ManageMacroScriptsDialog : Window
    {
        public ManageMacroScriptsDialog(IUserService userService)
        {
            InitializeComponent();

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
    }
}
