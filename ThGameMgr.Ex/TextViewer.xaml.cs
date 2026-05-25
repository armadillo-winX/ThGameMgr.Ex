namespace ThGameMgr.Ex
{
    /// <summary>
    /// TextViewer.xaml の相互作用ロジック
    /// </summary>
    public partial class TextViewer : Window
    {
        public TextViewer()
        {
            InitializeComponent();
        }

        public string? FilePath { get; set; }

        public string? Encode { get; set; }

        private void WindowLoaded(object sender, RoutedEventArgs e)
        {
            if (File.Exists(this.FilePath))
            {
                try
                {
                    //Shift_JISに対応させる
                    Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

                    StreamReader streamReader;
                    if (!string.IsNullOrEmpty(this.Encode))
                    {
                        streamReader = new(this.FilePath, Encoding.GetEncoding(this.Encode));
                    }
                    else
                    {
                        streamReader = new(this.FilePath, Encoding.UTF8);
                    }
                    string text = streamReader.ReadToEnd();
                    streamReader.Close();

                    MainTextBox.Text = text;
                }
                catch (Exception ex)
                {
                    MainTextBox.Text =
                        $"指定されたファイルの読み込みに失敗しました。\n{ex.Message}";
                    this.Title = "エラー";
                }
            }
            else
            {
                MainTextBox.Text = "指定されたファイルが存在しません。";
            }
        }
    }
}
