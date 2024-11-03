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

            if (!File.Exists($"{User.CurrentUserDirectoryPath}\\Settings\\ExternalTools.xml"))
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

            string exToolsConfig = $"{User.CurrentUserDirectoryPath}\\Settings\\ExternalTools.xml";
            if (File.Exists(exToolsConfig))
            {
                XmlDocument exToolsConfigXml = new();
                exToolsConfigXml.Load(exToolsConfig);
                XmlNodeList exToolsNodeList = exToolsConfigXml.SelectNodes("ExternalTools/ExternalTool");
                if (exToolsNodeList.Count > 0)
                {
                    foreach (XmlNode toolNode in exToolsNodeList)
                    {
                        string name = toolNode.SelectSingleNode("Name").InnerText;
                        ExternalToolsListBox.Items.Add(name);
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
            if (ExternalToolsListBox.SelectedIndex > -1)
            {
                try
                {
                    string toolName = ExternalToolsListBox.SelectedItem.ToString();
                    ExternalTool.DeleteExternalTool(toolName);
                    GetExternalTools();
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
