using System.Windows.Controls;

namespace ThGameMgr.Ex.Dialogs
{
    /// <summary>
    /// ManagePluginsDialog.xaml の相互作用ロジック
    /// </summary>
    public partial class ManagePluginsDialog : Window
    {
        public ManagePluginsDialog()
        {
            InitializeComponent();

            PluginDataGrid.AutoGenerateColumns = false;

            if (PluginHandler.GameFilesPlugins != null && PluginHandler.GameFilesPlugins.Count > 0)
            {
                foreach (dynamic plugin in PluginHandler.GameFilesPlugins)
                {
                    PluginDataGrid.Items.Add(new PluginInfo()
                    {
                        Name = plugin.Name,
                        Version = plugin.Version,
                        Developer = plugin.Developer,
                        Description = plugin.Description
                    });
                }
            }

            if (PluginHandler.SelectedGamePlugins != null && PluginHandler.SelectedGamePlugins.Count > 0)
            {
                foreach (dynamic plugin in PluginHandler.SelectedGamePlugins)
                {
                    PluginDataGrid.Items.Add(new PluginInfo()
                    {
                        Name = plugin.Name,
                        Version = plugin.Version,
                        Developer = plugin.Developer,
                        Description = plugin.Description
                    });
                }
            }

            if (PluginHandler.ToolPlugins != null && PluginHandler.ToolPlugins.Count > 0)
            {
                foreach (dynamic plugin in PluginHandler.ToolPlugins)
                {
                    PluginDataGrid.Items.Add(new PluginInfo()
                    {
                        Name = plugin.Name,
                        Version = plugin.Version,
                        Developer = plugin.Developer,
                        Description = plugin.Description
                    });
                }
            }
        }

        private void PluginDataGridSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (PluginDataGrid.Items != null &&
                PluginDataGrid.Items.Count > 0 &&
                PluginDataGrid.SelectedIndex > -1)
            {
                PluginInfo pluginInfo = PluginDataGrid.SelectedItem as PluginInfo;
                if (pluginInfo != null)
                    DetailBox.Text
                        = @$"{pluginInfo.Name}
Version.{pluginInfo.Version}
開発者:{pluginInfo.Developer}

[説明]
{pluginInfo.Description}";
            }
        }
    }
}
