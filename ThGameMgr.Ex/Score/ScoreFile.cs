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
    }
}
