using System.Windows.Controls;

namespace ThGameMgr.Ex.Dialogs
{
    /// <summary>
    /// AddLinkDialog.xaml の相互作用ロジック
    /// </summary>
    public partial class AddLinkDialog : Window
    {
        public LinkData? LinkData { get; set; }

        public AddLinkDialog()
        {
            InitializeComponent();

            AttentionBlock.Text = "";
        }

        private void NameBoxTextChanged(object sender, TextChangedEventArgs e)
        {
            if (NameBox.Text.Contains('|'))
            {
                AttentionBlock.Text = "リンクの名前に '|' を含むことはできません。";
                AddButton.IsEnabled = false;
            }
            else
            {
                AttentionBlock.Text = "";
                AddButton.IsEnabled = true;
            }
        }

        private void AddButtonClick(object sender, RoutedEventArgs e)
        {
            if (NameBox.Text.Length > 0 && UrlBox.Text.Length > 0)
            {
                this.LinkData = new()
                {
                    Name = NameBox.Text,
                    Url = UrlBox.Text
                };
                this.DialogResult = true;
            }
            else
            {
                MessageBox.Show(this,
                    "名前またはURLが空です。", "リンクの追加",
                    MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
        }
    }
}
