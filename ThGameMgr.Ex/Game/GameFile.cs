using System.Drawing;
using System.Collections.Generic;

namespace ThGameMgr.Ex.Game
{
    internal class GameFile
    {
        private static Dictionary<string, string>? _gameFilesDictionary;

        public static string GetGameFilePath(string gameId)
        {
            if (_gameFilesDictionary == null)
            {
                return string.Empty;
            }
            else
            {
                if (_gameFilesDictionary.TryGetValue(gameId, out string? gameFilePath))
                {
                    return gameFilePath;
                }
                else
                {
                    return string.Empty;
                }
            }
        }

        public static void SetGameFilePath(string gameId, string gameFilePath)
        {
            if (_gameFilesDictionary == null)
            {
                _gameFilesDictionary = [];
                _gameFilesDictionary.Add(gameId, gameFilePath);
            }
            else
            {
                if (_gameFilesDictionary.ContainsKey(gameId))
                {
                    _gameFilesDictionary[gameId] = gameFilePath;
                }
                else
                {
                    _gameFilesDictionary.Add(gameId, gameFilePath);
                }
            }
        }

        /// <summary>
        /// ゲームインストールフォルダの全 thprac 実行ファイルのファイル名を返す(List型)
        /// </summary>
        /// <param name="gameId"></param>
        /// <returns></returns>
        /// <exception cref="DirectoryNotFoundException"></exception>
        public static List<string> GetThpracFiles(string gameId)
        {
            string gameFile = GetGameFilePath(gameId);
            string gameDirectory = Path.GetDirectoryName(gameFile);
            if (!string.IsNullOrEmpty(gameDirectory))
            {
                List<string> thpracFiles = [];

                foreach (string thpracFile in
                    Directory.GetFiles(gameDirectory, "thprac*.exe", SearchOption.TopDirectoryOnly))
                {
                    string thpracFileName = Path.GetFileName(thpracFile);
                    thpracFiles.Add(thpracFileName);
                }

                return thpracFiles;
            }
            else
            {
                throw new DirectoryNotFoundException("ゲームがインストールされているフォルダが見つかりませんでした。");
            }
        }

        public static Icon? GetGameIcon(string gameId)
        {
            try
            {
                string gameFilePath = GetGameFilePath(gameId);
                Icon? gameIcon = Icon.ExtractAssociatedIcon(gameFilePath);
                return gameIcon;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static void OpenGameDirectory(string gameId)
        {
            string? gamePath = GameFile.GetGameFilePath(gameId);
            string gameDirectory = Path.GetDirectoryName(gamePath);
            if (Directory.Exists(gameDirectory))
            {
                Process.Start("explorer.exe", gameDirectory);
            }
        }

        public static void OpenScoreDirectory(string gameId)
        {
            string? scoreFilePath = ScoreFile.GetScoreFilePath(gameId);
            string scoreFileDirectory = Path.GetDirectoryName(scoreFilePath);
            if (Directory.Exists(scoreFileDirectory))
            {
                Process.Start("explorer.exe", scoreFileDirectory);
            }
        }

        public static void OpenGameLog(string gameId)
        {
            string? scoreFilePath = ScoreFile.GetScoreFilePath(gameId);
            string scoreFileDirectory = Path.GetDirectoryName(scoreFilePath);
            string logFile = $"{scoreFileDirectory}\\log.txt";
            if (File.Exists(logFile))
            {
                Process.Start("notepad.exe", logFile);
            }
        }

        public static string GetGameLogFile(string gameId)
        {
            string? scoreFilePath = ScoreFile.GetScoreFilePath(gameId);
            if (!string.IsNullOrEmpty(scoreFilePath))
            {
                string scoreFileDirectory = Path.GetDirectoryName(scoreFilePath);
                string logFile = $"{scoreFileDirectory}\\log.txt";

                return logFile;
            }
            else
            {
                return string.Empty;
            }
        }
    }
}
