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

        /// <summary>
        /// スコアデータの回帰元ファイルを作成します
        /// </summary>
        /// <param name="gameId"></param>
        public static void CreateRecallSourceFile(string gameId)
        {
            string scoreDataFile = GetScoreFilePath(gameId);

            string tempDirectory = $"{PathInfo.AppLocation}\\temp\\";
            if (!Directory.Exists(tempDirectory))
                Directory.CreateDirectory(tempDirectory);

            string recallSourceFile = $"{tempDirectory}\\recall.dat";
            File.Copy(scoreDataFile, recallSourceFile, true);

            string recallSourceInfoFile = $"{tempDirectory}\\recall_information.xml";
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
