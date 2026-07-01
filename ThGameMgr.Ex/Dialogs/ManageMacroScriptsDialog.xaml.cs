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
        }

        private string GenerateVfsEntropyName(string userDirName)
        {
            string guid = Guid.NewGuid().ToString("N");
            string entropyName = $"ThGameMgr.Ex.{userDirName}.{guid}";

            return entropyName;
        }
    }
}
