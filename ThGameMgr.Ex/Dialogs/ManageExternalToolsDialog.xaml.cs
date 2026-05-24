using System.Windows.Controls;
using System.Xml;

namespace ThGameMgr.Ex.Dialogs
{
    /// <summary>
    /// ManageExternalToolsDialog.xaml の相互作用ロジック
    /// </summary>
    public partial class ManageExternalToolsDialog : Window
    {
        public ManageExternalToolsDialog()
        {
            InitializeComponent();

            if (!File.Exists($"{UserConfigurator.CurrentUserDirectoryPath}\\Settings\\ExternalTools.xml"))
            {
                try
                {
                    ExternalTool.CreateExternalConfigFile();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(
                        $"外部ツールの一覧の取得に失敗しました。\n{ex.Message}",
                        "エラー",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }

            try
            {
                GetExternalTools();
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, "エラー",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void GetExternalTools()
        {
            ExternalToolsListBox.Items.Clear();

            string exToolsConfig = $"{UserConfigurator.CurrentUserDirectoryPath}\\Settings\\ExternalTools.xml";
            if (File.Exists(exToolsConfig))
            {
                XmlDocument exToolsConfigXml = new();
                exToolsConfigXml.Load(exToolsConfig);
                XmlNodeList? exToolsNodeList = exToolsConfigXml.SelectNodes("ExternalTools/ExternalTool");
                if (exToolsNodeList != null &&
                    exToolsNodeList.Count > 0)
                {
                    foreach (XmlNode toolNode in exToolsNodeList)
                    {
                        XmlNode? nameNode = toolNode.SelectSingleNode("Name");
                        if (nameNode != null)
                        {
                            string name = nameNode.InnerText;
                            ListBoxItem item = new()
                            {
                                Content = name,
                            };
                            ExternalToolsListBox.Items.Add(item);
                        }
                    }
                }
            }
        }

        private void AddButtonClick(object sender, RoutedEventArgs e)
        {
            try
            {
                AddExternalToolDialog addExternalToolDialog = new()
                {
                    Owner = this
                };
                if (addExternalToolDialog.ShowDialog() == true)
                {
                    GetExternalTools();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, "エラー",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void DeleteButtonClick(object sender, RoutedEventArgs e)
        {
            ListBoxItem? selectedItem = ExternalToolsListBox.SelectedItem as ListBoxItem;
            if (selectedItem != null)
            {
                try
                {
                    string? toolName = selectedItem.Content.ToString();
                    if (!string.IsNullOrEmpty(toolName))
                    {
                        ExternalTool.Delete(toolName);
                        GetExternalTools();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(this, ex.Message, "エラー",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show(this, "削除する外部ツールを選択してください。", "外部ツールの管理",
                    MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
        }
    }
}
