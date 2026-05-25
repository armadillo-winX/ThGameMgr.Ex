namespace ThGameMgr.Ex.Score
{
    internal class ScoreBackup
    {
        public static bool Create(string gameId, string backupDirectory)
        {
            string scoreFilePath = ScoreFile.GetScoreFilePath(gameId);

            if (File.Exists(scoreFilePath))
            {
                if (!Directory.Exists(backupDirectory))
                    Directory.CreateDirectory(backupDirectory);

                string timestamp = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");

                File.Copy(scoreFilePath, $"{backupDirectory}\\{timestamp}.bak", true);

                return true;
            }
            else
            {
                return false;
            }
        }

        public static void Restore(string gameId, string backupFilePath)
        {
            string scoreFilePath = ScoreFile.GetScoreFilePath(gameId);

            File.Copy(backupFilePath, scoreFilePath, true);
        }

        public static string[] GetScoreBackupFiles(string gameId, string backupDirectory)
        {
            string[] scoreBackupFiles = Directory.GetFiles(Path.Combine(backupDirectory, gameId), "*.bak", SearchOption.TopDirectoryOnly);

            return scoreBackupFiles;
        }

        public static void Delete(string gameId, string backupFileName, string backupDirectory)
        {
            string backupFilePath = Path.Combine(backupDirectory, backupFileName);
            File.Delete(backupFilePath);
        }
    }
}
