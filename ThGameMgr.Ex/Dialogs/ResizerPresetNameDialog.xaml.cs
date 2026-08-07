namespace ThGameMgr.Ex.Dialogs
{
    /// <summary>
    /// ResizerPresetNameDialog.xaml の相互作用ロジック
    /// </summary>
    public partial class ResizerPresetNameDialog : Window
    {
        public string PresetName { get; set; }

        public ResizerPresetNameDialog(string? defaultName)
        {
            InitializeComponent();

            NameBox.Text = defaultName;
            _ = NameBox.Focus();

            this.PresetName = defaultName != null ? defaultName : string.Empty;
        }

        private void OKButtonClick(object sender, RoutedEventArgs e)
        {
            string name = NameBox.Text;
            if (!string.IsNullOrWhiteSpace(name))
            {
                this.PresetName = name;
                this.DialogResult = true;
            }
            else
            {
                MessageBox.Show(this,
                    "プリセット名を入力してください．", "リサイザプリセットを保存",
                    MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
        }
    }
}
