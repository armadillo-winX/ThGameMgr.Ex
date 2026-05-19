using System.Collections.ObjectModel;

namespace ThGameMgr.Ex.Replay
{
    internal class ReplayFile
    {
        public static string GetReplayDirectory(string gameId)
        {
            string scoreFilePath = ScoreFile.GetScoreFilePath(gameId);
            return $"{Path.GetDirectoryName(scoreFilePath)}\\replay";
        }

        public static ObservableCollection<ReplayFileInfo> GetReplayFiles(string? gameId)
        {
            ObservableCollection<ReplayFileInfo> replayFileInfos = [];

            string? replayDirectory = GetReplayDirectory(gameId);
            if (!string.IsNullOrWhiteSpace(replayDirectory) &&
                Directory.Exists(replayDirectory))
            {
                string[] replayFilesList = Directory.GetFiles(replayDirectory, "*.rpy", SearchOption.TopDirectoryOnly);
                foreach (string replayFile in replayFilesList)
                {
                    ReplayFileInfo replayFileInfo;
                    try
                    {
                        replayFileInfo = GetReplayFileInfo(replayFile);
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex.Message);

                        replayFileInfo = new()
                        {
                            FileName = Path.GetFileName(replayFile),
                            UpdateDate = "Error",
                            FileSize = "Error"
                        };
                    }

                    replayFileInfos.Add(replayFileInfo);
                }

                return replayFileInfos;
            }
            else
            {
                return replayFileInfos;
            }
        }

        private static ReplayFileInfo GetReplayFileInfo(string replayFilePath)
        {
            DateTime updateTime = File.GetLastWriteTime(replayFilePath);
            string fileSize = GetReplayFileSize(replayFilePath);

            return new ReplayFileInfo
            {
                FileName = Path.GetFileName(replayFilePath),
                UpdateDate = updateTime.ToString("yyyy/MM/dd HH:mm:ss"),
                FileSize = fileSize
            };
        }

        private static string GetReplayFileSize(string replayFilePath)
        {
            FileInfo fileInfo = new(replayFilePath);
            long fileSize = fileInfo.Length;
            return $"{fileSize / 1024} KiB";
        }

        public static void Rename(string gameId, string replayFileName, string newReplayFileName)
        {
            string replayDirectory = GetReplayDirectory(gameId);

            if (replayFileName != newReplayFileName)
            {
                File.Move($"{replayDirectory}\\{replayFileName}", $"{replayDirectory}\\{newReplayFileName}");
            }
        }

        public static bool Exists(string gameId, string replayFileName)
        {
            string replayDirectory = GetReplayDirectory(gameId);

            return File.Exists($"{replayDirectory}\\{replayFileName}");
        }

        public static void Delete(string gameId, string replayFileName)
        {
            string replayDirectory = GetReplayDirectory(gameId);
            File.Delete($"{replayDirectory}\\{replayFileName}");
        }

    }
}
