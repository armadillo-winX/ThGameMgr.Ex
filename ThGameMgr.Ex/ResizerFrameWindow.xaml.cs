using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;
using System.Windows.Controls;
using System.Windows.Threading;

namespace ThGameMgr.Ex
{
    /// <summary>
    /// ResizerFrameWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class ResizerFrameWindow : Window
    {
        private IntPtr _gameWindow;
        private DispatcherTimer? _timer;

        private readonly IUserService _currentUserService;

        public IntPtr GameWindow
        {
            get
            {
                return _gameWindow;
            }

            set
            {
                _gameWindow = value;
                SetFramePosition(value);

                _timer = new()
                {
                    Interval = TimeSpan.FromMilliseconds(50)
                };

                _timer.Tick += (e, s) =>
                {
                    try
                    {
                        if (GameWindowManager.GameWindowExists(value))
                        {
                            GameWindowPosition gameWindowPosition = GameWindowManager.GetGameWindowPosition(value);

                            this.Left = gameWindowPosition.X - 18;
                            this.Top = gameWindowPosition.Y - 18;
                        }
                        else
                        {
                            this.Close();
                        }
                    }
                    catch (Exception)
                    {
                    }
                };

                _timer.Start();
            }
        }

        public ResizerFrameWindow(IUserService currentUserService)
        {
            InitializeComponent();

            _currentUserService = currentUserService;

            try
            {
                SettingsConfigurator settingsConfigurator = new(currentUserService);
                ResizerFrameWindowSettings resizerFrameWindowSettings
                    = settingsConfigurator.ConfigureResizerFrameWindowSettings();
                AutoCloseMenuItem.IsChecked = resizerFrameWindowSettings.AutoClose;
                if (resizerFrameWindowSettings.FixAspectRate)
                {
                    FixAspectRateCheckBox.IsChecked = true;
                }
                else
                {
                    FixAspectRateCheckBox.IsChecked = false;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);

                FixAspectRateCheckBox.IsChecked = true;
            }

            SetPresetMenuItems();
        }

        private void SetFramePosition(IntPtr gameWindow)
        {
            try
            {
                GameWindowPosition gameWindowPosition = GameWindowManager.GetGameWindowPosition(gameWindow);
                GameWindowSizes gameWindowSizes = GameWindowManager.GetGameWindowSizes(gameWindow);

                this.Left = gameWindowPosition.X - 18;
                this.Top = gameWindowPosition.Y - 18;

                this.Width = gameWindowSizes.Width + 36;
                this.Height = gameWindowSizes.Height + 36;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private void Resize()
        {
            try
            {
                int width = (int)(this.Width - 36);
                int height = (int)(this.Height - 36);

                GameWindowManager.ResizeGameWindow(this.GameWindow, width, height);

                if (AutoCloseMenuItem.IsChecked)
                {
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, "エラー",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SaveResizerPreset(string name)
        {
            ResizerPreset preset = new()
            {
                PresetName = name,
                ResizeWidth = (int)this.Width - 36,
                ResizeHeight = (int)this.Height - 36,
                FixAspectRate = FixAspectRateCheckBox.IsChecked == true
            };

            string timestamp =
                DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss-fffff");
            string fileName = $"ResizerPreset_{timestamp}.thgmrrepr";
            string presetSettingsDir 
                = Path.Combine(_currentUserService.GetCurrentUserSettingsDirectory(), "ResizerPresets");
            string filePath = Path.Combine(presetSettingsDir, fileName);

            JsonSerializerOptions options = new()
            {
                Encoder = JavaScriptEncoder.Create(UnicodeRanges.All),
                WriteIndented = true
            };

            string jsonData = JsonSerializer.Serialize(preset, options);

            if (!Directory.Exists(presetSettingsDir)) { Directory.CreateDirectory(presetSettingsDir); }

            File.WriteAllText(filePath, jsonData);
        }

        private void SetPresetMenuItems()
        {
            PresetMenu.Items.Clear();

            MenuItem savePresetMenuItem = new()
            {
                Header = "リサイザプリセットを保存"
            };
            savePresetMenuItem.Click += new RoutedEventHandler(SaveResizerPresetMenuItemClick);
            PresetMenu.Items.Add(savePresetMenuItem);

            Separator separator = new();
            PresetMenu.Items.Add(separator);
        }

        private void ResizeButtonClick(object sender, RoutedEventArgs e)
        {
            Resize();
        }

        private void SaveResizerPresetMenuItemClick(object sender, RoutedEventArgs e)
        {
            double resizeWidth = this.Width - 36;
            double resizeHeight = this.Height - 36;
            string resizePresetName = $"{(int)resizeWidth}x{(int)resizeHeight}";

            ResizerPresetNameDialog resizerPresetNameDialog = new(resizePresetName)
            {
                Owner = this,
                Top = this.Top + 20,
                Left = this.Left + 20,
            };

            if (resizerPresetNameDialog.ShowDialog() == true)
            {
                resizePresetName = resizerPresetNameDialog.PresetName;
                try
                {
                    SaveResizerPreset(resizePresetName);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(
                        this, $"プリセットの保存に失敗しました．\n{ex.Message}", "エラー",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void FixAspectRateCheckBoxClick(object sender, RoutedEventArgs e)
        {
            if (FixAspectRateCheckBox.IsChecked == true)
            {
                this.Height = (this.Width - 36) * 0.75 + 24 + 36;
            }
        }

        private void WindowSizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (FixAspectRateCheckBox.IsChecked == true)
            {
                this.Height = (this.Width - 36) * 0.75 + 24 + 36;
            }

            WidthBox.Text = (this.Width - 36).ToString();
            HeightBox.Text = (this.Height - 36).ToString();
        }

        private void WindowClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (_timer != null && _timer.IsEnabled)
            {
                _timer.Stop();
            }

            try
            {
                ResizerFrameWindowSettings resizerFrameWindowSettings = new()
                {
                    AutoClose = AutoCloseMenuItem.IsChecked,
                    FixAspectRate = FixAspectRateCheckBox.IsChecked == true
                };

                SettingsConfigurator settingsConfigurator = new(_currentUserService);
                settingsConfigurator.SaveResizerFrameWindowSettings(resizerFrameWindowSettings);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }

            this.Owner.Activate();
        }

        private void CloseMenuClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void WidthBoxKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
            {
                float width;
                float height;

                bool result0 = float.TryParse(WidthBox.Text, out width);
                bool result1 = float.TryParse(HeightBox.Text, out height);

                if (!result0 || !result1)
                {
                    MessageBox.Show(
                        this, "入力が正しくありません．", "エラー",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                    WidthBox.Text = (this.Width - 36).ToString();
                    HeightBox.Text = (this.Height - 36).ToString();

                    return;
                }

                if (FixAspectRateCheckBox.IsChecked == true)
                {
                    this.Width = width + 36;
                }
                else
                {
                    this.Width = width + 36;
                    this.Height = height + 36;
                }

                Resize();
            }
        }

        private void HeightBoxKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
            {
                float width;
                float height;

                bool result0 = float.TryParse(WidthBox.Text, out width);
                bool result1 = float.TryParse(HeightBox.Text, out height);

                if (!result0 || !result1)
                {
                    MessageBox.Show(
                        this, "入力が正しくありません．", "エラー",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                    WidthBox.Text = (this.Width - 36).ToString();
                    HeightBox.Text = (this.Height - 36).ToString();

                    return;
                }

                if (FixAspectRateCheckBox.IsChecked == true)
                {
                    // 一時的にアスペクト比固定を解除して，既定アスペクト比に基づいて幅を設定
                    FixAspectRateCheckBox.IsChecked = false;
                    this.Height = height + 36;
                    this.Width = ((height - 24) * 4) / 3 + 36;
                    FixAspectRateCheckBox.IsChecked = true;
                }
                else
                {
                    this.Width = width + 36;
                    this.Height = height + 36;
                }

                Resize();
            }
        }
    }
}
