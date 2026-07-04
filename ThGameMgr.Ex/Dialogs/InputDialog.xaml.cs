namespace ThGameMgr.Ex.Dialogs
{
    /// <summary>
    /// InputDialog.xaml の相互作用ロジック
    /// </summary>
    public partial class InputDialog : Window
    {
        public string InputText { get; set; }

        public InputDialog(string title)
        {
            InitializeComponent();

            this.InputText = string.Empty;
            this.Title = title;
        }

        private void OKButtonClick(object sender, RoutedEventArgs e)
        {
            this.InputText = InputBox.Text;
            this.DialogResult = true;
        }

        private void CancelButtonClick(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }
    }
}
