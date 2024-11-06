using System.Collections.Generic;

namespace ThGameMgr.Ex.Dialogs
{
    /// <summary>
    /// SelectThpracDialog.xaml の相互作用ロジック
    /// </summary>
    public partial class SelectThpracDialog : Window
    {
        public List<string> ThpracFiles
        {
            set
            {
                foreach (string thpracFile in value)
                {
                    ThpracFilesListBox.Items.Add(thpracFile);
                }
            }
        }

        public string? SelectedThpracFile { get; set; }

        public SelectThpracDialog()
        {
            InitializeComponent();
        }

        private void OKButtonClick(object sender, RoutedEventArgs e)
        {
            if (ThpracFilesListBox.SelectedIndex > -1)
            {
                this.SelectedThpracFile = ThpracFilesListBox.SelectedItem as string;
                this.DialogResult = true;
            }
            else
            {
                MessageBox.Show(this, "thprac 実行ファイルを選択してください。", "thprac 選択",
                    MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
        }
    }
}
