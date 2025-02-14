global using System;
global using System.Diagnostics;
global using System.IO;
global using System.Text;
global using System.Windows;

global using ThGameMgr.Ex.Dialogs;
global using ThGameMgr.Ex.Data;
global using ThGameMgr.Ex.Exceptions;
global using ThGameMgr.Ex.Extensions;
global using ThGameMgr.Ex.Game;
global using ThGameMgr.Ex.Plugin;
global using ThGameMgr.Ex.Replay;
global using ThGameMgr.Ex.Score;
global using ThGameMgr.Ex.Settings;

using Microsoft.Win32;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using System.Xml;
using UsersSelectionValidity = ThGameMgr.Ex.User.UsersSelectionValidity;

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
        private string? _spellCardPlayer;
        private string? _filterEnemy;
        private string? _filterPracticeEnemy;
        private string? _filterLevel;

        private BackgroundWorker? _gameEndWaitingModeWorker = null;
        private DispatcherTimer? _gameControlTimer = null;
        private DispatcherTimer? _gameAudioControlTimer = null;
        private ResizerFrameWindow? _resizerFrameWindow = null;
        private ScoreRecordDetailDialog? _scoreRecordDetailDialog = null;
        private SpellCardRecordDetailDialog? _spellCardRecordDetailDialog = null;

        private enum DialogMode
        {
            Information,
            Error
        };

        private string? GameId
        {
            get
            {
                return _gameId;
            }

            set
            {
                _gameId = value;

                if (!string.IsNullOrEmpty(value))
                {
                    GameIdBlock.Text = value;

                    AutoStartWindowResizerCheckBox.IsEnabled = true;
                    PlayersFilterButton.IsEnabled = true;
                    EnemiesFilterButton.IsEnabled = true;
                    LevelFilterButton.IsEnabled = true;

                    AutoStartWindowResizerCheckBox.IsChecked = GameSpecificSettings.GetAutoResizerConfig(value);
                    if (value != GameIndex.Th06 && value != GameIndex.Th07)
                    {
                        SpellCardLevelColumn.Visibility = Visibility.Visible;
                        SpellPracticeLevelColumn.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        SpellCardLevelColumn.Visibility = Visibility.Collapsed;
                        SpellPracticeLevelColumn.Visibility = Visibility.Collapsed;
                    }

                    if (value != GameIndex.Th06)
                    {
                        SpellCardPlayersSwitchButton.IsEnabled = true;
                    }
                    else
                    {
                        SpellCardPlayersSwitchButton.IsEnabled = false;
                    }

                    if (SpellCard.IsSpellPracticeAvailable(value))
                    {
                        SpellPracticeTabItem.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        //スペルプラクティスタブを開いていたら自動で切り替え
                        if (((TabItem)MainTabControl.SelectedItem)?.Header.ToString() == "スペルプラクティス")
                            MainTabControl.SelectedIndex = 0;

                        SpellPracticeTabItem.Visibility = Visibility.Collapsed;
                    }

                    this.FilterPlayer = GameSpecificSettings.GetScoreFilterPlayer(value);
                    this.FilterSpellCardPlayer
                        = value != GameIndex.Th06 ? GameSpecificSettings.GetSpellCardFilterPlayer(value) : "ALL";
                    this.FilterEnemy = GameSpecificSettings.GetScoreFilterEnemy(value);
                    this.FilterPracticeEnemy = GameSpecificSettings.GetScoreFilterPracticeEnemy(value);
                    this.FilterLevel = GameSpecificSettings.GetScoreFilterLevel(value);

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
                if (!string.IsNullOrEmpty(value))
                {
                    _filterPlayer = value;
                    FilterPlayerNameBlock.Text =
                        value == "ALL" ? "全機体" : value;

                    if (!string.IsNullOrEmpty(this.GameId))
                    {
                        GameSpecificSettings.SetScoreFilterPlayer(this.GameId, value);
                    }
                }
                else
                {
                    _filterPlayer = "ALL";
                    FilterPlayerNameBlock.Text = "全機体";
                    if (!string.IsNullOrEmpty(this.GameId))
                    {
                        GameSpecificSettings.SetScoreFilterPlayer(this.GameId, "ALL");
                    }
                }
            }
        }

        private string? FilterSpellCardPlayer
        {
            get
            {
                return _spellCardPlayer;
            }

            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    _spellCardPlayer = value;
                    SpellCardPlayerBlock.Text =
                        value == "ALL" ? "全自機合計" : value;

                    if (!string.IsNullOrEmpty(this.GameId))
                    {
                        GameSpecificSettings.SetSpellCardFilterPlayer(this.GameId, value);
                    }
                }
                else
                {
                    _spellCardPlayer = "ALL";
                    SpellCardPlayerBlock.Text = "全自機合計";
                    if (!string.IsNullOrEmpty(this.GameId))
                    {
                        GameSpecificSettings.SetSpellCardFilterPlayer(this.GameId, "ALL");
                    }
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
                if (!string.IsNullOrEmpty(value))
                {
                    _filterEnemy = value;
                    FilterEnemyNameBlock.Text =
                        value == "ALL" ? "全敵機体" : value;

                    if (!string.IsNullOrEmpty(this.GameId))
                    {
                        GameSpecificSettings.SetScoreFilterEnemy(this.GameId, value);
                    }
                }
                else
                {
                    _filterEnemy = "ALL";
                    FilterEnemyNameBlock.Text = "全敵機体";
                    if (!string.IsNullOrEmpty(this.GameId))
                    {
                        GameSpecificSettings.SetScoreFilterEnemy(this.GameId, "ALL");
                    }
                }
            }
        }

        private string? FilterPracticeEnemy
        {
            get
            {
                return _filterPracticeEnemy;
            }

            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    _filterPracticeEnemy = value;
                    FilterPracticeEnemyNameBlock.Text =
                        value == "ALL" ? "全敵機体" : value;

                    if (!string.IsNullOrEmpty(this.GameId))
                    {
                        GameSpecificSettings.SetScoreFilterPracticeEnemy(this.GameId, value);
                    }
                }
                else
                {
                    _filterPracticeEnemy = "ALL";
                    FilterPracticeEnemyNameBlock.Text = "全敵機体";
                    if (!string.IsNullOrEmpty(this.GameId))
                    {
                        GameSpecificSettings.SetScoreFilterPracticeEnemy(this.GameId, "ALL");
                    }
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
                if (!string.IsNullOrEmpty(value))
                {
                    _filterLevel = value;
                    FilterLevelBlock.Text =
                        value == "ALL" ? "全難易度" : value;

                    if (!string.IsNullOrEmpty(this.GameId))
                    {
                        GameSpecificSettings.SetScoreFilterLevel(this.GameId, value);
                    }
                }
                else
                {
                    _filterLevel = "ALL";
                    FilterLevelBlock.Text = "全難易度";
                    if (!string.IsNullOrEmpty(this.GameId))
                    {
                        GameSpecificSettings.SetScoreFilterLevel(this.GameId, "ALL");
                    }
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
            SetStartGameStatus(string.Empty);

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

            if (Directory.Exists($"{PathInfo.AppLocation}\\SpellCardData"))
            {
                try
                {
                    Directory.Delete($"{PathInfo.AppLocation}\\SpellCardData", true);
                }
                catch (Exception)
                {

                }
            }

            if (!Directory.Exists(PathInfo.PluginDirectory))
            {
                try
                {
                    Directory.CreateDirectory(PathInfo.PluginDirectory);
                }
                catch (Exception)
                {

                }
            }

            UsersSelectionValidity usersSelectionValidity = User.GetUsersSelectionValidity();

            if (usersSelectionValidity == UsersSelectionValidity.Valid)
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
                    Environment.Exit(1);
                }
            }
            else if (usersSelectionValidity == UsersSelectionValidity.Invalid)
            {
                //初回起動時の挙動
                InitializeAppSetup();
            }
            else
            {
                MessageBox.Show("ユーザー選択の有効性検証に失敗しました。\nアプリケーションを終了します。", "エラー",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                Environment.Exit(1);
            }

            ConfigurePlugins();
        }

        private async void StartGame()
        {
            string gameId = this.GameId;
            if (!string.IsNullOrEmpty(gameId))
            {
                EnableGameEndWaitingLimitationMode(true);
                SetStartGameStatus("ゲームの起動を待機中...");
                try
                {
                    Process gameProcess
                        = await Task.Run(()
                                => GameProcessHandler.StartGameProcess(gameId)
                                );
                    StartGameEndWaitingMode(gameProcess);
                }
                catch (Exception)
                {
                    MessageBoxResult result = 
                        MessageBox.Show(this, $"ゲームの起動に失敗しました。\n再試行しますか？", "エラー",
                        MessageBoxButton.YesNo, MessageBoxImage.Error);
                    if (result == MessageBoxResult.Yes)
                    {
                        StartGame();
                    }
                    else
                    {
                        EnableGameEndWaitingLimitationMode(false);
                        SetStartGameStatus(string.Empty);
                    }
                }
            }
        }

        private async void StartGameWithApplyingTool(string toolName)
        {
            string gameId = this.GameId;
            if (!string.IsNullOrEmpty(gameId))
            {
                EnableGameEndWaitingLimitationMode(true);
                SetStartGameStatus("ゲームの起動を待機中(約5秒)...");
                try
                {
                    Process gameProcess
                        = await Task.Run(()
                                => GameProcessHandler.StartGameProcessWithApplyingTool(gameId, toolName)
                                );
                    StartGameEndWaitingMode(gameProcess);
                }
                catch (Exception)
                {
                    MessageBoxResult result =
                    MessageBox.Show(this, $"ゲームの起動に失敗しました。\n再試行しますか？", "エラー",
                        MessageBoxButton.YesNo, MessageBoxImage.Error);
                    if (result == MessageBoxResult.Yes)
                    {
                        StartGameWithApplyingTool(toolName);
                    }
                    else
                    {
                        EnableGameEndWaitingLimitationMode(false);
                        SetStartGameStatus(string.Empty);
                    }
                }
            }
        }

        private async void GetScoreData()
        {
            ShowScoreDataViewerDialog(DialogMode.Error, false, string.Empty);

            ScoreDataGrid.Items.Clear();
            SpellCardDataGrid.Items.Clear();
            SpellPracticeDataGrid.Items.Clear();
            string gameId = this.GameId;
            if (!string.IsNullOrEmpty(gameId))
            {
                EnableRetrievingScoreDataLimitationMode(true);

                try
                {
                    bool result = await Task.Run(()
                    => ScoreData.Get(gameId)
                    );

                    if (result)
                    {
                        ApplyScoreViewFilter();
                    }
                    else
                    {
                        ShowScoreDataViewerDialog(DialogMode.Information, true, "スコアデータビューアが未対応です。");
                    }
                }
                catch (Exception)
                {
                    ShowScoreDataViewerDialog(DialogMode.Error, true, "エラー:スコアデータの取得に失敗しました。");
                }

                EnableRetrievingScoreDataLimitationMode(false);
                EnemiesFilterButton.IsEnabled = this.GameId != GameIndex.Th09;
            }
        }

        private void GetReplayFiles()
        {
            ReplayFilesDataGrid.Effect = null;
            ReplayFilesDataGrid.IsEnabled = true;
            AddReplayFileButton.IsEnabled = true;
            RenameReplayFileButton.IsEnabled = true;
            DeleteReplayFileButton.IsEnabled = true;
            CopyReplayFileButton.IsEnabled = true;
            MoveReplayFileButton.IsEnabled = true;

            ReplayFileErrorBlock.Visibility = Visibility.Hidden;
            ReplayFileErrorImage.Visibility = Visibility.Hidden;

            ReplayFilesDataGrid.Items.Clear();
            string gameId = this.GameId;
            if (!string.IsNullOrEmpty(gameId))
            {
                try
                {
                    ObservableCollection<ReplayFileInfo> replayFiles 
                        = ReplayFile.GetReplayFiles(gameId);
                    foreach (ReplayFileInfo replayFileInfo in replayFiles) 
                    {
                        ReplayFilesDataGrid.Items.Add(replayFileInfo);
                    }
                }
                catch (Exception)
                {
                    System.Media.SystemSounds.Hand.Play();

                    BlurEffect blurEffect = new()
                    {
                        Radius = 7,
                        KernelType = KernelType.Gaussian
                    };
                    ReplayFilesDataGrid.Effect = blurEffect;
                    ReplayFilesDataGrid.IsEnabled = false;
                    AddReplayFileButton.IsEnabled = false;
                    RenameReplayFileButton.IsEnabled = false;
                    DeleteReplayFileButton.IsEnabled = false;
                    CopyReplayFileButton.IsEnabled = false;
                    MoveReplayFileButton.IsEnabled = false;
                    ReplayFileErrorBlock.Text = "エラー:リプレイファイルの取得に失敗しました。";
                    ReplayFileErrorBlock.Visibility = Visibility.Visible;
                    ReplayFileErrorImage.Visibility = Visibility.Visible;
                }
            }
        }

        private void ApplyScoreViewFilter()
        {
            ScoreDataGrid.Items.Clear();
            SpellCardDataGrid.Items.Clear();
            SpellPracticeDataGrid.Items.Clear();

            ScoreFilter scoreFilter = new()
            {
                Level = this.FilterLevel,
                Player = this.FilterPlayer,
            };

            IEnumerable<ScoreRecordData>? filteredScoreDataRecords = ScoreData.RetrieveScoreData(scoreFilter);
            if (filteredScoreDataRecords != null &&
                filteredScoreDataRecords.Count() >= 0)
            {
                ScoreDataGrid.AutoGenerateColumns = false;
                foreach (ScoreRecordData scoreRecordData in filteredScoreDataRecords)
                {
                    ScoreDataGrid.Items.Add(scoreRecordData);
                }
            }

            SpellCardRecordFilter spellCardRecordFilter = new()
            {
                Player = this.FilterSpellCardPlayer,
                Enemy = this.FilterEnemy
            };
            IEnumerable<SpellCardRecordData>? filteredSpellCardRecordLists
                = ScoreData.RetrieveSpellCardData(spellCardRecordFilter);

            if (filteredSpellCardRecordLists != null &&
                filteredSpellCardRecordLists.Count() >= 0)
            {
                SpellCardDataGrid.AutoGenerateColumns = false;
                if (DisplayUnchallengedCardMenuItem.IsChecked)
                {
                    foreach (SpellCardRecordData spellCardRecordData in filteredSpellCardRecordLists)
                    {
                        SpellCardDataGrid.Items.Add(spellCardRecordData);
                    }
                }
                else
                {
                    foreach (SpellCardRecordData spellCardRecordData in filteredSpellCardRecordLists)
                    {
                        if (int.Parse(spellCardRecordData.TryCount) > 0)
                        {
                            SpellCardDataGrid.Items.Add(spellCardRecordData);
                        }
                        else if (!ExcludeUntriedCardDataMenuItem.IsChecked)
                        {
                            SpellCardDataGrid.Items.Add(new SpellCardRecordData()
                            {
                                CardID = spellCardRecordData.CardID,
                                CardName = "------------------------",
                                GetCount = spellCardRecordData.GetCount,
                                TryCount = spellCardRecordData.TryCount,
                                Rate = spellCardRecordData.Rate,
                                Level = spellCardRecordData.Level,
                                Place = spellCardRecordData.Place,
                                Enemy = spellCardRecordData.Enemy
                            });
                        }
                    }
                }
            }

            SpellCardRecordFilter spellPracticeRecordFilter = new()
            {
                Player = "ALL",
                Enemy = this.FilterPracticeEnemy
            };
            IEnumerable<SpellCardRecordData>? filteredSpellPracticeRecordLists
                = ScoreData.RetrieveSpellPracticeData(spellPracticeRecordFilter);

            if (filteredSpellPracticeRecordLists != null &&
                filteredSpellPracticeRecordLists.Count() >= 0)
            {
                SpellPracticeDataGrid.AutoGenerateColumns = false;
                if (DisplayUnchallengedCardMenuItem.IsChecked)
                {
                    foreach (SpellCardRecordData spellPracticeRecordData in filteredSpellPracticeRecordLists)
                    {
                        SpellPracticeDataGrid.Items.Add(spellPracticeRecordData);
                    }
                }
                else
                {
                    foreach (SpellCardRecordData spellPracticeRecordData in filteredSpellPracticeRecordLists)
                    {
                        if (int.Parse(spellPracticeRecordData.TryCount) > 0)
                        {
                            SpellPracticeDataGrid.Items.Add(spellPracticeRecordData);
                        }
                        else if (!ExcludeUntriedCardDataMenuItem.IsChecked)
                        {
                            SpellPracticeDataGrid.Items.Add(new SpellCardRecordData()
                            {
                                CardID = spellPracticeRecordData.CardID,
                                CardName = "------------------------",
                                GetCount = spellPracticeRecordData.GetCount,
                                TryCount = spellPracticeRecordData.TryCount,
                                Rate = spellPracticeRecordData.Rate,
                                Place = spellPracticeRecordData.Place,
                                Enemy = spellPracticeRecordData.Enemy
                            });
                        }
                    }
                }
            }
        }

        private void ShowScoreDataViewerDialog(DialogMode mode, bool enabled, string? message)
        {
            if (mode == DialogMode.Information)
            {
                ScoreDataErrorImage1.Source =
                    new BitmapImage(
                        new Uri("pack://application:,,,/ThGameMgr.Ex;component/Images/InformationIcon32x32.png")
                    );
                ScoreDataErrorImage2.Source =
                    new BitmapImage(
                        new Uri("pack://application:,,,/ThGameMgr.Ex;component/Images/InformationIcon32x32.png")
                    );
                ScoreDataErrorImage3.Source =
                    new BitmapImage(
                        new Uri("pack://application:,,,/ThGameMgr.Ex;component/Images/InformationIcon32x32.png")
                    );
            }
            else
            {
                ScoreDataErrorImage1.Source =
                    new BitmapImage(
                        new Uri("pack://application:,,,/ThGameMgr.Ex;component/Images/ErrorIcon32x32.png")
                    );
                ScoreDataErrorImage2.Source =
                    new BitmapImage(
                        new Uri("pack://application:,,,/ThGameMgr.Ex;component/Images/ErrorIcon32x32.png")
                    );
                ScoreDataErrorImage3.Source =
                    new BitmapImage(
                        new Uri("pack://application:,,,/ThGameMgr.Ex;component/Images/ErrorIcon32x32.png")
                    );
            }

            if (enabled)
            {
                if (mode == DialogMode.Error)
                {
                    System.Media.SystemSounds.Hand.Play();
                }

                BlurEffect blurEffect = new()
                {
                    Radius = 7,
                    KernelType = KernelType.Gaussian
                };

                ScoreDataGrid.Effect = blurEffect;
                ScoreDataGrid.IsEnabled = false;
                SpellCardDataGrid.Effect = blurEffect;
                SpellCardDataGrid.IsEnabled = false;
                SpellPracticeDataGrid.Effect = blurEffect;
                SpellPracticeDataGrid.IsEnabled = false;

                ScoreDataErrorImage1.Visibility = Visibility.Visible;
                ScoreDataErrorBlock1.Text = message;
                ScoreDataErrorBlock1.Visibility = Visibility.Visible;

                ScoreDataErrorImage2.Visibility = Visibility.Visible;
                ScoreDataErrorBlock2.Text = message;
                ScoreDataErrorBlock2.Visibility = Visibility.Visible;

                ScoreDataErrorImage3.Visibility = Visibility.Visible;
                ScoreDataErrorBlock3.Text = message;
                ScoreDataErrorBlock3.Visibility = Visibility.Visible;
            }
            else
            {
                ScoreDataGrid.Effect = null;
                ScoreDataGrid.IsEnabled = true;
                SpellCardDataGrid.Effect = null;
                SpellCardDataGrid.IsEnabled = true;
                SpellPracticeDataGrid.Effect = null;
                SpellPracticeDataGrid.IsEnabled = true;

                ScoreDataErrorImage1.Visibility = Visibility.Hidden;
                ScoreDataErrorBlock1.Visibility = Visibility.Hidden;
                ScoreDataErrorImage2.Visibility = Visibility.Hidden;
                ScoreDataErrorBlock2.Visibility = Visibility.Hidden;
                ScoreDataErrorImage3.Visibility = Visibility.Hidden;
                ScoreDataErrorBlock3.Visibility = Visibility.Hidden;
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

                    System.Drawing.Icon? gameIcon = GameFile.GetGameIcon(gameId);
                    if (gameIcon != null)
                    {
                        gameMenuItem.Icon = new Image
                        {
                            //GetHbitmap()で Bitmap のハンドルを取得する
                            // CreateBitmapSourceFromHBitmap()で System.Windows.Media.Imaging.BitmapSource に変換する
                            Source = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                                gameIcon.ToBitmap().GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions())
                        };
                    }

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
                    SetSpellCardPlayerSwitchMenu();
                    SetEnemiesFilterMenu();
                    SetPracticeEnemiesFilterMenu();
                    SetLevelFilterMenu();

                    GetScoreData();
                    GetReplayFiles();
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
            if (!string.IsNullOrEmpty(gamePlayers))
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
            ApplyScoreViewFilter();
        }

        private void SetSpellCardPlayerSwitchMenu()
        {
            SpellCardPlayersSwitchButtonContextMenu.Items.Clear();

            MenuItem allItem = new()
            {
                Header = "ALL"
            };
            allItem.Click += new RoutedEventHandler(SpellCardPlayerSwitchMenuClick);
            SpellCardPlayersSwitchButtonContextMenu.Items.Add(allItem);

            Separator separator = new();
            SpellCardPlayersSwitchButtonContextMenu.Items.Add(separator);

            string gameId = this.GameId;
            string gamePlayers = GamePlayers.GetGamePlayers(gameId);
            if (!string.IsNullOrEmpty(gamePlayers))
            {
                string[] gamePlayersList = gamePlayers.Split(',');
                foreach (string gamePlayer in gamePlayersList)
                {
                    MenuItem item = new()
                    {
                        Header = gamePlayer
                    };
                    item.Click += new RoutedEventHandler(SpellCardPlayerSwitchMenuClick);
                    SpellCardPlayersSwitchButtonContextMenu.Items.Add(item);
                }
            }
        }

        private void SpellCardPlayerSwitchMenuClick(object sender, RoutedEventArgs e)
        {
            string playerName = ((MenuItem)sender).Header.ToString();
            this.FilterSpellCardPlayer = playerName;
            ApplyScoreViewFilter();
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
            if (!string.IsNullOrEmpty(gameEnemies))
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
            ApplyScoreViewFilter();
        }

        private void SetPracticeEnemiesFilterMenu()
        {
            PracticeEnemiesFilterButtonContextMenu.Items.Clear();

            MenuItem allItem = new()
            {
                Header = "ALL"
            };
            allItem.Click += new RoutedEventHandler(PracticeEnemiesFilterMenuClick);
            PracticeEnemiesFilterButtonContextMenu.Items.Add(allItem);

            Separator separator = new();
            PracticeEnemiesFilterButtonContextMenu.Items.Add(separator);

            string gameId = this.GameId;
            string gameEnemies = GameEnemies.GetGameEnemies(gameId);
            if (!string.IsNullOrEmpty(gameEnemies))
            {
                string[] gameEnemiesList = gameEnemies.Split(',');
                foreach (string gameEnemy in gameEnemiesList)
                {
                    MenuItem item = new()
                    {
                        Header = gameEnemy
                    };
                    item.Click += new RoutedEventHandler(PracticeEnemiesFilterMenuClick);
                    PracticeEnemiesFilterButtonContextMenu.Items.Add(item);
                }
            }
        }

        private void PracticeEnemiesFilterMenuClick(object sender, RoutedEventArgs e)
        {
            string enemyName = ((MenuItem)sender).Header.ToString();
            this.FilterPracticeEnemy = enemyName;
            ApplyScoreViewFilter();
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

            string[] gameLevelList = ["Easy", "Normal", "Hard", "Lunatic", "Extra"];
            foreach (string gameLevel in gameLevelList)
            {
                MenuItem item = new()
                {
                    Header = gameLevel
                };
                item.Click += new RoutedEventHandler(LevelFilterMenuClick);
                LevelFilterButtonContextMenu.Items.Add(item);
            }

            if (this.GameId == GameIndex.Th07)
            {
                MenuItem item = new()
                {
                    Header = "Phantasm"
                };
                item.Click += new RoutedEventHandler(LevelFilterMenuClick);
                LevelFilterButtonContextMenu.Items.Add(item);
            }
        }

        private void LevelFilterMenuClick(object sender, RoutedEventArgs e)
        {
            string level = ((MenuItem)sender).Header.ToString();
            this.FilterLevel = level;
            ApplyScoreViewFilter();
        }

        private void SetExternalToolsMenu()
        {
            ExternalToolsMenu.Items.Clear();

            string exToolsConfig = $"{User.CurrentUserDirectoryPath}\\Settings\\ExternalTools.xml";
            if (File.Exists(exToolsConfig))
            {
                try
                {
                    XmlDocument exToolsConfigXml = new();
                    exToolsConfigXml.Load(exToolsConfig);
                    XmlNodeList exToolsNodeList = exToolsConfigXml.SelectNodes("ExternalTools/ExternalTool");
                    if (exToolsNodeList.Count > 0)
                    {
                        foreach (XmlNode toolNode in exToolsNodeList)
                        {
                            string toolName = toolNode.SelectSingleNode("Name").InnerText;

                            MenuItem item = new()
                            {
                                Header = toolName
                            };
                            item.Click += new RoutedEventHandler(StartExternalToolMenuItem);
                            ExternalToolsMenu.Items.Add(item);
                        }
                    }
                    else
                    {
                        MenuItem item = new()
                        {
                            Header = "(なし)",
                            IsEnabled = false
                        };
                        ExternalToolsMenu.Items.Add(item);
                    }
                }
                catch (Exception)
                {
                    MenuItem item = new()
                    {
                        Header = "外部ツール読み込みに失敗",
                        IsEnabled = false
                    };
                    ExternalToolsMenu.Items.Add(item);
                }
            }
            else
            {
                MenuItem item = new()
                {
                    Header = "(なし)",
                    IsEnabled = false
                };
                ExternalToolsMenu.Items.Add(item);
            }
        }

        private void StartExternalToolMenuItem(object sender, RoutedEventArgs e)
        {
            //MenuItemのHeaderプロパティを取得
            string name = ((MenuItem)sender).Header.ToString();
            try
            {
                ExternalTool.Start(name);
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, "エラー",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
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

                SetExternalToolsMenu();

                CurrentUserStatusBarItem.Content = User.CurrentUserName;
            }
            else
            {
                MessageBox.Show("有効なユーザーが存在しないので、続行できません。\nアプリケーションを終了します。",
                    VersionInfo.AppName,
                    MessageBoxButton.OK, MessageBoxImage.Exclamation);
                Environment.Exit(0);
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
                        Environment.Exit(0);
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
                    Environment.Exit(0);
                }
            }
        }

        private void ConfigureSettings()
        {
            SettingsConfigurator.ConfigureGamePathSettings();
            SettingsConfigurator.ConfigureGameSpecificConfig();

            string defaultGameId;
            try
            {
                defaultGameId = SettingsConfigurator.ConfigureDefaultGameSettings();
            }
            catch (Exception)
            {
                defaultGameId = string.Empty;
            }

            List<string> enabledGamesList = GameIndex.GetEnabledGamesList();

            MainWindowSettings mainWindowSettings = SettingsConfigurator.ConfigureMainWindowSettings();

            this.Width = mainWindowSettings.MainWindowWidth;
            this.Height = mainWindowSettings.MainWindowHeight;
            if (!string.IsNullOrEmpty(defaultGameId) && enabledGamesList.Contains(defaultGameId))
            {
                this.GameId = defaultGameId;
            }
            else
            {
                this.GameId = mainWindowSettings.SelectedGameId;
            }
            DisplayUnchallengedCardMenuItem.IsChecked = mainWindowSettings.DisplayUnchallengedCard;
            if (DisplayUnchallengedCardMenuItem.IsChecked)
            {
                ExcludeUntriedCardDataMenuItem.IsChecked = false;
                ExcludeUntriedCardDataMenuItem.IsEnabled = false;
            }
            else
            {
                ExcludeUntriedCardDataMenuItem.IsChecked = mainWindowSettings.ExcludeUnchallengedCardData;
                ExcludeUntriedCardDataMenuItem.IsEnabled = true;
            }

            AutoBackupMenuItem.IsChecked = mainWindowSettings.AutoBackup;

            SetGameSelectionMenu();
            SetPlayersFilterMenu();
            SetSpellCardPlayerSwitchMenu();
            SetEnemiesFilterMenu();
            SetPracticeEnemiesFilterMenu();
            SetLevelFilterMenu();
            SetExternalToolsMenu();

            AutoStartWindowResizerCheckBox.IsChecked = GameSpecificSettings.GetAutoResizerConfig(this.GameId);
            CurrentUserStatusBarItem.Content = User.CurrentUserName;

            GetScoreData();
            GetReplayFiles();
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
                DisplayUnchallengedCard = DisplayUnchallengedCardMenuItem.IsChecked,
                ExcludeUnchallengedCardData = ExcludeUntriedCardDataMenuItem.IsChecked,
                AutoBackup = AutoBackupMenuItem.IsChecked
            };

            SettingsConfigurator.SaveMainWindowSettings(mainWindowSettings);
        }

        private void ConfigurePlugins()
        {
            try
            {
                if (Directory.Exists(PathInfo.PluginDirectory))
                {
                    PluginHandler.GetPlugins();

                    if (PluginHandler.StartGamePlugins != null && PluginHandler.StartGamePlugins.Count > 0)
                        SetStartGamePluginMenu(PluginHandler.StartGamePlugins);

                    if (PluginHandler.GameFilesPlugins != null && PluginHandler.GameFilesPlugins.Count > 0)
                        SetGameFilesPluginMenu(PluginHandler.GameFilesPlugins);

                    if (PluginHandler.SelectedGamePlugins != null && PluginHandler.SelectedGamePlugins.Count > 0)
                        SetSelectedGamePluginMenu(PluginHandler.SelectedGamePlugins);

                    if (PluginHandler.ScoreRecordsPlugins != null && PluginHandler.ScoreRecordsPlugins.Count > 0)
                        SetScoreRecordsPluginMenu(PluginHandler.ScoreRecordsPlugins);

                    if (PluginHandler.SpellCardRecordsPlugins != null
                        && PluginHandler.SpellCardRecordsPlugins.Count > 0)
                        SetSpellCardRecordsPluginMenu(PluginHandler.SpellCardRecordsPlugins);

                    if (PluginHandler.AllScoreRecordsPlugins != null
                        && PluginHandler.AllScoreRecordsPlugins.Count > 0)
                        SetAllScoreRecordsPluginMenu(PluginHandler.AllScoreRecordsPlugins);

                    if (PluginHandler.ToolPlugins != null && PluginHandler.ToolPlugins.Count > 0)
                        SetToolPluginMenu(PluginHandler.ToolPlugins);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"プラグインの読み込みに失敗しました。\n{ex.Message}",
                    "エラー",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SetStartGamePluginMenu(List<dynamic> startGamePlugins)
        {
            int i = 3;
            foreach (dynamic startGamePlugin in startGamePlugins)
            {
                try
                {
                    startGamePlugin.MainWindow = this;
                }
                catch (Exception) { }

                MenuItem menuItem = new()
                {
                    Header = startGamePlugin.CommandName
                };

                menuItem.Click += (object sender, RoutedEventArgs e) =>
                {
                    string? gameId = this.GameId;
                    if (!string.IsNullOrEmpty(gameId))
                    {
                        string gameFilePath = GameFile.GetGameFilePath(gameId);

                        EnableGameEndWaitingLimitationMode(true);
                        SetStartGameStatus("ゲームの起動を待機中...");
                        try
                        {
                            Process gameProcess = startGamePlugin.Main(gameId,  gameFilePath);
                            if (gameProcess.ProcessName == Path.GetFileNameWithoutExtension(gameFilePath))
                            {
                                gameProcess.WaitForInputIdle();
                                StartGameEndWaitingMode(gameProcess);
                            }
                            else
                            {
                                MessageBox.Show(this,
                                    $"プラグインによって起動されたプロセスがゲームのものではありません。\n{startGamePlugin.Name}",
                                    "プラグインによる実行",
                                    MessageBoxButton.OK,
                                    MessageBoxImage.Exclamation);
                                EnableGameEndWaitingLimitationMode(false);
                                SetStartGameStatus(string.Empty);
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(this, 
                                $"プラグインによるゲーム実行に失敗しました。\n{startGamePlugin.Name}\n{ex.Message}",
                                "エラー",
                                MessageBoxButton.OK, MessageBoxImage.Error);
                            EnableGameEndWaitingLimitationMode(false);
                            SetStartGameStatus(string.Empty);
                        }
                    }
                };

                GameMenu.Items.Insert(i, menuItem);
                i++;
            }
        }

        private void SetGameFilesPluginMenu(List<dynamic> gameFilesPlugins)
        {
            Separator separator = new();
            GameMenu.Items.Add(separator);
            foreach (dynamic gameFilesPlugin in gameFilesPlugins)
            {
                try
                {
                    gameFilesPlugin.MainWindow = this;
                }
                catch (Exception) { }

                MenuItem menuItem = new()
                {
                    Header = gameFilesPlugin.CommandName
                };

                menuItem.Click += (object sender, RoutedEventArgs e) =>
                {
                    List<string> availableGamesList = GameIndex.GetEnabledGamesList();
                    Dictionary<string, string> availableGameFilesDictionary = [];
                    if (availableGamesList.Count > 0)
                    {
                        foreach (string gameId in availableGamesList)
                        {
                            availableGameFilesDictionary.Add(gameId, GameFile.GetGameFilePath(gameId));
                        }
                    }

                    try
                    {
                        gameFilesPlugin.Main(availableGamesList, availableGameFilesDictionary);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(this, ex.Message, "エラー",
                            MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                };

                GameMenu.Items.Add(menuItem);
            }
        }

        private void SetSelectedGamePluginMenu(List<dynamic> selectedGamePlugins)
        {
            Separator separator = new();
            GameMenu.Items.Add(separator);
            foreach (dynamic selectedGamePlugin in selectedGamePlugins)
            {
                try
                {
                    selectedGamePlugin.MainWindow = this;
                }
                catch (Exception) { }

                MenuItem menuItem = new()
                {
                    Header = selectedGamePlugin.CommandName
                };

                menuItem.Click += (object sender, RoutedEventArgs e) =>
                {
                    try
                    {
                        if (!string.IsNullOrEmpty(this.GameId))
                        {
                            string gameFilePath = GameFile.GetGameFilePath(this.GameId);
                            selectedGamePlugin.Main(this.GameId, gameFilePath);
                        }
                        else
                        {
                            MessageBox.Show(this, "ゲームが選択されていません。", "プラグインの実行",
                                            MessageBoxButton.OK, MessageBoxImage.Exclamation);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(this, ex.Message, "エラー",
                                        MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                };

                GameMenu.Items.Add(menuItem);
            }
        }

        private void SetScoreRecordsPluginMenu(List<dynamic> scoreRecordsPlugins)
        {
            foreach (dynamic scoreRecordsPlugin in scoreRecordsPlugins)
            {
                try
                {
                    scoreRecordsPlugin.MainWindow = this;
                }
                catch (Exception) { }

                MenuItem menuItem = new()
                {
                    Header = scoreRecordsPlugin.CommandName
                };

                menuItem.Click += (object sender, RoutedEventArgs e) =>
                {
                    try
                    {
                        if (!string.IsNullOrEmpty(this.GameId) && ScoreData.ScoreRecordLists.Count > 0)
                        {
                            scoreRecordsPlugin.Main(this.GameId, ScoreData.ScoreRecordLists);
                        }
                        else
                        {
                            MessageBox.Show(this, "利用可能なデータがありません。", "プラグインの実行",
                                MessageBoxButton.OK, MessageBoxImage.Exclamation);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(this, ex.Message, "エラー",
                                        MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                };

                ScoreMenu.Items.Add(menuItem);
            }
        }

        private void SetSpellCardRecordsPluginMenu(List<dynamic> spellCardRecordsPlugins)
        {
            foreach (dynamic spellCardRecordsPlugin in spellCardRecordsPlugins)
            {
                try
                {
                    spellCardRecordsPlugin.MainWindow = this;
                }
                catch (Exception) { }

                MenuItem menuItem = new()
                {
                    Header = spellCardRecordsPlugin.CommandName
                };

                menuItem.Click += (object sender, RoutedEventArgs e) =>
                {
                    try
                    {
                        if (!string.IsNullOrEmpty(this.GameId) && ScoreData.SpellCardRecordLists.Count > 0)
                        {
                            spellCardRecordsPlugin.Main(this.GameId, ScoreData.SpellCardRecordLists);
                        }
                        else
                        {
                            MessageBox.Show(this, "利用可能なデータがありません。", "プラグインの実行",
                                MessageBoxButton.OK, MessageBoxImage.Exclamation);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(this, ex.Message, "エラー",
                                        MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                };

                ScoreMenu.Items.Add(menuItem);
            }
        }

        private void SetAllScoreRecordsPluginMenu(List<dynamic> allScoreRecordsPlugins)
        {
            foreach (dynamic allScoreRecordsPlugin in allScoreRecordsPlugins)
            {
                try
                {
                    allScoreRecordsPlugin.MainWindow = this;
                }
                catch (Exception) { }

                MenuItem menuItem = new()
                {
                    Header = allScoreRecordsPlugin.CommandName
                };

                menuItem.Click += (object sender, RoutedEventArgs e) =>
                {
                    try
                    {
                        if (!string.IsNullOrEmpty(this.GameId) && 
                            ScoreData.ScoreRecordLists.Count > 0 &&
                            ScoreData.SpellCardRecordLists.Count > 0)
                        {
                            allScoreRecordsPlugin.Main(
                                this.GameId, ScoreData.ScoreRecordLists, ScoreData.SpellCardRecordLists);
                        }
                        else
                        {
                            MessageBox.Show(this, "利用可能なデータがありません。", "プラグインの実行",
                                MessageBoxButton.OK, MessageBoxImage.Exclamation);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(this, ex.Message, "エラー",
                                        MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                };

                ScoreMenu.Items.Add(menuItem);
            }
        }

        private void SetToolPluginMenu(List<dynamic> toolPlugins)
        {
            foreach (dynamic toolPlugin in toolPlugins)
            {
                try
                {
                    toolPlugin.MainWindow = this;
                }
                catch (Exception) { }

                MenuItem menuItem = new()
                {
                    Header = toolPlugin.CommandName
                };

                menuItem.Click += (object sender, RoutedEventArgs e) =>
                {
                    try
                    {
                        toolPlugin.Main();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(this, ex.Message, "エラー",
                                        MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                };

                ToolMenu.Items.Add(menuItem);
            }
        }

        private void SetStartGameStatus(string message)
        {
            StartGameStatusBarItem.Content = message;
        }

        /// <summary>
        /// ゲーム終了待機中の機能制限モードを有効化します。
        /// </summary>
        /// <param name="enabled"></param>
        private void EnableGameEndWaitingLimitationMode(bool enabled)
        {
            AddUserMenuItem.IsEnabled = !enabled;
            SwitchUserMenuItem.IsEnabled = !enabled;
            GameMenu.IsEnabled = !enabled;
            ScoreMenu.IsEnabled = !enabled;

            StartWindowResizerMenuItem.IsEnabled = enabled;

            GameSelectionButton.IsEnabled = !enabled;
            StartGameButton.IsEnabled = !enabled;
            StartGameWithVpatchButton.IsEnabled = !enabled;
            StartGameWithThpracButton.IsEnabled = !enabled;
            StartCustomProgramButton.IsEnabled = !enabled;
            GameAudioControlSlider.IsEnabled = enabled;
        }

        /// <summary>
        /// スコアデータ取得中の機能制限モードを有効化します。
        /// </summary>
        /// <param name="enabled"></param>
        private void EnableRetrievingScoreDataLimitationMode(bool enabled)
        {
            AddUserMenuItem.IsEnabled = !enabled;
            SwitchUserMenuItem.IsEnabled = !enabled;
            GameMenu.IsEnabled = !enabled;
            ScoreMenu.IsEnabled = !enabled;

            GameSelectionButton.IsEnabled = !enabled;
            StartGameButton.IsEnabled = !enabled;
            StartGameWithVpatchButton.IsEnabled = !enabled;
            StartGameWithThpracButton.IsEnabled = !enabled;
            StartCustomProgramButton.IsEnabled = !enabled;
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
            SetStartGameStatus("ゲームが実行中..");

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
                                Owner = this,
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

            GamePlayLogData gamePlayLogData = new()
            {
                GameId = this.GameId,
                GameName = this.GameName,
                GameStartTime = this.GameStartTime.ToString("yyyy/MM/dd HH:mm:ss"),
                GameEndTime = gameEndTime.ToString("yyyy/MM/dd HH:mm:ss"),
                GameRunningTime = runningTimeSpan.ToString("mm\\:ss")
            };

            try
            {
                GamePlayLogRecorder.SaveGamePlayLog(gamePlayLogData);
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, $"ゲーム実行履歴の保存に失敗しました。\n{ex.Message}", "エラー",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }

            if (_resizerFrameWindow != null && _resizerFrameWindow.IsLoaded)
            {
                _resizerFrameWindow.Close();
            }
            _resizerFrameWindow = null;

            if (AutoBackupMenuItem.IsChecked)
            {
                try
                {
                    ScoreBackup.Create(this.GameId);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(this, $"スコアファイルのバックアップの作成に失敗しました。\n{ex.Message}", "エラー",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }

            EnableGameEndWaitingLimitationMode(false);
            SetStartGameStatus(string.Empty);
            
            GetScoreData();
            GetReplayFiles();
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

            GetScoreData();
        }

        private void StartGameMenuItemClick(object sender, RoutedEventArgs e)
        {
            StartGame();
        }

        private void StartGameWithVpatchMenuItemClick(object sender, RoutedEventArgs e)
        {
            StartGameWithApplyingTool("vpatch.exe");
        }

        private void StartGameWithThpracMenuItemClick(object sender, RoutedEventArgs e)
        {
            string gameId = this.GameId;
            if (!string.IsNullOrEmpty(gameId))
            {
                List<string> thpracFiles = GameFile.GetThpracFiles(gameId);
                if (thpracFiles.Count == 0)
                {
                    MessageBox.Show(this, "利用可能な thprac 実行ファイルが存在しません。", "thprac の適用",
                        MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
                else if (thpracFiles.Count == 1)
                {
                    StartGameWithApplyingTool(thpracFiles[0]);
                }
                else
                {
                    SelectThpracDialog selectThpracDialog = new()
                    {
                        ThpracFiles = thpracFiles,
                        Owner = this
                    };

                    if (selectThpracDialog.ShowDialog() == true)
                    {
                        string? thpracFile = selectThpracDialog.SelectedThpracFile;
                        if (!string.IsNullOrEmpty(thpracFile))
                        {
                            StartGameWithApplyingTool(thpracFile);
                        }
                        else
                        {
                            MessageBox.Show(this, "選択された thprac 実行ファイルが不正です。", "エラー",
                                MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                }
            }
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
                GameSpecificSettings.SetAutoResizerConfig(
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

        private void SpellCardPlayersSwitchButtonClick(object sender, RoutedEventArgs e)
        {
            if (!SpellCardPlayersSwitchButtonContextMenu.IsOpen)
            {
                SpellCardPlayersSwitchButtonContextMenu.PlacementTarget = SpellCardPlayersSwitchButton;
                SpellCardPlayersSwitchButtonContextMenu.Placement = System.Windows.Controls.Primitives.PlacementMode.Bottom;
                SpellCardPlayersSwitchButtonContextMenu.IsOpen = true;
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

        private void PracticeEnemiesFilterButtonClick(object sender, RoutedEventArgs e)
        {
            if (!PracticeEnemiesFilterButtonContextMenu.IsOpen)
            {
                PracticeEnemiesFilterButtonContextMenu.PlacementTarget = PracticeEnemiesFilterButton;
                PracticeEnemiesFilterButtonContextMenu.Placement = System.Windows.Controls.Primitives.PlacementMode.Bottom;
                PracticeEnemiesFilterButtonContextMenu.IsOpen = true;
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
            if (DisplayUnchallengedCardMenuItem.IsChecked)
            {
                ExcludeUntriedCardDataMenuItem.IsChecked = false;
                ExcludeUntriedCardDataMenuItem.IsEnabled = false;
            }
            else
            {
                ExcludeUntriedCardDataMenuItem.IsEnabled = true;
            }

            ApplyScoreViewFilter();
        }

        private void ExcludeUntriedCardDataMenuItemClick(object sender, RoutedEventArgs e)
        {
            ApplyScoreViewFilter();
        }

        private void StartWindowResizerMenuItemClick(object sender, RoutedEventArgs e)
        {
            if (_resizerFrameWindow == null || !_resizerFrameWindow.IsLoaded)
            {
                _resizerFrameWindow = new()
                {
                    Owner = this,
                    GameWindow = this.GameProcess.MainWindowHandle
                };

                _resizerFrameWindow.Show();
            }
            else
            {
                _resizerFrameWindow.Activate();
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
            SpellCardStatisticsData? spellCardStatisticsData = ScoreData.AnalyzeSpellCardStatisitcs();
            if (spellCardStatisticsData != null)
            {
                SpellCardRecordStatisticDialog spellCardRecordStatisticDialog = new()
                {
                    GameId = this.GameId,
                    ChallengedCardCount = spellCardStatisticsData.TriedCardCount,
                    GetCardCount = spellCardStatisticsData.GetCardCount,
                    GetCardRate = spellCardStatisticsData.GetCardCountRate,
                    TotalChallengeCount = spellCardStatisticsData.TotalTryCount,
                    TotalGetCount = spellCardStatisticsData.TotalGetCount,
                    TotalGetRate = spellCardStatisticsData.TotalGetCountRate,
                    Owner = this
                };
                spellCardRecordStatisticDialog.ShowDialog();
            }
            else
            {
                MessageBox.Show(
                    this, "利用可能なデータがありません。", "御札戦歴統計",
                    MessageBoxButton.OK, MessageBoxImage.Exclamation);
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

        private void ViewScoreRecordDetailMenuItemClick(object sender, RoutedEventArgs e)
        {
            if (ScoreDataGrid.Items.Count > 0 &&
                ScoreDataGrid.SelectedIndex > -1)
            {
                try
                {
                    ScoreRecordData scoreRecordList = (ScoreRecordData)ScoreDataGrid.SelectedItem;
                    if (_scoreRecordDetailDialog == null ||
                        !_scoreRecordDetailDialog.IsLoaded)
                    {
                        _scoreRecordDetailDialog = new()
                        {
                            Owner = this,
                            DataContext = scoreRecordList
                        };
                        _scoreRecordDetailDialog.Show();
                    }
                    else
                    {
                        _scoreRecordDetailDialog.DataContext = scoreRecordList;
                        _scoreRecordDetailDialog.WindowState = WindowState.Normal;
                        _scoreRecordDetailDialog.Activate();
                    }
                }
                catch (Exception)
                {

                }
            }
        }

        private void ScoreDataGridSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(ScoreDataGrid.Items.Count > 0 &&
                ScoreDataGrid.SelectedIndex > -1)
            {
                try
                {
                    ScoreRecordData scoreRecordList = (ScoreRecordData)ScoreDataGrid.SelectedItem;
                    if (_scoreRecordDetailDialog != null && _scoreRecordDetailDialog.IsLoaded)
                    {
                        _scoreRecordDetailDialog.DataContext = scoreRecordList;
                        _scoreRecordDetailDialog.WindowState = WindowState.Normal;
                    }
                }
                catch (Exception)
                {

                }
            }
        }

        private void ViewSpellCardRecordMenuItemClick(object sender, RoutedEventArgs e)
        {
            if (SpellCardDataGrid.Items.Count > 0 &&
                SpellCardDataGrid.SelectedIndex > -1)
            {
                try
                {
                    SpellCardRecordData spellCardRecordList = (SpellCardRecordData)SpellCardDataGrid.SelectedItem;
                    if (_spellCardRecordDetailDialog == null ||
                        !_spellCardRecordDetailDialog.IsLoaded)
                    {
                        _spellCardRecordDetailDialog = new()
                        {
                            Owner = this,
                            DataContext = spellCardRecordList,
                            Title = "御札戦歴詳細"
                        };
                        _spellCardRecordDetailDialog.Show();
                    }
                    else
                    {
                        _spellCardRecordDetailDialog.Title = "御札戦歴詳細";
                        _spellCardRecordDetailDialog.DataContext = spellCardRecordList;
                        _spellCardRecordDetailDialog.WindowState = WindowState.Normal;
                        _spellCardRecordDetailDialog.Activate();
                    }
                }
                catch (Exception)
                {

                }
            }
        }

        private void SpellCardDataGridSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SpellCardDataGrid.Items.Count > 0 &&
                SpellCardDataGrid.SelectedIndex > -1)
            {
                try
                {
                    SpellCardRecordData spellCardRecordList = (SpellCardRecordData)SpellCardDataGrid.SelectedItem;
                    if (_spellCardRecordDetailDialog != null &&
                        _spellCardRecordDetailDialog.IsLoaded)
                    {
                        _spellCardRecordDetailDialog.Title = "御札戦歴詳細";
                        _spellCardRecordDetailDialog.DataContext = spellCardRecordList;
                        _spellCardRecordDetailDialog.WindowState = WindowState.Normal;
                    }
                }
                catch (Exception)
                {

                }
            }
        }

        private void ViewSpellPracticeRecordMenuItemClick(object sender, RoutedEventArgs e)
        {
            if (SpellPracticeDataGrid.Items.Count > 0 &&
                SpellPracticeDataGrid.SelectedIndex > -1)
            {
                try
                {
                    SpellCardRecordData spellCardRecordList = (SpellCardRecordData)SpellPracticeDataGrid.SelectedItem;
                    if (_spellCardRecordDetailDialog == null ||
                        !_spellCardRecordDetailDialog.IsLoaded)
                    {
                        _spellCardRecordDetailDialog = new()
                        {
                            Owner = this,
                            DataContext = spellCardRecordList,
                            Title = "スペルプラクティス詳細"
                        };
                        _spellCardRecordDetailDialog.Show();
                    }
                    else
                    {
                        _spellCardRecordDetailDialog.Title = "スペルプラクティス詳細";
                        _spellCardRecordDetailDialog.DataContext = spellCardRecordList;
                        _spellCardRecordDetailDialog.WindowState = WindowState.Normal;
                        _spellCardRecordDetailDialog.Activate();
                    }
                }
                catch (Exception)
                {

                }
            }
        }

        private void SpellPracticeDataGridSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SpellPracticeDataGrid.Items.Count > 0 &&
                SpellPracticeDataGrid.SelectedIndex > -1)
            {
                try
                {
                    SpellCardRecordData spellCardRecordList = (SpellCardRecordData)SpellPracticeDataGrid.SelectedItem;
                    if (_spellCardRecordDetailDialog != null &&
                        _spellCardRecordDetailDialog.IsLoaded)
                    {
                        _spellCardRecordDetailDialog.Title = "スペルプラクティス詳細";
                        _spellCardRecordDetailDialog.DataContext = spellCardRecordList;
                        _spellCardRecordDetailDialog.WindowState = WindowState.Normal;
                    }
                }
                catch (Exception)
                {

                }
            }
        }

        private void CopySpellPracticeRecordMenuItemClick(object sender, RoutedEventArgs e)
        {
            if (SpellPracticeDataGrid.Items.Count > 0 &&
                SpellPracticeDataGrid.SelectedIndex > -1)
            {
                try
                {
                    SpellCardRecordData spellCardRecordList = (SpellCardRecordData)SpellPracticeDataGrid.SelectedItem;

                    string data =
                        $"No.{spellCardRecordList.CardID}\r\n{spellCardRecordList.CardName}\r\n取得数: {spellCardRecordList.GetCount}\r\n挑戦数: {spellCardRecordList.TryCount}\r\n取得率: {spellCardRecordList.Rate}\r\n発動場所: {spellCardRecordList.Place}\r\n術者: {spellCardRecordList.Enemy}";

                    Clipboard.SetText(data);
                }
                catch (Exception)
                {

                }
            }
        }

        private void AboutNAudioMenuItemClick(object sender, RoutedEventArgs e)
        {
            string naudioCoreDllPath = $"{PathInfo.AppLocation}\\NAudio.Core.dll";
            string naudioWasapiDllPath = $"{PathInfo.AppLocation}\\NAudio.Wasapi.dll";

            string naudioCoreDllName = FileVersionInfo.GetVersionInfo(naudioCoreDllPath).ProductName;
            string naudioCoreDllVersion = FileVersionInfo.GetVersionInfo(naudioCoreDllPath).FileVersion;
            string naudioCoreDllDeveloper = FileVersionInfo.GetVersionInfo(naudioCoreDllPath).CompanyName;
            string naudioCoreDllCopyright = FileVersionInfo.GetVersionInfo(naudioCoreDllPath).LegalCopyright;

            string naudioCoreDllInformation
                = $"{naudioCoreDllName}\nVersion.{naudioCoreDllVersion}\nby {naudioCoreDllDeveloper}\n{naudioCoreDllCopyright}";

            string naudioWasapiDllName = FileVersionInfo.GetVersionInfo(naudioWasapiDllPath).ProductName;
            string naudioWasapiDllVersion = FileVersionInfo.GetVersionInfo(naudioWasapiDllPath).FileVersion;
            string naudioWasapiDllDeveloper = FileVersionInfo.GetVersionInfo(naudioWasapiDllPath).CompanyName;
            string naudioWasapiDllCopyright = FileVersionInfo.GetVersionInfo(naudioWasapiDllPath).LegalCopyright;

            string naudioWasapiDllInformation
                = $"{naudioWasapiDllName}\nVersion.{naudioWasapiDllVersion}\nby {naudioWasapiDllDeveloper}\n{naudioWasapiDllCopyright}";

            MessageBox.Show(this, $"{naudioCoreDllInformation}\n\n{naudioWasapiDllInformation}",
                "NAudio について",
                MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void GamePlayLogMenuItemClick(object sender, RoutedEventArgs e)
        {
            GamePlayLogDialog gamePlayLogDialog = new()
            {
                Owner = this
            };

            gamePlayLogDialog.ShowDialog();
        }

        private void ManageExternalToolsMenuItemClick(object sender, RoutedEventArgs e)
        {
            ManageExternalToolsDialog manageExternalToolsDialog = new()
            {
                Owner = this
            };

            manageExternalToolsDialog.ShowDialog();
            SetExternalToolsMenu();
        }

        private void OpenGameDirectoryMenuItemClick(object sender, RoutedEventArgs e)
        {
            string gameId = this.GameId;
            try
            {
                GameFile.OpenGameDirectory(gameId);
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, "エラー",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void OpenLogMenuItemClick(object sender, RoutedEventArgs e)
        {
            string gameId = this.GameId;
            string logFile = GameFile.GetGameLogFile(gameId);
            if (!string.IsNullOrEmpty(logFile) && File.Exists(logFile))
            {
                TextViewer textViewer = new()
                {
                    Title = "東方動作記録",
                    FilePath = logFile,
                    Encode = "shift_jis",
                    Owner = this
                };

                textViewer.ShowDialog();
            }
            else
            {
                MessageBox.Show(this, "log.txt(東方動作記録)が存在しません。", VersionInfo.AppName,
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void OpenScoreDirectoryMenuItemClick(object sender, RoutedEventArgs e)
        {
            string gameId = this.GameId;
            try
            {
                GameFile.OpenScoreDirectory(gameId);
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, "エラー",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CreateScoreBackupSelectedGameMenuItemClick(object sender, RoutedEventArgs e)
        {
            string? gameId = this.GameId;
            if (!string.IsNullOrEmpty(gameId))
            {
                try
                {
                    bool result = ScoreBackup.Create(gameId);
                    if (result)
                    {
                        MessageBox.Show(this, "スコアファイルのバックアップを作成しました。", "スコアファイルのバックアップ",
                            MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(this,
                        $"バックアップの作成に失敗しました。\n{gameId}: {GameIndex.GetGameName(gameId)}\n{ex.Message}", "エラー",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void RestoreScoreFileMenuItemClick(object sender, RoutedEventArgs e)
        {
            ManageScoreBackupDialog manageScoreBackupDialog = new()
            {
                Owner = this
            };
            manageScoreBackupDialog.ShowDialog();

            GetScoreData();
        }

        private void FeedbackMenuItemClick(object sender, RoutedEventArgs e)
        {
            ProcessStartInfo processStartInfo = new()
            {
                FileName = "https://forms.gle/WBabN5bJJqvNdkhu5",
                UseShellExecute = true
            };

            try
            {
                Process.Start(processStartInfo);
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, "エラー",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ManagePluginsMenuItemClick(object sender, RoutedEventArgs e)
        {
            ManagePluginsDialog managePluginsDialog = new()
            {
                Owner = this
            };

            managePluginsDialog.ShowDialog();
        }

        private void AboutPluginBaseLibraryMenuItemClick(object sender, RoutedEventArgs e)
        {
            string baseLibraryDll = $"{PathInfo.AppLocation}\\ThGameMgr.Ex.Plugin.dll";

            string name = FileVersionInfo.GetVersionInfo(baseLibraryDll).ProductName;
            string version = FileVersionInfo.GetVersionInfo(baseLibraryDll).ProductVersion;
            string developer = FileVersionInfo.GetVersionInfo(baseLibraryDll).CompanyName;
            string copyright = FileVersionInfo.GetVersionInfo(baseLibraryDll).LegalCopyright;

            string message = $"{name}\nVersion.{version}\nby {developer}\n{copyright}";
            MessageBox.Show(this, message, "プラグイン基底ライブラリについて",
                MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void DeleteUserMenuItemClick(object sender, RoutedEventArgs e)
        {
            DeleteUserDialog deleteUserDialog = new()
            {
                Owner = this
            };
            deleteUserDialog.ShowDialog();
        }

        private void CopyScoreRecordMenuItemClick(object sender, RoutedEventArgs e)
        {
            if (ScoreDataGrid.Items.Count > 0 &&
                ScoreDataGrid.SelectedIndex > -1)
            {
                try
                {
                    ScoreRecordData scoreRecordList = (ScoreRecordData)ScoreDataGrid.SelectedItem;

                    string data =
                        $"スコア: {scoreRecordList.Score}\r\n自機:{scoreRecordList.Player}\r\n難易度:{scoreRecordList.Level}\r\n名前:{scoreRecordList.Name.TrimEnd('\0')}";

                    if (!string.IsNullOrEmpty(scoreRecordList.Progress))
                        data += $"\r\n到達面:{scoreRecordList.Progress}";
                    if (!string.IsNullOrEmpty(scoreRecordList.Date.TrimEnd('\0'))
                        && scoreRecordList.Date.TrimEnd('\0') != "--/--")
                        data += $"\r\n日時:{scoreRecordList.Date.TrimEnd('\0')}";
                    if (!string.IsNullOrEmpty(scoreRecordList.SlowRate)
                        && scoreRecordList.SlowRate != "-.--%")
                        data += $"\r\n処理落ち率:{scoreRecordList.SlowRate}";
                    if (!string.IsNullOrEmpty(scoreRecordList.OtherData))
                        data += $"\r\nその他\r\n{scoreRecordList.OtherData}\r\n";

                    Clipboard.SetText(data);
                }
                catch (Exception)
                {

                }
            }
        }

        private void CopySpellCardRecordMenuItemClick(object sender, RoutedEventArgs e)
        {
            if (SpellCardDataGrid.Items.Count > 0 &&
                SpellCardDataGrid.SelectedIndex > -1)
            {
                try
                {
                    SpellCardRecordData spellCardRecordList = (SpellCardRecordData)SpellCardDataGrid.SelectedItem;

                    string data =
                        $"No.{spellCardRecordList.CardID}\r\n{spellCardRecordList.CardName}\r\n取得数: {spellCardRecordList.GetCount}\r\n挑戦数: {spellCardRecordList.TryCount}\r\n取得率: {spellCardRecordList.Rate}\r\n発動場所: {spellCardRecordList.Place}\r\n術者: {spellCardRecordList.Enemy}";

                    Clipboard.SetText(data);
                }
                catch (Exception)
                {

                }
            }
        }

        private void ExportScoreDataMenuItemClick(object sender, RoutedEventArgs e)
        {
            ExportScoreDataDialog exportScoreDataDialog = new()
            {
                Owner = this
            };
            exportScoreDataDialog.ShowDialog();
        }

        private void ReloadReplayFilesButtonClick(object sender, RoutedEventArgs e)
        {
            GetReplayFiles();
        }

        private void DeleteReplayFileButtonClick(object sender, RoutedEventArgs e)
        {
            if (ReplayFilesDataGrid.Items.Count > 0
                && ReplayFilesDataGrid.SelectedIndex >= 0)
            {
                ReplayFileInfo selectedReplayFileInfo = ReplayFilesDataGrid.SelectedItem as ReplayFileInfo;
                string replayFileName = selectedReplayFileInfo.FileName;

                MessageBoxResult result = MessageBox.Show(
                    this, $"'{replayFileName}' を削除してもよろしいですか。", "リプレイファイルの削除",
                    MessageBoxButton.YesNo, MessageBoxImage.Information);
                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        ReplayFile.Delete(this.GameId, replayFileName);

                        GetReplayFiles();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(this, $"リプレイファイルの削除に失敗しました。\n{ex.Message}", "エラー",
                            MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }

        private void CopyReplayFileButtonClick(object sender, RoutedEventArgs e)
        {
            if (ReplayFilesDataGrid.Items.Count > 0 &&
                ReplayFilesDataGrid.SelectedIndex >= 0)
            {
                ReplayFileInfo selectedReplayFileInfo = ReplayFilesDataGrid.SelectedItem as ReplayFileInfo;
                string replayFileName = selectedReplayFileInfo.FileName;
                string replayDirectory = ReplayFile.GetReplayDirectory(this.GameId);

                SaveFileDialog saveFileDialog = new()
                {
                    Filter = "リプレイファイル|*.rpy",
                    FileName = replayFileName
                };

                if (saveFileDialog.ShowDialog() == true)
                {
                    try
                    {
                        File.Copy($"{replayDirectory}\\{replayFileName}", saveFileDialog.FileName, true);
                        MessageBox.Show(this,
                            $"複製しました。\n{saveFileDialog.FileName}", "リプレイファイルの複製",
                            MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(this, $"リプレイファイルの複製に失敗しました。\n{ex.Message}", "エラー",
                            MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }

        private void MoveReplayFileButtonClick(object sender, RoutedEventArgs e)
        {
            if (ReplayFilesDataGrid.Items.Count > 0 &&
                ReplayFilesDataGrid.SelectedIndex >= 0)
            {
                ReplayFileInfo selectedReplayFileInfo = ReplayFilesDataGrid.SelectedItem as ReplayFileInfo;
                string replayFileName = selectedReplayFileInfo.FileName;
                string replayDirectory = ReplayFile.GetReplayDirectory(this.GameId);

                SaveFileDialog saveFileDialog = new()
                {
                    Filter = "リプレイファイル|*.rpy",
                    FileName = replayFileName
                };

                if (saveFileDialog.ShowDialog() == true)
                {
                    try
                    {
                        File.Move($"{replayDirectory}\\{replayFileName}", saveFileDialog.FileName, true);
                        MessageBox.Show(this,
                            $"移動しました。\n{saveFileDialog.FileName}", "リプレイファイルの移動",
                            MessageBoxButton.OK, MessageBoxImage.Information);

                        GetReplayFiles();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(this, $"リプレイファイルの移動に失敗しました。\n{ex.Message}", "エラー",
                            MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }

        private void RenameReplayFileButtonClick(object sender, RoutedEventArgs e)
        {
            if (ReplayFilesDataGrid.Items.Count > 0
                && ReplayFilesDataGrid.SelectedIndex >= 0)
            {
                ReplayFileInfo selectedReplayFileInfo = ReplayFilesDataGrid.SelectedItem as ReplayFileInfo;
                string replayFileName = selectedReplayFileInfo.FileName;

                RenameReplayFileDialog renameDialog = new()
                {
                    ReplayFileName = Path.GetFileNameWithoutExtension(replayFileName),
                    Owner = this
                };

                if (renameDialog.ShowDialog() == true)
                {
                    string newReplayFileName = $"{renameDialog.ReplayFileName}.rpy";

                    if (!ReplayFile.Exists(this.GameId, newReplayFileName))
                    {
                        try
                        {
                            ReplayFile.Rename(this.GameId, replayFileName, newReplayFileName);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(this, $"リプレイファイルのリネームに失敗しました。\n{ex.Message}", "エラー",
                                MessageBoxButton.OK, MessageBoxImage.Error);
                        }

                        GetReplayFiles();
                    }
                    else
                    {
                        MessageBox.Show(this, $"'{newReplayFileName}' は既に存在します。", "リプレイファイルのリネーム",
                            MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    }
                }
            }
        }

        private void AddReplayFileButtonClick(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new()
            {
                Filter = "リプレイファイル|*.rpy|すべてのファイル|*.*"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                string gameId = this.GameId;
                
                string replayDirectory = ReplayFile.GetReplayDirectory(gameId);
                string replayFile = openFileDialog.FileName;
                string newReplayFile = $"{replayDirectory}\\{Path.GetFileName(replayFile)}";
                try
                {
                    while (File.Exists(newReplayFile))
                    {
                        MessageBox.Show(this,
                            "追加先の replay フォルダに、同名のリプレイファイルが存在します。\n名前を変更してください。",
                            "リプレイファイルの追加",
                            MessageBoxButton.OK,
                            MessageBoxImage.Exclamation);
                        RenameReplayFileDialog renameDialog = new()
                        {
                            Owner = this,
                            ReplayFileName = Path.GetFileNameWithoutExtension(newReplayFile)
                        };

                        if (renameDialog.ShowDialog() == true)
                        {
                            string newReplayFileName = $"{renameDialog.ReplayFileName}.rpy";
                            newReplayFile = $"{replayDirectory}\\{newReplayFileName}";
                        }
                        else
                        {
                            throw new InvalidOperationException(
                                "新しい名前が指定されませんでした。");
                        }
                    }

                    File.Copy(replayFile, newReplayFile);
                    GetReplayFiles();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(this, $"リプレイファイルを追加できませんでした。\n{ex.Message}", "エラー",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void DefaultGameSettingsMenuItemClick(object sender, RoutedEventArgs e)
        {
            DefaultGameSettingsDialog defaultGameSettingsDialog = new()
            {
                Owner = this
            };
            defaultGameSettingsDialog.ShowDialog();
        }

        private void DistributionSiteMenuItemClick(object sender, RoutedEventArgs e)
        {
            ProcessStartInfo processStartInfo = new()
            {
                FileName = "https://mashironn.net/ThGameMgr/Ex/",
                UseShellExecute = true
            };

            try
            {
                Process.Start(processStartInfo);
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, "エラー",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}