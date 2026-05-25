using System.Collections.ObjectModel;
using System.Xml;

namespace ThGameMgr.Ex.Game
{
    internal class GamePlayLogRecorder
    {
        private IUserService _currentUserService;

        public GamePlayLogRecorder(IUserService currentUserService)
        {
            _currentUserService = currentUserService;
        }

        public void SaveGamePlayLog(GamePlayLogData gamePlayLogData)
        {
            string gamePlayLogFile = _currentUserService.GetCurrentUserGamePlayLogRecordFilePath();

            if (!File.Exists(gamePlayLogFile))
            {
                CreateGamePlayLogFile();
            }

            XmlDocument gamePlayLogXml = new();
            gamePlayLogXml.Load(gamePlayLogFile);
            XmlElement? rootNode = gamePlayLogXml.DocumentElement;

            XmlElement gamePlayLog = gamePlayLogXml.CreateElement("GamePlayLog");

            //ノードをRootノードに追加
            _ = rootNode?.AppendChild(gamePlayLog);

            XmlElement gameId = gamePlayLogXml.CreateElement("GameId");
            _ = gameId.AppendChild(gamePlayLogXml.CreateTextNode(gamePlayLogData.GameId));
            _ = gamePlayLog.AppendChild(gameId);

            XmlElement gameName = gamePlayLogXml.CreateElement("GameName");
            _ = gameName.AppendChild(gamePlayLogXml.CreateTextNode(gamePlayLogData.GameName));
            _ = gamePlayLog.AppendChild(gameName);

            XmlElement gameStartTime = gamePlayLogXml.CreateElement("GameStartTime");
            _ = gameStartTime.AppendChild(gamePlayLogXml.CreateTextNode(gamePlayLogData.GameStartTime));
            _ = gamePlayLog.AppendChild(gameStartTime);

            XmlElement gameEndTime = gamePlayLogXml.CreateElement("GameEndTime");
            _ = gameEndTime.AppendChild(gamePlayLogXml.CreateTextNode(gamePlayLogData.GameEndTime));
            _ = gamePlayLog.AppendChild(gameEndTime);

            XmlElement gameRunningTime = gamePlayLogXml.CreateElement("GameRunningTime");
            _ = gameRunningTime.AppendChild(gamePlayLogXml.CreateTextNode(gamePlayLogData.GameRunningTime));
            _ = gamePlayLog.AppendChild(gameRunningTime);

            gamePlayLogXml.Save(gamePlayLogFile);
        }

        public ObservableCollection<GamePlayLogData> GetGamePlayLogDataCollection()
        {
            string gamePlayLogFile = _currentUserService.GetCurrentUserGamePlayLogRecordFilePath();

            ObservableCollection<GamePlayLogData> gamePlayLogDataCollection = [];

            XmlDocument gameLogDataXml = new();
            gameLogDataXml.Load(gamePlayLogFile);
            XmlNodeList? allGameLogs = gameLogDataXml.SelectNodes("GamePlayLogData/GamePlayLog");
            if (allGameLogs != null &&
                allGameLogs.Count != 0)
            {
                foreach (XmlNode gameLog in allGameLogs)
                {
                    XmlNode? gameIdNode = gameLog.SelectSingleNode("GameId");
                    XmlNode? gameNameNode = gameLog.SelectSingleNode("GameName");
                    XmlNode? gameStartTimeNode = gameLog.SelectSingleNode("GameStartTime");
                    XmlNode? gameEndTimeNode = gameLog.SelectSingleNode("GameEndTime");
                    XmlNode? gameRunningTimeNode = gameLog.SelectSingleNode("GameRunningTime");

                    GamePlayLogData gamePlayLogData = new()
                    {
                        GameId = gameIdNode != null ? gameIdNode.InnerText : string.Empty,
                        GameName = gameNameNode != null ? gameNameNode.InnerText : string.Empty,
                        GameStartTime = gameStartTimeNode != null ? gameStartTimeNode.InnerText : string.Empty,
                        GameEndTime = gameEndTimeNode != null ? gameEndTimeNode.InnerText : string.Empty,
                        GameRunningTime = gameRunningTimeNode != null ? gameRunningTimeNode.InnerText : string.Empty
                    };

                    gamePlayLogDataCollection.Add(gamePlayLogData);
                }
            }

            return gamePlayLogDataCollection;
        }

        public void CreateGamePlayLogFile()
        {
            string gamePlayLogFile = _currentUserService.GetCurrentUserGamePlayLogRecordFilePath();

            XmlDocument gamePlayLogXml = new();
            XmlNode docNode = gamePlayLogXml.CreateXmlDeclaration("1.0", "UTF-8", null);
            _ = gamePlayLogXml.AppendChild(docNode);

            XmlNode rootNode = gamePlayLogXml.CreateElement("GamePlayLogData");
            _ = gamePlayLogXml.AppendChild(rootNode);

            gamePlayLogXml.Save(gamePlayLogFile);
        }
    }
}
