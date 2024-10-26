namespace ThGameMgr.Ex.Dialogs
{
    /// <summary>
    /// AddUserDialog.xaml の相互作用ロジック
    /// </summary>
    public partial class AddUserDialog : Window
    {
        public AddUserDialog()
        {
            InitializeComponent();

            _ = UserNameBox.Focus();
        }

        private void AddButtonClick(object sender, RoutedEventArgs e)
        {
            if (UserNameBox.Text.Length > 0)
            {
                string userName = UserNameBox.Text;
                if (User.Exists(userName))
                {
                    MessageBox.Show(this, $"ユーザー '{userName}' は既に存在します。\n別のユーザー名を指定してください。",
                        "ユーザーの追加",
                        MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
                else
                {
                    try
                    {
                        User.Add(userName);
                        User.Switch(userName);
                        this.DialogResult = true;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(this, $"ユーザーの追加と切り替えに失敗しました。\n{ex.Message}", "エラー",
                            MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show(this, "追加するユーザー名を入力してください。", "ユーザーの追加",
                    MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
        }
    }
}
