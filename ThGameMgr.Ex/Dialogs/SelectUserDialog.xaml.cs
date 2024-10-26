using System.Windows.Controls;
using System.Xml;

namespace ThGameMgr.Ex.Dialogs
{
    /// <summary>
    /// SelectUserDialog.xaml の相互作用ロジック
    /// </summary>
    public partial class SelectUserDialog : Window
    {
        public SelectUserDialog()
        {
            InitializeComponent();

            string? usersIndexFile = PathInfo.UsersIndexFile;

            if (File.Exists(usersIndexFile))
            {
                try
                {
                    XmlDocument usersIndexDocument = new();
                    usersIndexDocument.Load(usersIndexFile);
                    XmlNodeList userNodeList = usersIndexDocument.SelectNodes("UsersIndex/User");
                    if (userNodeList.Count > 0)
                    {
                        foreach (XmlNode userNode in userNodeList)
                        {
                            string userName = userNode.SelectSingleNode("Name").InnerText;
                            
                            ListBoxItem userItem = new()
                            {
                                Content = userName,
                                IsEnabled = userName != User.CurrentUserName
                            };
                            _ = UsersListBox.Items.Add(userItem);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"ユーザー一覧の読み取りに失敗しました。\n{ex.Message}", "エラー",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void OKButtonClick(object sender, RoutedEventArgs e)
        {
            try
            {
                if (UsersListBox.SelectedIndex > -1)
                {
                    ListBoxItem selectedItem = UsersListBox.SelectedItem as ListBoxItem;
                    string userName = selectedItem.Content.ToString();
                    User.Switch(userName);
                    this.DialogResult = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, $"ユーザーの切り替えに失敗しました。{ex.Message}", "エラー",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
