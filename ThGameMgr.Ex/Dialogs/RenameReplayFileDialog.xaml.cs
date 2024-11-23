namespace ThGameMgr.Ex.Dialogs
{
    /// <summary>
    /// RenameReplayFileDialog.xaml の相互作用ロジック
    /// </summary>
    public partial class RenameReplayFileDialog : Window
    {
        private string? _replayFileName;

        public string? ReplayFileName
        {
            get
            {
                return _replayFileName;
            }

            set
            {
                _replayFileName = value;
                FileNameBox.Text = value;
            }
        }

        public RenameReplayFileDialog()
        {
            InitializeComponent();
        }

        private void WindowLoaded(object sender, RoutedEventArgs e)
        {
            _ = FileNameBox.Focus();
        }

        private void RenameButtonClick(object sender, RoutedEventArgs e)
        {
            if (FileNameBox.Text.Length > 0)
            {
                this.ReplayFileName = FileNameBox.Text;
                this.DialogResult = true;
            }
            else
            {
                MessageBox.Show(this, "ファイル名を空にはできません。", "リプレイファイルのリネーム",
                    MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
        }
    }
}
