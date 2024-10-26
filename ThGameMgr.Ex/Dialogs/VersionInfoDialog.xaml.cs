namespace ThGameMgr.Ex.Dialogs
{
    /// <summary>
    /// VersionInfoDialog.xaml の相互作用ロジック
    /// </summary>
    public partial class VersionInfoDialog : Window
    {
        public VersionInfoDialog()
        {
            InitializeComponent();

            AppNameBlock.Text = $"{VersionInfo.AppName} {VersionInfo.CodeName}";
            AppVersionBlock.Text = $"ver.{VersionInfo.AppVersion}";
            DeveloperBlock.Text = $"by {VersionInfo.Developer}";
            SystemBlock.Text = VersionInfo.OSVersion;
            RuntimeBlock.Text = VersionInfo.DotNetVersion;
        }

        private void OKButtonClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
