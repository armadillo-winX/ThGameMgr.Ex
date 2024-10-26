global using System;
global using System.Diagnostics;
global using System.IO;
global using System.Text;
global using System.Windows;

global using ThGameMgr.Ex.Dialogs;
global using ThGameMgr.Ex.Exceptions;
global using ThGameMgr.Ex.Extensions;
global using ThGameMgr.Ex.Game;
global using ThGameMgr.Ex.Score;
global using ThGameMgr.Ex.Settings;

using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace ThGameMgr.Ex
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string? _gameId;
        private string? _gameName;
        private string? _filterPlayer;
        private string? _filterEnemy;
        private string? _filterLevel;

        private BackgroundWorker? _gameEndWaitingModeWorker = null;
        private DispatcherTimer? _gameControlTimer = null;
        private DispatcherTimer? _gameAudioControlTimer = null;
        private ResizerFrameWindow? _resizerFrameWindow = null;

        private string? GameId
        {
            get
            {
                return _gameId;
            }

            set
            {
                _gameId = value;

                if (!string.IsNullOrWhiteSpace(value))
                {
                    GameIdBlock.Text = value;

                    AutoStartWindowResizerCheckBox.IsEnabled = true;
                    PlayersFilterButton.IsEnabled = true;
                    EnemiesFilterButton.IsEnabled = true;
                    LevelFilterButton.IsEnabled = true;

                    AutoStartWindowResizerCheckBox.IsChecked = GameSpecificConfig.GetAutoResizerConfig(value);
                    this.FilterPlayer = GameSpecificConfig.GetScoreFilterPlayer(value);
                    this.FilterEnemy = GameSpecificConfig.GetScoreFilterEnemy(value);
                    this.FilterLevel = GameSpecificConfig.GetScoreFilterLevel(value);

                    string gameName = GameIndex.GetGameName(value);
                    this.GameName = gameName;
                }
                else
                {
                    GameIdBlock.Text = "Th00";
                    this.GameName = string.Empty;

                    AutoStartWindowResizerCheckBox.IsEnabled = false;
                    PlayersFilterButton.IsEnabled = false;
                    EnemiesFilterButton.IsEnabled = false;
                    LevelFilterButton.IsEnabled = false;
                }
            }
        }

        private string? GameName
        {
            get
            {
                return _gameName;
            }

            set
            {
                _gameName = value;

                if (!string.IsNullOrEmpty(value))
                {
                    GameNameBlock.Text = value;
                    ControlPanelGameNameBlock.Text = value;
                }
                else
                {
                    GameNameBlock.Text = "[ゲーム未選択]";
                    ControlPanelGameNameBlock.Text = "[ゲーム未選択]";
                }
            }
        }

        private Process GameProcess { get; set; }

        private DateTime GameStartTime { get; set; }

        private string? FilterPlayer
        {
            get
            {
                return _filterPlayer;
            }

            set
            {
                _filterPlayer = value;
                FilterPlayerNameBlock.Text =
                    value == "ALL" ? "全機体" : value;

                if (!string.IsNullOrEmpty(this.GameId))
                {
                    GameSpecificConfig.SetScoreFilterPlayer(this.GameId, value);
                }
            }
        }

        private string? FilterEnemy
        {
            get
            {
                return _filterEnemy;
            }

            set
            {
                _filterEnemy = value;
                FilterEnemyNameBlock.Text =
                    value == "ALL" ? "全ボス機体" : value;

                if (!string.IsNullOrEmpty(this.GameId))
                {
                    GameSpecificConfig.SetScoreFilterEnemy(this.GameId, value);
                }
            }
        }

        private string? FilterLevel
        {
            get
            {
                return _filterLevel;
            }

            set
            {
                _filterLevel = value;
                FilterLevelBlock.Text =
                    value == "ALL" ? "全難易度" : value;

                if (!string.IsNullOrEmpty(this.GameId))
                {
                    GameSpecificConfig.SetScoreFilterLevel(this.GameId, value);
                }
            }
        }

        public MainWindow()
        {
            InitializeComponent();

            this.Title = $"{VersionInfo.AppName} {VersionInfo.CodeName}";
            this.GameId = string.Empty;
            this.GameProcess = new();

            EnableGameEndWaitingLimitationMode(false);

            if (!Directory.Exists(PathInfo.UsersDirectory))
            {
                try
                {
                    Directory.CreateDirectory(PathInfo.UsersDirectory);
                }
                catch (Exception)
                {

                }
            }

            if (File.Exists(PathInfo.UsersIndexFile))
            {
                try
                {
                    ConfigureUserSelection();

                    ConfigureSettings();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"設定の復元ができませんでした。\nアプリケーションを終了します。\n{ex.Message}", "エラー",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                    Environment.Exit(-1);
                }
            }
            else
            {
                //初回起動時の挙動
                InitializeAppSetup();
            }
        }

        private async void GetScoreData()
        {
            ScoreDataGrid.DataContext = null;
            SpellCardDataGrid.DataContext = null;
            string gameId = this.GameId;
            if (!string.IsNullOrEmpty(gameId))
            {
                EnableGettingScoreDataLimitationMode(true);

                ScoreViewFilter filter = new()
                {
                    ScoreLevelFilter = this.FilterLevel,
                    ScorePlayerFilter = this.FilterPlayer,
                    SpellCardEnemyFilter = this.FilterEnemy
                };

                bool displayUnchallengedCard = DisplayUnchallengedCardMenuItem.IsChecked;

                try
                {
                    await Task.Run(()
                    => ScoreView.GetScoreData(gameId, filter, displayUnchallengedCard)
                    );

                    if (ScoreView.ScoreRecordLists.Count >= 0)
                    {
                        ScoreDataGrid.AutoGenerateColumns = false;
                        ScoreDataGrid.DataContext = ScoreView.ScoreRecordLists;
                    }

                    if (ScoreView.SpellCardRecordLists.Count >= 0)
                    {
                        SpellCardDataGrid.AutoGenerateColumns = false;
                        SpellCardDataGrid.DataContext = ScoreView.SpellCardRecordLists;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(this, $"スコアデータの取得に失敗しました。\n{ex.Message}", "エラー",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }

                EnableGettingScoreDataLimitationMode(false);
            }
        }

        private void SetGameSelectionMenu()
        {
            GameSelectionButtonContextMenu.Items.Clear();

            List<string> enabledGamesList = GameIndex.GetEnabledGamesList();

            if (enabledGamesList.Count > 0)
            {
                foreach (string gameId in enabledGamesList)
                {
                    string gameName = GameIndex.GetGameName(gameId);

                    MenuItem gameMenuItem = new()
                    {
                        Header = $"{gameId}: {gameName}",
                        Uid = gameId
                    };

                    gameMenuItem.Click += new RoutedEventHandler(GameSelectionMenuItemClick);

                    GameSelectionButtonContextMenu.Items.Add(gameMenuItem);
                }
            }

            Separator separator = new();
            GameSelectionButtonContextMenu.Items.Add(separator);

            MenuItem gamePathSettingsMenuItem = new()
            {
                Header = "ゲームのパスを設定する",
                Icon =
                new Image
                {
                    Source = new BitmapImage(
                        new Uri("pack://application:,,,/ThGameMgr.Ex;component/Images/Settings.png")
                        )
                }
            };
            gamePathSettingsMenuItem.Click += new RoutedEventHandler(GamePathSettingsMenuItemClick);
            GameSelectionButtonContextMenu.Items.Add(gamePathSettingsMenuItem);
        }

        private void GameSelectionMenuItemClick(object sender, RoutedEventArgs e)
        {
            try
            {
                string gameId = ((MenuItem)sender).Uid;
                if (!string.IsNullOrEmpty(gameId))
                {
                    this.GameId = gameId;

                    SetPlayersFilterMenu();
                    SetEnemiesFilterMenu();
                    SetLevelFilterMenu();

                    GetScoreData();
                }
                else
                {
                    MessageBox.Show(this, "不正な選択です。", "エラー",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception)
            {
                MessageBox.Show(this, "不正な選択です。",
                    "エラー",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SetPlayersFilterMenu()
        {
            PlayersFilterButtonContextMenu.Items.Clear();

            MenuItem allItem = new()
            {
                Header = "ALL"
            };
            allItem.Click += new RoutedEventHandler(PlayersFilterMenuClick);
            PlayersFilterButtonContextMenu.Items.Add(allItem);

            Separator separator = new();
            PlayersFilterButtonContextMenu.Items.Add(separator);

            string gameId = this.GameId;
            string gamePlayers = GamePlayers.GetGamePlayers(gameId);
            if (gamePlayers != null)
            {
                string[] gamePlayersList = gamePlayers.Split(',');
                foreach (string gamePlayer in gamePlayersList)
                {
                    MenuItem item = new()
                    {
                        Header = gamePlayer
                    };
                    item.Click += new RoutedEventHandler(PlayersFilterMenuClick);
                    PlayersFilterButtonContextMenu.Items.Add(item);
                }
            }
        }

        private void PlayersFilterMenuClick(object sender, RoutedEventArgs e)
        {
            string playerName = ((MenuItem)sender).Header.ToString();
            this.FilterPlayer = playerName;
            GetScoreData();
        }

        private void SetEnemiesFilterMenu()
        {
            EnemiesFilterButtonContextMenu.Items.Clear();

            MenuItem allItem = new()
            {
                Header = "ALL"
            };
            allItem.Click += new RoutedEventHandler(EnemiesFilterMenuClick);
            EnemiesFilterButtonContextMenu.Items.Add(allItem);

            Separator separator = new();
            EnemiesFilterButtonContextMenu.Items.Add(separator);

            string gameId = this.GameId;
            string gameEnemies = GameEnemies.GetGameEnemies(gameId);
            if (gameEnemies != null)
            {
                string[] gameEnemiesList = gameEnemies.Split(',');
                foreach (string gameEnemy in gameEnemiesList)
                {
                    MenuItem item = new()
                    {
                        Header = gameEnemy
                    };
                    item.Click += new RoutedEventHandler(EnemiesFilterMenuClick);
                    EnemiesFilterButtonContextMenu.Items.Add(item);
                }
            }
        }

        private void EnemiesFilterMenuClick(object sender, RoutedEventArgs e)
        {
            string enemyName = ((MenuItem)sender).Header.ToString();
            this.FilterEnemy = enemyName;
            GetScoreData();
        }

        private void SetLevelFilterMenu()
        {
            LevelFilterButtonContextMenu.Items.Clear();

            MenuItem allItem = new()
            {
                Header = "ALL"
            };
            allItem.Click += new RoutedEventHandler(LevelFilterMenuClick);
            LevelFilterButtonContextMenu.Items.Add(allItem);

            Separator separator = new();
            LevelFilterButtonContextMenu.Items.Add(separator);

            string[] gameLevelList = ["Easy", "Normal", "Hard", "Lunatic", "Extra", "Phantasm"];
            foreach (string gameLevel in gameLevelList)
            {
                MenuItem item = new()
                {
                    Header = gameLevel
                };
                item.Click += new RoutedEventHandler(LevelFilterMenuClick);
                LevelFilterButtonContextMenu.Items.Add(item);
            }
        }

        private void LevelFilterMenuClick(object sender, RoutedEventArgs e)
        {
            string level = ((MenuItem)sender).Header.ToString();
            this.FilterLevel = level;
            GetScoreData();
        }

        /// <summary>
        /// 初回起動時のセットアップ処理です。
        /// </summary>
        private void InitializeAppSetup()
        {

            MessageBox.Show("ユーザーを追加してください。", VersionInfo.AppName,
                MessageBoxButton.OK, MessageBoxImage.Information);

            AddUserDialog addUserDialog = new();
            if (addUserDialog.ShowDialog() == true)
            {
                MessageBox.Show(
                    "ゲームのパスを設定してください。", VersionInfo.AppName,
                    MessageBoxButton.OK, MessageBoxImage.Information);

                GamePathSettingsDialog gamePathSettingsDialog = new();
                gamePathSettingsDialog.ShowDialog();

                SetGameSelectionMenu();

                CurrentUserStatusBarItem.Content = User.CurrentUserName;
            }
            else
            {
                MessageBox.Show("有効なユーザーが存在しないので、続行できません。\nアプリケーションを終了します。",
                    VersionInfo.AppName,
                    MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
        }

        /// <summary>
        /// メインウィンドウの読み込み時にユーザー情報を構成します。
        /// </summary>
        private void ConfigureUserSelection()
        {
            if (File.Exists(PathInfo.UserSelectionConfigFile))
            {
                string userName = User.GetUserSelection();
                if (User.Exists(userName))
                {
                    User.Switch(userName);
                }
                else
                {
                    SelectUserDialog selectUserDialog = new();
                    if (selectUserDialog.ShowDialog() != true)
                    {
                        MessageBox.Show("ユーザーが選択されませんでした。\nアプリケーションを終了します。",
                            VersionInfo.AppName,
                            MessageBoxButton.OK,
                            MessageBoxImage.Exclamation);
                    }
                }
            }
            else
            {
                SelectUserDialog selectUserDialog = new();
                if (selectUserDialog.ShowDialog() != true)
                {
                    MessageBox.Show("ユーザーが選択されませんでした。\nアプリケーションを終了します。",
                        VersionInfo.AppName,
                        MessageBoxButton.OK,
                        MessageBoxImage.Exclamation);
                }
            }
        }

        private void ConfigureSettings()
        {
            SettingsConfigurator.ConfigureGamePathSettings();
            SettingsConfigurator.ConfigureGameSpecificConfig();
            MainWindowSettings mainWindowSettings = SettingsConfigurator.ConfigureMainWindowSettings();

            this.Width = mainWindowSettings.MainWindowWidth;
            this.Height = mainWindowSettings.MainWindowHeight;
            this.GameId = mainWindowSettings.SelectedGameId;
            DisplayUnchallengedCardMenuItem.IsChecked = mainWindowSettings.DisplayUnchallengedCard;

            SetGameSelectionMenu();
            SetPlayersFilterMenu();
            SetEnemiesFilterMenu();
            SetLevelFilterMenu();

            AutoStartWindowResizerCheckBox.IsChecked = GameSpecificConfig.GetAutoResizerConfig(this.GameId);
            CurrentUserStatusBarItem.Content = User.CurrentUserName;

            GetScoreData();
        }

        private void SaveSettings()
        {
            SettingsConfigurator.SaveGamePathSettings();
            SettingsConfigurator.SaveGameSpecificConfig();

            MainWindowSettings mainWindowSettings = new()
            {
                MainWindowWidth = this.Width,
                MainWindowHeight = this.Height,
                SelectedGameId = this.GameId,
                DisplayUnchallengedCard = DisplayUnchallengedCardMenuItem.IsChecked
            };

            SettingsConfigurator.SaveMainWindowSettings(mainWindowSettings);
        }

        /// <summary>
        /// ゲーム終了待機中の機能制限モードを有効化します。
        /// </summary>
        /// <param name="enabled"></param>
        private void EnableGameEndWaitingLimitationMode(bool enabled)
        {
            AddUserMenuItem.IsEnabled = !enabled;
            SwitchUserMenuItem.IsEnabled = !enabled;
            StartGameMenuItem.IsEnabled = !enabled;
            StartGameWithVpatchMenuItem.IsEnabled = !enabled;
            StartGameWithThpracMenuItem.IsEnabled = !enabled;
            StartCustomMenuItem.IsEnabled = !enabled;
            GamePathSettingsMenuItem.IsEnabled = !enabled;
            ReloadScoreDataMenuItem.IsEnabled = !enabled;
            DisplayUnchallengedCardMenuItem.IsEnabled = !enabled;
            StartWindowResizerMenuItem.IsEnabled = enabled;

            GameSelectionButton.IsEnabled = !enabled;
            StartGameButton.IsEnabled = !enabled;
            StartGameWithVpatchButton.IsEnabled = !enabled;
            StartGameWithThpracButton.IsEnabled = !enabled;
            StartCustomProgramButton.IsEnabled = !enabled;
            GameAudioControlSlider.IsEnabled = enabled;
            PlayersFilterButton.IsEnabled = !enabled;
            LevelFilterButton.IsEnabled = !enabled;
            EnemiesFilterButton.IsEnabled = !enabled;
        }

        /// <summary>
        /// スコアデータ取得中の機能制限モードを有効化します。
        /// </summary>
        /// <param name="enabled"></param>
        private void EnableGettingScoreDataLimitationMode(bool enabled)
        {
            AddUserMenuItem.IsEnabled = !enabled;
            SwitchUserMenuItem.IsEnabled = !enabled;
            StartGameMenuItem.IsEnabled = !enabled;
            StartGameWithVpatchMenuItem.IsEnabled = !enabled;
            StartGameWithThpracMenuItem.IsEnabled = !enabled;
            StartCustomMenuItem.IsEnabled = !enabled;
            GamePathSettingsMenuItem.IsEnabled = !enabled;
            ReloadScoreDataMenuItem.IsEnabled = !enabled;
            DisplayUnchallengedCardMenuItem.IsEnabled = !enabled;

            GameSelectionButton.IsEnabled = !enabled;
            StartGameButton.IsEnabled = !enabled;
            StartGameWithVpatchButton.IsEnabled = !enabled;
            StartGameWithThpracButton.IsEnabled = !enabled;
            StartCustomProgramButton.IsEnabled = !enabled;
            PlayersFilterButton.IsEnabled = !enabled;
            LevelFilterButton.IsEnabled = !enabled;
            EnemiesFilterButton.IsEnabled = !enabled;
        }

        /// <summary>
        /// ゲーム終了待機モードを開始します。
        /// </summary>
        /// <param name="enabled"></param>
        private void StartGameEndWaitingMode(Process gameProcess)
        {
            this.GameProcess = gameProcess;
            this.GameStartTime = gameProcess.StartTime;

            int gameProcessId = gameProcess.Id;

            EnableGameEndWaitingLimitationMode(true);

            _gameEndWaitingModeWorker = new BackgroundWorker();
            _gameEndWaitingModeWorker.DoWork += new DoWorkEventHandler(WorkerDoWork);
            _gameEndWaitingModeWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(WorkerRunningComplete);
            _gameEndWaitingModeWorker.RunWorkerAsync(gameProcess);

            _gameControlTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(10)
            };

            _gameControlTimer.Tick += (e, s) =>
            {
                TimeSpan time = DateTime.Now - this.GameStartTime;
                GameRunningTimeBlock.Text = time.ToString(@"mm\m\i\nss\s\e\c");

                //_resizerFrameWindow が null であるのは、ウィンドウリサイザフレームが一回も起動していないのと同じ
                if (AutoStartWindowResizerCheckBox.IsChecked == true &&
                _resizerFrameWindow == null)
                {
                    try
                    {
                        //タイマー処理の度にプロセスIDからプロセスを取得する
                        gameProcess = Process.GetProcessById(gameProcessId);

                        GameWindowSizes gameWindowSizes = GameWindowHandler.GetWindowSizes(gameProcess.MainWindowHandle);

                        if (gameWindowSizes.Width > 500)
                        {
                            _resizerFrameWindow = new()
                            {
                                GameWindow = gameProcess.MainWindowHandle
                            };

                            _resizerFrameWindow.Show();
                        }
                    }
                    catch (Exception)
                    {
                    }
                }
            };

            _gameAudioControlTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(10)
            };

            _gameAudioControlTimer.Tick += (e, s) =>
            {
                float gameAudioVolume = 0;
                try
                {
                    gameAudioVolume = GameAudio.GetGameProcessAudioVolume(this.GameProcess);
                }
                catch (Exception)
                {
                    gameAudioVolume = 0;
                }

                GameAudioControlSlider.Value = gameAudioVolume * 100;
            };

            _gameControlTimer.Start();
            _gameAudioControlTimer.Start();
        }

        private void WorkerDoWork(object? sender, DoWorkEventArgs e)
        {
            Process gameProcess = (Process)e.Argument;
            gameProcess.WaitForExit();
        }

        private void WorkerRunningComplete(object? sender, RunWorkerCompletedEventArgs e)
        {
            this.GameProcess.Dispose();
            _gameControlTimer?.Stop();
            _gameAudioControlTimer?.Stop();

            GameAudioControlSlider.Value = 0;

            DateTime gameEndTime = DateTime.Now;

            TimeSpan runningTimeSpan = gameEndTime - this.GameStartTime;

            if (_resizerFrameWindow != null && _resizerFrameWindow.IsLoaded)
            {
                _resizerFrameWindow.Close();
            }
            _resizerFrameWindow = null;

            EnableGameEndWaitingLimitationMode(false);
            
            GetScoreData();
        }

        private void ExitMenuItemClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void GamePathSettingsMenuItemClick(object sender, RoutedEventArgs e)
        {
            GamePathSettingsDialog gamePathSettingsDialog = new()
            {
                Owner = this
            };

            gamePathSettingsDialog.ShowDialog();
            SetGameSelectionMenu();
        }

        private async void StartGameMenuItemClick(object sender, RoutedEventArgs e)
        {
            string gameId = this.GameId;
            if (!string.IsNullOrEmpty(gameId))
            {
                EnableGameEndWaitingLimitationMode(true);
                try
                {
                    Process gameProcess
                        = await Task.Run(()
                                => GameProcessHandler.StartGameProcess(gameId)
                                );
                    StartGameEndWaitingMode(gameProcess);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(this, $"ゲームの起動に失敗しました。\n{ex.Message}", "エラー",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                    EnableGameEndWaitingLimitationMode(false);
                }
            }
        }

        private async void StartGameWithVpatchMenuItemClick(object sender, RoutedEventArgs e)
        {
            string gameId = this.GameId;
            if (!string.IsNullOrEmpty(gameId))
            {
                EnableGameEndWaitingLimitationMode(true);
                try
                {
                    Process gameProcess
                        = await Task.Run(()
                                => GameProcessHandler.StartGameProcessWithApplyingTool(gameId, "vpatch.exe")
                                );
                    StartGameEndWaitingMode(gameProcess);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(this, $"ゲームの起動に失敗しました。\n{ex.Message}", "エラー",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                    EnableGameEndWaitingLimitationMode(false);
                }
            }
        }

        private void StartGameWithThpracMenuItemClick(object sender, RoutedEventArgs e)
        {

        }

        private void StartCustomMenuItemClick(object sender, RoutedEventArgs e)
        {
            string gameId = this.GameId;
            if (!string.IsNullOrEmpty(gameId))
            {
                try
                {
                    GameProcessHandler.StartCustomProgramProcess(gameId);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(this, $"custom.exe (環境設定)の起動に失敗しました。\n{ex.Message}", "エラー",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void AddUserMenuItemClick(object sender, RoutedEventArgs e)
        {
            AddUserDialog addUserDialog = new()
            {
                Owner = this
            };

            try
            {
                SaveSettings();

            }
            catch (Exception ex)
            {
                MessageBox.Show(this, 
                    $"設定の保存に失敗しました。\n現在の設定は、別のユーザーに切り替わった際に破棄されます。\n\n{ex.Message}",
                    "エラー", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            if (addUserDialog.ShowDialog() == true)
            {
                try
                {
                    ConfigureSettings();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(this, $"設定の構成に失敗しました。\n{ex.Message}", "エラー",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void GameSelectionButtonClick(object sender, RoutedEventArgs e)
        {
            if (!GameSelectionButtonContextMenu.IsOpen)
            {
                GameSelectionButtonContextMenu.PlacementTarget = GameSelectionButton;
                GameSelectionButtonContextMenu.Placement = System.Windows.Controls.Primitives.PlacementMode.Bottom;
                GameSelectionButtonContextMenu.IsOpen = true;
            }
        }

        private void AutoStartWindowResizerCheckBoxClick(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(this.GameId))
            {
                GameSpecificConfig.SetAutoResizerConfig(
                    this.GameId, AutoStartWindowResizerCheckBox.IsChecked);
            }
        }

        private void WindowClosing(object sender, CancelEventArgs e)
        {
            try
            {
                User.SaveUserSelectionConfig();
                SaveSettings();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"設定の保存に失敗しました。\n{ex.Message}", "エラー",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SelectUserMenuItemClick(object sender, RoutedEventArgs e)
        {
            SelectUserDialog selectUserDialog = new()
            {
                Owner = this
            };

            try
            {
                SaveSettings();

            }
            catch (Exception ex)
            {
                MessageBox.Show(this,
                    $"設定の保存に失敗しました。\n現在の設定は、別のユーザーに切り替わった際に破棄されます。\n\n{ex.Message}",
                    "エラー", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            if (selectUserDialog.ShowDialog() == true)
            {
                try
                {
                    ConfigureSettings();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(this, $"設定の構成に失敗しました。\n{ex.Message}", "エラー",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void PlayersFilterButtonClick(object sender, RoutedEventArgs e)
        {
            if (!PlayersFilterButtonContextMenu.IsOpen)
            {
                PlayersFilterButtonContextMenu.PlacementTarget = PlayersFilterButton;
                PlayersFilterButtonContextMenu.Placement = System.Windows.Controls.Primitives.PlacementMode.Bottom;
                PlayersFilterButtonContextMenu.IsOpen = true;
            }
        }

        private void EnemiesFilterButtonClick(object sender, RoutedEventArgs e)
        {
            if (!EnemiesFilterButtonContextMenu.IsOpen)
            {
                EnemiesFilterButtonContextMenu.PlacementTarget = EnemiesFilterButton;
                EnemiesFilterButtonContextMenu.Placement = System.Windows.Controls.Primitives.PlacementMode.Bottom;
                EnemiesFilterButtonContextMenu.IsOpen = true;
            }
        }

        private void LevelFilterButtonClick(object sender, RoutedEventArgs e)
        {
            if (!LevelFilterButtonContextMenu.IsOpen)
            {
                LevelFilterButtonContextMenu.PlacementTarget = LevelFilterButton;
                LevelFilterButtonContextMenu.Placement = System.Windows.Controls.Primitives.PlacementMode.Bottom;
                LevelFilterButtonContextMenu.IsOpen = true;
            }
        }

        private void ReloadScoreDataMenuItemClick(object sender, RoutedEventArgs e)
        {
            GetScoreData();
        }

        private void ShowUnchallengedCardMenuItemClick(object sender, RoutedEventArgs e)
        {
            GetScoreData();
        }

        private void StartWindowResizerMenuItemClick(object sender, RoutedEventArgs e)
        {
            if (_resizerFrameWindow == null || !_resizerFrameWindow.IsLoaded)
            {
                _resizerFrameWindow = new()
                {
                    GameWindow = this.GameProcess.MainWindowHandle
                };

                _resizerFrameWindow.Show();
            }
        }

        private void VersionInfoMenuItemClick(object sender, RoutedEventArgs e)
        {
            VersionInfoDialog versionInfoDialog = new()
            {
                Owner = this
            };

            versionInfoDialog.ShowDialog();
        }

        private void StatisticSpellCardRecordMenuItemClick(object sender, RoutedEventArgs e)
        {
            if (ScoreView.SpellCardRecordLists != null &&
                ScoreView.SpellCardRecordLists.Count > 0)
            {
                int totalChallengeCount = 0;
                int totalGetCount = 0;

                for (int i = 0; i < ScoreView.SpellCardRecordLists.Count; i++)
                {
                    SpellCardRecordList spellCardRecordList = ScoreView.SpellCardRecordLists[i];
                    totalChallengeCount += int.Parse(spellCardRecordList.Challenge);
                    totalGetCount += int.Parse(spellCardRecordList.Get);
                }

                string totalGetRate = ScoreCalculator.CalcSpellCardGetRate(totalGetCount, totalChallengeCount);

                SpellCardRecordStatisticDialog spellCardRecordStatisticDialog = new()
                {
                    GameId = this.GameId,
                    ToTalChallengeCount = totalChallengeCount,
                    TotalGetCount = totalGetCount,
                    TotalGetRate = totalGetRate,
                    Owner = this
                };
                spellCardRecordStatisticDialog.ShowDialog();
            }
        }

        private void GameAudioControlSliderValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            GameAudioVolumeBlock.Text = ((int)(GameAudioControlSlider.Value)).ToString();
            if (_gameEndWaitingModeWorker != null && _gameEndWaitingModeWorker.IsBusy)
            {
                try
                {
                    float gameAudioVolume = GameAudio.GetGameProcessAudioVolume(this.GameProcess);
                    if (gameAudioVolume * 100 != GameAudioControlSlider.Value)
                    {
                        GameAudio.SetGameProcessAudioVolume(
                            this.GameProcess, (float)(GameAudioControlSlider.Value / 100));
                    }
                }
                catch (Exception)
                {

                }
            }
        }
    }
}