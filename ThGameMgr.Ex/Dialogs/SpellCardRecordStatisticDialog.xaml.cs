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

        public string? GetCardRate
        {
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    GetCardRateBlock.Text = value;
                }
                else
                {
                    GetCardRateBlock.Text = "0.00%";
                }
            }
        }

        public int TotalChallengeCount
        {
            set
            {
                TotalChallengeCountBlock.Text = value.ToString();
            }
        }

        public int TotalGetCount
        {
            set
            {
                TotalGetCountBlock.Text = value.ToString();
            }
        }

        public string? TotalGetRate
        {
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    TotalGetRateBlock.Text = value;
                }
                else
                {
                    TotalGetRateBlock.Text = "0.00%";
                }
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
