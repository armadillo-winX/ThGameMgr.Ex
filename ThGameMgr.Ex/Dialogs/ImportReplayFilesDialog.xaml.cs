using System.Threading.Tasks;

namespace ThGameMgr.Ex.Dialogs
{
    /// <summary>
    /// ImportReplayFilesDialog.xaml の相互作用ロジック
    /// </summary>
    public partial class ImportReplayFilesDialog : Window
    {
        public ImportReplayFilesDialog()
        {
            InitializeComponent();
        }

        private async void ImportButtonClick(object sender, RoutedEventArgs e)
        {
            if (ReplayFilesListBox.Items.Count > 0)
            {
                string[] replayFiles = new string[ReplayFilesListBox.Items.Count];

                int i = 0;
                foreach (object replayFileItem in ReplayFilesListBox.Items)
                {
                    replayFiles[i] = (string)replayFileItem;
                    i++;
                }

                ImportButton.IsEnabled = false;
                await Task.Run(() =>
                {
                    foreach (string replayFile in replayFiles)
                    {
                        string message = ReplayFile.Import(replayFile);

                        this.Dispatcher.Invoke((Action)(() =>
                        {
                            ReplayFilesListBox.Items.Remove(replayFile);
                            OutputBox.Text += $"{message}\n";
                        }
                        ));
                    }
                });
                ImportButton.IsEnabled = true;

                MessageBox.Show(this, "リプレイファイルを取り込みました。", "リプレイファイルの一括追加",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show(this, "取り込むリプレイファイルがありません。", "リプレイファイルの取り込み",
                    MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
        }

        private void WindowPreviewDragOver(object sender, DragEventArgs e)
        {
            //ファイルがドラッグされたとき、カーソルをドラッグ中のアイコンに変更し、そうでない場合は何もしない。
            e.Effects = e.Data.GetDataPresent(DataFormats.FileDrop) ? DragDropEffects.Copy : e.Effects = DragDropEffects.None;
            e.Handled = true;
        }

        private void WindowPreviewDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop)) // ドロップされたものがファイルかどうか確認する。
            {
                string[] paths = (string[])e.Data.GetData(DataFormats.FileDrop);
                
                foreach (string path in paths)
                {
                    ReplayFilesListBox.Items.Add(path);
                }
            }
        }
    }
}
