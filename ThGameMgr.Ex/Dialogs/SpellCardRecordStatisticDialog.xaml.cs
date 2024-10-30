namespace ThGameMgr.Ex.Dialogs
{
    /// <summary>
    /// SpellCardStatisticDialog.xaml の相互作用ロジック
    /// </summary>
    public partial class SpellCardRecordStatisticDialog : Window
    {
        public string? GameId
        {
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    GameNameBlock.Text = GameIndex.GetGameName(value);
                }
                else
                {
                    GameNameBlock.Text = string.Empty;
                }
            }
        }

        public int ChallengedCardCount
        {
            set
            {
                ChallengedCardCountBlock.Text = value.ToString();
            }
        }

        public int GetCardCount
        {
            set
            {
                GetCardCountBlock.Text = value.ToString();
            }
        }

        public string GetCardRate
        {
            set
            {
                GetCardRateBlock.Text = value;
            }
        }

        public SpellCardRecordStatisticDialog()
        {
            InitializeComponent();
        }

        private void CloseButtonClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
