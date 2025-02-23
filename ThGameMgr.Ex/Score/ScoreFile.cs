using System.Xml;

namespace ThGameMgr.Ex.Score
{
    internal class ScoreFile
    {
        public static string? GetScoreFilePath(string gameId)
        {
            string shanghaiAliceAppData = $"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}\\ShanghaiAlice";
            string gamePath = GameFile.GetGameFilePath(gameId);

            if (gamePath == null)
            {
                return null;
            }
            else
            {
                string gameDirectory = Path.GetDirectoryName(gamePath);

                if (
                    gameId == GameIndex.Th06 ||
                    gameId == GameIndex.Th07 ||
                    gameId == GameIndex.Th08 ||
                    gameId == GameIndex.Th09)
                {
                    return $"{gameDirectory}\\score.dat";
                }
                else if (
                    gameId == GameIndex.Th10 ||
                    gameId == GameIndex.Th11 ||
                    gameId == GameIndex.Th12)
                {
                    return $"{gameDirectory}\\score{gameId.ToLower()}.dat";
                }
                else
                {
                    return $"{shanghaiAliceAppData}\\{gameId.ToLower()}\\score{gameId.ToLower()}.dat";
                }
            }
        }

        public static bool RecallScoreDataFile(string gameId)
        {
            string recallDirectory = $"{PathInfo.AppLocation}\\Recall\\";
            string recallSourceFile = $"{recallDirectory}\\recall.dat";
            string recallSourceInfoFile = $"{recallDirectory}\\recall_information.xml";

            XmlDocument recallSourceInfoXml = new();
            recallSourceInfoXml.Load(recallSourceInfoFile);

            XmlElement? rootNode = recallSourceInfoXml.DocumentElement;

            XmlNode? gameIdNode
                = rootNode?.SelectSingleNode("GameId");

            if (gameIdNode?.InnerText == gameId)
            {
                XmlNode? scoreFilePathNode = rootNode?.SelectSingleNode("ScoreDataFilePath");

                string? scoreDataFilePath = GetScoreFilePath(gameId);
                if (scoreFilePathNode?.InnerText == scoreDataFilePath)
                {
                    File.Move(recallSourceFile, scoreDataFilePath, true);
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// スコアデータの回帰元ファイルを作成します
        /// </summary>
        /// <param name="gameId"></param>
        public static void CreateRecallSourceFile(string gameId)
        {
            string scoreDataFile = GetScoreFilePath(gameId);

            string recallDirectory = $"{PathInfo.AppLocation}\\Recall\\";
            if (!Directory.Exists(recallDirectory))
                Directory.CreateDirectory(recallDirectory);

            string recallSourceFile = $"{recallDirectory}\\recall.dat";
            File.Copy(scoreDataFile, recallSourceFile, true);

            string recallSourceInfoFile = $"{recallDirectory}\\recall_information.xml";

            CreateRecallSourceInfoFile(gameId, scoreDataFile, recallSourceInfoFile);
        }

        /// <summary>
        /// スコアデータ回帰元ファイルについての情報を記述します。
        /// 記述される情報はゲーム種別(Game ID)とスコアデータファイルの元のパスです。
        /// </summary>
        /// <param name="gameId"></param>
        /// <param name="scoreDataFile"></param>
        /// <param name="recallSourceInfoFile"></param>
        private static void CreateRecallSourceInfoFile(string gameId, string scoreDataFile, string recallSourceInfoFile)
        {
            XmlDocument recallSourceInfoXml = new();

            XmlNode docNode = recallSourceInfoXml.CreateXmlDeclaration("1.0", "UTF-8", null);
            _ = recallSourceInfoXml.AppendChild(docNode);

            XmlNode rootNode = recallSourceInfoXml.CreateElement("RecallSourceInformation");
            _ = recallSourceInfoXml.AppendChild(rootNode);

            XmlNode gameIdNode = recallSourceInfoXml.CreateElement("GameId");
            gameIdNode.InnerText = gameId;
            _ = rootNode.AppendChild(gameIdNode);

            XmlNode scoreFilePathNode = recallSourceInfoXml.CreateElement("ScoreDataFilePath");
            scoreFilePathNode.InnerText = scoreDataFile;
            _ = rootNode.AppendChild(scoreFilePathNode);

            recallSourceInfoXml.Save(recallSourceInfoFile);
        }
    }
}
