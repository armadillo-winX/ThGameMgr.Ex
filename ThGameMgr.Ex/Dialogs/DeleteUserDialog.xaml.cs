using System.Collections.Generic;
using System.Windows.Controls;

namespace ThGameMgr.Ex.Dialogs
{
    /// <summary>
    /// DeleteUserDialog.xaml の相互作用ロジック
    /// </summary>
    public partial class DeleteUserDialog : Window
    {
        public DeleteUserDialog()
        {
            InitializeComponent();

            string? usersIndexFile = PathInfo.UsersIndexFile;

            if (File.Exists(usersIndexFile))
            {
                try
                {
                    List<string>? usersList = User.GetUsersList();
                    if (usersList != null && usersList.Count > 0)
                    {
                        foreach (string userName in usersList)
                        {
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

        private void CancelButtonClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void DeleteButtonClick(object sender, RoutedEventArgs e)
        {
            try
            {
                ListBoxItem? selectedItem = UsersListBox.SelectedItem as ListBoxItem;
                if (selectedItem != null)
                {
                    string? userName = selectedItem.Content.ToString();

                    MessageBoxResult result = MessageBox.Show(
                        $"ユーザー '{userName}' を削除してもよろしいですか。", "ユーザーの削除",
                        MessageBoxButton.YesNo, MessageBoxImage.Information);
                    if (result == MessageBoxResult.Yes &&
                        !string.IsNullOrEmpty(userName))
                    {
                        User.Delete(userName);
                        UsersListBox.Items.Remove(selectedItem);
                    }
                }
                else
                {
                    MessageBox.Show(this, "削除するユーザーを選択してください。", "ユーザーの削除",
                        MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, $"ユーザーの削除に失敗しました。\n{ex.Message}", "エラー",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
