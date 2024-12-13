using System.Collections.Generic;

namespace ThGameMgr.Ex.Dialogs
{
    /// <summary>
    /// EditLinksListDialog.xaml の相互作用ロジック
    /// </summary>
    public partial class EditLinksListDialog : Window
    {
        public EditLinksListDialog()
        {
            InitializeComponent();

            GetLinkLists();
        }

        private void GetLinkLists()
        {
            LinkListsDataGrid.Items.Clear();
            try
            {
                string linksListFile = $"{User.CurrentUserDirectoryPath}\\links.txt";
                if (!File.Exists(linksListFile))
                    File.Copy(PathInfo.DefaultLinksListFile, linksListFile);

                IEnumerable<string> lines = File.ReadLines(linksListFile);
                foreach (string line in lines)
                {
                    string[] strings = line.Split('|');
                    string name = strings[0];
                    string url = strings[1];

                    LinkListsDataGrid.Items.Add(new LinkData
                    {
                        Name = name,
                        Url = url,
                    });
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"リンクのリストの取得に失敗しました。\n{ex.Message}", "エラー",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                this.Close();
            }
        }

        private void AddButtonClick(object sender, RoutedEventArgs e)
        {
            AddLinkDialog addLinkDialog = new()
            {
                Owner = this
            };

            if (addLinkDialog.ShowDialog() == true)
            {
                if (addLinkDialog.LinkData != null)
                {
                    LinkListsDataGrid.Items.Add(addLinkDialog.LinkData);
                }
            }
        }

        private void RemoveButtonClick(object sender, RoutedEventArgs e)
        {
            if (LinkListsDataGrid.SelectedIndex > -1)
            {
                LinkListsDataGrid.Items.Remove(LinkListsDataGrid.SelectedItem);
            }
        }

        private void SaveButtonClick(object sender, RoutedEventArgs e)
        {
            string data = "";

            try
            {
                string linksListFile = $"{User.CurrentUserDirectoryPath}\\links.txt";

                if (LinkListsDataGrid.Items.Count > 0)
                {
                    for (int i = 0; i < LinkListsDataGrid.Items.Count; i++)
                    {
                        LinkData linkData = LinkListsDataGrid.Items[i] as LinkData;
                        data += $"{linkData.Name}|{linkData.Url}\r\n";
                    }
                }

                StreamWriter streamWriter = new(linksListFile, false);
                streamWriter.Write(data);
                streamWriter.Close();

                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, $"リンクリストの保存に失敗しました。\n{ex.Message}", "エラー",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                this.Close();
            }
        }
    }
}
