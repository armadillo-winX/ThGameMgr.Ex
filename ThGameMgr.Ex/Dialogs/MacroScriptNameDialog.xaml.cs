namespace ThGameMgr.Ex.Dialogs
{
    /// <summary>
    /// MacroScriptNameDialog.xaml の相互作用ロジック
    /// </summary>
    public partial class MacroScriptNameDialog : Window
    {
        public string ScriptName { get; set; }

        public MacroScriptNameDialog()
        {
            InitializeComponent();
            _ = NameBox.Focus();

            this.ScriptName = string.Empty;
        }

        private void OKButtonClick(object sender, RoutedEventArgs e)
        {
            this.ScriptName = NameBox.Text + ".masis";
            this.DialogResult = true;
        }
    }
}
