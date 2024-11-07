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
                        if (GameWindowHandler.GameWindowExists(value))
                        {
                            GameWindowPosition gameWindowPosition = GameWindowHandler.GetWindowPosition(value);

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

        public ResizerFrameWindow()
        {
            InitializeComponent();

            try
            {
                ResizerFrameWindowSettings resizerFrameWindowSettings
                    = SettingsConfigurator.ConfigureResizerFrameWindowSettings();
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
        }

        private void SetFramePosition(IntPtr gameWindow)
        {
            try
            {
                GameWindowPosition gameWindowPosition = GameWindowHandler.GetWindowPosition(gameWindow);
                GameWindowSizes gameWindowSizes = GameWindowHandler.GetWindowSizes(gameWindow);

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

        private void ResizeButtonClick(object sender, RoutedEventArgs e)
        {
            try
            {
                int width = (int)(this.Width - 36);
                int height = (int)(this.Height - 36);

                GameWindowHandler.ResizeWindow(this.GameWindow, width, height);

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

                SettingsConfigurator.SaveResizerFrameWindowSettings(resizerFrameWindowSettings);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        private void CloseMenuClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
