using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace ThGameMgr.Ex.Replay
{
    internal class ReplayFiles
    {
        private readonly static Dictionary<string, string> _gameIdDictionary
            = new()
            {
                { "th6", "Th06" },
                { "th7", "Th07" },
                { "th8", "Th08" },
                { "th9", "Th09" },
                { "th10", "Th10" },
                { "th11", "Th11" },
                { "th12", "Th12" },
                { "th13", "Th13" },
                { "th14", "Th14" },
                { "th15", "Th15" },
                { "th16", "Th16" },
                { "th17", "Th17" },
                { "th18", "Th18" }
            };

        public static string GetReplayDirectory(string gameId)
        {
            string scoreFilePath = ScoreFile.GetScoreFilePath(gameId);
            return $"{Path.GetDirectoryName(scoreFilePath)}\\replay";
        }

        public static ObservableCollection<ReplayFileInfo> GetReplayFiles(string? gameId)
        {
            ObservableCollection<ReplayFileInfo> replayFileInfos = [];

            string? replayDirectory = GetReplayDirectory(gameId);
            if (!string.IsNullOrWhiteSpace(replayDirectory))
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

        public static string? GetGameId(string replayFilePath)
        {
            string replayName = Path.GetFileNameWithoutExtension(replayFilePath);
            return _gameIdDictionary[replayName.Split('_')[0]];
        }

        public static string Import(string replayFilePath)
        {
            string gameId = GetGameId(replayFilePath);
            string? replayDirectory = GetReplayDirectory(gameId);
            string replayName = Path.GetFileNameWithoutExtension(replayFilePath);
            if (Directory.Exists(replayDirectory))
            {
                try
                {
                    string newReplayFile = $"{replayDirectory}\\{replayName}.rpy";
                    int i = 0;
                    while (File.Exists(newReplayFile))
                    {
                        i++;
                        newReplayFile = $"{replayDirectory}\\{replayName}-{i}.rpy";
                    }

                    File.Move(replayFilePath, newReplayFile);
                    return $"成功:{newReplayFile}";
                }
                catch (Exception ex)
                {
                    return $"エラー:{ex.Message}";
                }
            }
            else
            {
                return $"取り込み先ディレクトリが存在しませんでした。Game:{gameId}";
            }
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
