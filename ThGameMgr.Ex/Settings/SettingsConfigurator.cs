using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;

namespace ThGameMgr.Ex.Settings
{
    class SettingsConfigurator
    {
        public static void SaveGamePathSettings()
        {
            if (!string.IsNullOrEmpty(User.CurrentUserDirectoryPath))
            {
                string? settingsDirectory = $"{User.CurrentUserDirectoryPath}\\Settings";
                string? gamePathSettingsFile = $"{settingsDirectory}\\GamePathSettings.xml";

                if (!Directory.Exists(settingsDirectory))
                {
                    Directory.CreateDirectory(settingsDirectory);
                }

                List<string> allGamesList = GameIndex.GetAllGamesList();

                XmlDocument gamePathSettingsXml = new();

                XmlNode docNode = gamePathSettingsXml.CreateXmlDeclaration("1.0", "UTF-8", null);
                _ = gamePathSettingsXml.AppendChild(docNode);

                XmlNode rootNode = gamePathSettingsXml.CreateElement("GamePathSettings");
                _ = gamePathSettingsXml.AppendChild(rootNode);

                foreach (string gameId in allGamesList)
                {
                    string gameFilePath = GameFile.GetGameFilePath(gameId);
                    XmlElement gamePathConfigNode = gamePathSettingsXml.CreateElement(gameId);
                    gamePathConfigNode.InnerText = gameFilePath ?? string.Empty; //nullチェック

                    _ = rootNode.AppendChild(gamePathConfigNode);
                }

                gamePathSettingsXml.Save(gamePathSettingsFile);
            }
        }

        public static void ConfigureGamePathSettings()
        {
            string? settingsDirectory = $"{User.CurrentUserDirectoryPath}\\Settings";
            string? gamePathSettingsFile = $"{settingsDirectory}\\GamePathSettings.xml";

            List<string> allGamesList = GameIndex.GetAllGamesList();

            if (!string.IsNullOrEmpty(gamePathSettingsFile) && File.Exists(gamePathSettingsFile))
            {
                XmlDocument gamePathSettingsXml = new();
                gamePathSettingsXml.Load(gamePathSettingsFile);

                XmlElement? rootNode = gamePathSettingsXml.DocumentElement;

                foreach (string gameId in allGamesList)
                {
                    XmlNode? gamePathConfigNode = rootNode?.SelectSingleNode(gameId);
                    if (gamePathConfigNode != null)
                    {
                        GameFile.SetGameFilePath(gameId, gamePathConfigNode.InnerText);
                    }
                    else
                    {
                        GameFile.SetGameFilePath(gameId, string.Empty);
                    }
                }
            }
            else
            {
                foreach (string gameId in allGamesList)
                {
                    GameFile.SetGameFilePath(gameId, string.Empty);
                }
            }
        }

        public static void SaveGameSpecificConfig()
        {
            if (!string.IsNullOrEmpty(User.CurrentUserDirectoryPath))
            {
                string? settingsDirectory = $"{User.CurrentUserDirectoryPath}\\Settings";
                string? gameSpecificConfigFile = $"{settingsDirectory}\\GameSpecificConfig.xml";

                if (!Directory.Exists(settingsDirectory))
                {
                    Directory.CreateDirectory(settingsDirectory);
                }

                List<string> allGamesList = GameIndex.GetAllGamesList();

                XmlDocument gameSpecificConfigXml = new();

                XmlNode docNode = gameSpecificConfigXml.CreateXmlDeclaration("1.0", "UTF-8", null);
                _ = gameSpecificConfigXml.AppendChild(docNode);

                XmlNode rootNode = gameSpecificConfigXml.CreateElement("GameSpecificConfig");
                _ = gameSpecificConfigXml.AppendChild(rootNode);

                XmlNode autoResizerConfigRootNode =
                    gameSpecificConfigXml.CreateElement("AutoResizerConfig");
                _ = rootNode.AppendChild(autoResizerConfigRootNode);

                XmlNode scoreFilterPlayerConfigRootNode =
                    gameSpecificConfigXml.CreateElement("ScoreFilterPlayerConfig");
                _ = rootNode.AppendChild(scoreFilterPlayerConfigRootNode);

                XmlNode scoreFilterEnemyConfigRootNode =
                    gameSpecificConfigXml.CreateElement("ScoreFilterEnemyConfig");
                _ = rootNode.AppendChild(scoreFilterEnemyConfigRootNode);

                XmlNode scoreFilterPracticeEnemyConfigRootNode =
                    gameSpecificConfigXml.CreateElement("ScoreFilterPracticeEnemyConfig");
                _ = rootNode.AppendChild(scoreFilterPracticeEnemyConfigRootNode);

                XmlNode scoreFilterLevelConfigRootNode =
                    gameSpecificConfigXml.CreateElement("ScoreFilterLevelConfig");
                _ = rootNode.AppendChild(scoreFilterLevelConfigRootNode);

                foreach (string gameId in allGamesList)
                {
                    bool? config = GameSpecificSettings.GetAutoResizerConfig(gameId);
                    XmlElement autoResizerConfigNode = gameSpecificConfigXml.CreateElement(gameId);
                    autoResizerConfigNode.InnerText = (config == true).ToString(); //nullチェック

                    _ = autoResizerConfigRootNode.AppendChild(autoResizerConfigNode);

                    string filterPlayer = GameSpecificSettings.GetScoreFilterPlayer(gameId);
                    XmlElement scoreFilterPlayerConfigNode = gameSpecificConfigXml.CreateElement(gameId);
                    scoreFilterPlayerConfigNode.InnerText = filterPlayer;

                    _ = scoreFilterPlayerConfigRootNode.AppendChild(scoreFilterPlayerConfigNode);

                    string filterEnemy = GameSpecificSettings.GetScoreFilterEnemy(gameId);
                    XmlElement scoreFilterEnemyConfigNode = gameSpecificConfigXml.CreateElement(gameId);
                    scoreFilterEnemyConfigNode.InnerText = filterEnemy;

                    _ = scoreFilterEnemyConfigRootNode.AppendChild(scoreFilterEnemyConfigNode);

                    string filterPracticeEnemy = GameSpecificSettings.GetScoreFilterPracticeEnemy(gameId);
                    XmlElement scoreFilterPracticeEnemyConfigNode = gameSpecificConfigXml.CreateElement(gameId);
                    scoreFilterPracticeEnemyConfigNode.InnerText = filterPracticeEnemy;

                    _ = scoreFilterPracticeEnemyConfigRootNode.AppendChild(scoreFilterPracticeEnemyConfigNode);

                    string filterLevel = GameSpecificSettings.GetScoreFilterLevel(gameId);
                    XmlElement scoreFilterLevelConfigNode = gameSpecificConfigXml.CreateElement(gameId);
                    scoreFilterLevelConfigNode.InnerText = filterLevel;

                    _ = scoreFilterLevelConfigRootNode.AppendChild(scoreFilterLevelConfigNode);
                }

                gameSpecificConfigXml.Save(gameSpecificConfigFile);
            }
        }

        public static void ConfigureGameSpecificConfig()
        {
            string? settingsDirectory = $"{User.CurrentUserDirectoryPath}\\Settings";
            string? gameSpecificConfigFile = $"{settingsDirectory}\\GameSpecificConfig.xml";

            List<string> allGamesList = GameIndex.GetAllGamesList();

            if (!string.IsNullOrEmpty(gameSpecificConfigFile) && File.Exists(gameSpecificConfigFile))
            {
                XmlDocument gameSpecificConfigXml = new();
                gameSpecificConfigXml.Load(gameSpecificConfigFile);

                XmlElement? rootNode = gameSpecificConfigXml.DocumentElement;

                XmlNode? autoResizerConfigRootNode
                    = rootNode?.SelectSingleNode("AutoResizerConfig");

                XmlNode? scoreFilterPlayerConfigRootNode =
                    rootNode?.SelectSingleNode("ScoreFilterPlayerConfig");

                XmlNode? scoreFilterEnemyConfigRootNode =
                    rootNode?.SelectSingleNode("ScoreFilterEnemyConfig");

                XmlNode? scoreFilterPracticeEnemyConfigRootNode =
                    rootNode?.SelectSingleNode("ScoreFilterPracticeEnemyConfig");

                XmlNode? scoreFilterLevelConfigRootNode =
                    rootNode?.SelectSingleNode("ScoreFilterLevelConfig");

                foreach (string gameId in allGamesList)
                {
                    XmlNode? autoResizerConfigNode = autoResizerConfigRootNode?.SelectSingleNode(gameId);
                    XmlNode? scoreFilterPlayerConfigNode = scoreFilterPlayerConfigRootNode?.SelectSingleNode(gameId);
                    XmlNode? scoreFilterEnemyConfigNode = scoreFilterEnemyConfigRootNode?.SelectSingleNode(gameId);
                    XmlNode? scoreFilterPracticeEnemyConfigNode
                                = scoreFilterPracticeEnemyConfigRootNode?.SelectSingleNode(gameId);
                    XmlNode? scoreFilterLevelConfigNode = scoreFilterLevelConfigRootNode?.SelectSingleNode(gameId);

                    if (autoResizerConfigNode != null)
                    {
                        GameSpecificSettings.SetAutoResizerConfig(gameId, autoResizerConfigNode.InnerText.ToLower() == "true");
                    }
                    else
                    {
                        GameSpecificSettings.SetAutoResizerConfig(gameId, false);
                    }

                    if (scoreFilterPlayerConfigNode != null)
                    {
                        GameSpecificSettings.SetScoreFilterPlayer(gameId, scoreFilterPlayerConfigNode.InnerText);
                    }
                    else
                    {
                        GameSpecificSettings.SetScoreFilterPlayer(gameId, "ALL");
                    }

                    if (scoreFilterEnemyConfigNode != null)
                    {
                        GameSpecificSettings.SetScoreFilterEnemy(gameId, scoreFilterEnemyConfigNode.InnerText);
                    }
                    else
                    {
                        GameSpecificSettings.SetScoreFilterEnemy(gameId, "ALL");
                    }

                    if (scoreFilterPracticeEnemyConfigNode != null)
                    {
                        GameSpecificSettings.SetScoreFilterPracticeEnemy(
                            gameId, scoreFilterPracticeEnemyConfigNode.InnerText);
                    }
                    else
                    {
                        GameSpecificSettings.SetScoreFilterPracticeEnemy(gameId, "ALL");
                    }

                    if (scoreFilterLevelConfigNode != null)
                    {
                        GameSpecificSettings.SetScoreFilterLevel(gameId, scoreFilterLevelConfigNode.InnerText);
                    }
                    else
                    {
                        GameSpecificSettings.SetScoreFilterLevel(gameId, "ALL");
                    }
                }
            }
            else
            {
                foreach (string gameId in allGamesList)
                {
                    GameSpecificSettings.SetAutoResizerConfig(gameId, false);
                    GameSpecificSettings.SetScoreFilterPlayer(gameId, "ALL");
                    GameSpecificSettings.SetScoreFilterEnemy(gameId, "ALL");
                    GameSpecificSettings.SetScoreFilterPracticeEnemy(gameId, "ALL");
                    GameSpecificSettings.SetScoreFilterLevel(gameId, "ALL");
                }
            }
        }

        public static void SaveMainWindowSettings(MainWindowSettings mainWindowSettings)
        {            
            if (!string.IsNullOrEmpty(User.CurrentUserDirectoryPath))
            {
                string? settingsDirectory = $"{User.CurrentUserDirectoryPath}\\Settings";
                string? mainWindowSettingsFile = $"{settingsDirectory}\\MainWindowSettings.xml";

                if (!Directory.Exists(settingsDirectory))
                {
                    Directory.CreateDirectory(settingsDirectory);
                }

                XmlSerializer mainWindowSettingsSerializer = new(typeof(MainWindowSettings));
                FileStream fileStream = new(mainWindowSettingsFile, FileMode.Create);
                mainWindowSettingsSerializer.Serialize(fileStream, mainWindowSettings);
                fileStream.Close();
            }
        }

        public static MainWindowSettings ConfigureMainWindowSettings()
        {
            string? settingsDirectory = $"{User.CurrentUserDirectoryPath}\\Settings";
            string? mainWindowSettingsFile = $"{settingsDirectory}\\MainWindowSettings.xml";

            MainWindowSettings? mainWindowSettings = new();

            if (!string.IsNullOrEmpty(mainWindowSettingsFile) &&
                File.Exists(mainWindowSettingsFile))
            {
                XmlSerializer mainWindowSettingsSerializer = new(typeof(MainWindowSettings));
                FileStream fileStream = new(mainWindowSettingsFile, FileMode.Open);

                mainWindowSettings = (MainWindowSettings)mainWindowSettingsSerializer.Deserialize(fileStream);
                fileStream.Close();
            }
            else
            {
                mainWindowSettings.MainWindowWidth = 680;
                mainWindowSettings.MainWindowHeight = 445;
                mainWindowSettings.SelectedGameId = string.Empty;
                mainWindowSettings.DisplayUnchallengedCard = false;
                mainWindowSettings.ExcludeUnchallengedCardData = false;
                mainWindowSettings.AutoBackup = false;
            }

            if (mainWindowSettings == null)
            {
                mainWindowSettings.MainWindowWidth = 680;
                mainWindowSettings.MainWindowHeight = 445;
                mainWindowSettings.SelectedGameId = string.Empty;
                mainWindowSettings.DisplayUnchallengedCard = false;
                mainWindowSettings.ExcludeUnchallengedCardData = false;
                mainWindowSettings.AutoBackup = false;
            }

            return mainWindowSettings;
        }

        public static void SaveResizerFrameWindowSettings(ResizerFrameWindowSettings resizerFrameWindowSettings)
        {
            string? settingsDirectory = $"{User.CurrentUserDirectoryPath}\\Settings";
            string? resizerFrameWindowSettingsFile = $"{settingsDirectory}\\ResizerFrameWindowSettings.xml";

            XmlSerializer resizerFrameWindowSettingsSerializer = new(typeof(ResizerFrameWindowSettings));
            FileStream fileStream = new(resizerFrameWindowSettingsFile, FileMode.Create);
            resizerFrameWindowSettingsSerializer.Serialize(fileStream, resizerFrameWindowSettings);
            fileStream.Close();
        }

        public static ResizerFrameWindowSettings ConfigureResizerFrameWindowSettings()
        {
            string? settingsDirectory = $"{User.CurrentUserDirectoryPath}\\Settings";
            string? resizerFrameWindowSettingsFile = $"{settingsDirectory}\\ResizerFrameWindowSettings.xml";

            ResizerFrameWindowSettings resizerFrameWindowSettings = new();

            if (!string.IsNullOrEmpty(resizerFrameWindowSettingsFile) &&
                File.Exists(resizerFrameWindowSettingsFile))
            {
                XmlSerializer resizerFrameWindowSettingsSerializer = new(typeof(ResizerFrameWindowSettings));
                FileStream fileStream = new(resizerFrameWindowSettingsFile, FileMode.Open);

                resizerFrameWindowSettings
                    = (ResizerFrameWindowSettings)resizerFrameWindowSettingsSerializer.Deserialize(fileStream);
                fileStream.Close();
            }
            else
            {
                resizerFrameWindowSettings.FixAspectRate = true;
                resizerFrameWindowSettings.AutoClose = false;
            }

            if (resizerFrameWindowSettings == null)
            {
                resizerFrameWindowSettings.FixAspectRate = true;
                resizerFrameWindowSettings.AutoClose = false;
            }

            return resizerFrameWindowSettings;
        }
    }
}
