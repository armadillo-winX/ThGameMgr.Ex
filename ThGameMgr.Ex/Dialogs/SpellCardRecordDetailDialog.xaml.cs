namespace ThGameMgr.Ex.Dialogs
{
    /// <summary>
    /// SpellCardRecordDetailDialog.xaml の相互作用ロジック
    /// </summary>
    public partial class SpellCardRecordDetailDialog : Window
    {
        public SpellCardRecordDetailDialog()
        {
            InitializeComponent();
        }

        private void CloseButtonClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
