namespace ThGameMgr.Ex.Dialogs
{
    /// <summary>
    /// ScoreRecordDetailDialog.xaml の相互作用ロジック
    /// </summary>
    public partial class ScoreRecordDetailDialog : Window
    {
        public ScoreRecordDetailDialog()
        {
            InitializeComponent();
        }

        private void CloseButtonClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
