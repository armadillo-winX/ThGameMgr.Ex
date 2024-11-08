namespace ThGameMgr.Ex.Score
{
    internal class ScoreBackup
    {
        public static bool Create(string gameId)
        {
            string scoreFilePath = ScoreFile.GetScoreFilePath(gameId);
            string backupDirectory = $"{User.CurrentUserDirectoryPath}\\backup\\{gameId}";

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

        public static void Restore(string gameId, string backupFileName)
        {
            string scoreFilePath = ScoreFile.GetScoreFilePath(gameId);
            string backupFilePath =
                $"{User.CurrentUserDirectoryPath}\\backup\\{gameId}\\{backupFileName}";

            File.Copy(backupFilePath, scoreFilePath, true);
        }

        public static string[] GetScoreBackupFiles(string gameId)
        {
            string backupDirectory = $"{User.CurrentUserDirectoryPath}\\backup\\{gameId}";
            string[] scoreBackupFiles = Directory.GetFiles(backupDirectory, "*.bak", SearchOption.TopDirectoryOnly);

            return scoreBackupFiles;
        }

        public static void Delete(string gameId, string backupFileName)
        {
            string backupFilePath = $"{User.CurrentUserDirectoryPath}\\backup\\{gameId}\\{backupFileName}";
            File.Delete(backupFilePath);
        }
    }
}
