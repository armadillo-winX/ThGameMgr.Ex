using System.Collections.Generic;
using System.Linq;

namespace ThGameMgr.Ex.Game
{
    internal class GameIndex
    {
        public static string Th06 => "Th06";

        public static string Th07 => "Th07";

        public static string Th08 => "Th08";

        public static string Th09 => "Th09";

        public static string Th10 => "Th10";

        public static string Th11 => "Th11";

        public static string Th12 => "Th12";

        public static string Th13 => "Th13";

        public static string Th14 => "Th14";

        public static string Th15 => "Th15";

        public static string Th16 => "Th16";

        public static string Th17 => "Th17";

        public static string Th18 => "Th18";

        private readonly static Dictionary<string, string> _gameNameDictionary = new()
        {
            { "Th06", "東方紅魔郷" },
            { "Th07", "東方妖々夢" },
            { "Th08", "東方永夜抄" },
            { "Th09", "東方花映塚" },
            { "Th10", "東方風神録" },
            { "Th11", "東方地霊殿" },
            { "Th12", "東方星蓮船" },
            { "Th13", "東方神霊廟" },
            { "Th14", "東方輝針城" },
            { "Th15", "東方紺珠伝" },
            { "Th16", "東方天空璋" },
            { "Th17", "東方鬼形獣" },
            { "Th18", "東方虹龍洞" }
        };

        public static string GetGameName(string? gameId)
        {
            if (!string.IsNullOrEmpty(gameId))
            {
                if (_gameNameDictionary.TryGetValue(gameId, out var gameName))
                {
                    return gameName;
                }
                else
                {
                    return string.Empty;
                }
            }
            else
            {
                return string.Empty;
            }
        }

        public static string? GetGameIdFromGameName(string? gameName)
        {
            if (!string.IsNullOrEmpty(gameName))
            {
                if (!string.IsNullOrEmpty(gameName))
                {
                    return _gameNameDictionary.FirstOrDefault(x => x.Value.Equals(gameName)).Key;
                }
                else
                {
                    return string.Empty;
                }
            }
            else
            {
                return string.Empty;
            }
        }

        public static List<string> GetAllGamesList()
        {
            List<string> allGamesList = [];

            foreach (KeyValuePair<string, string> gameNameDictionaryKeyValuePair in _gameNameDictionary)
            {
                string gameId = gameNameDictionaryKeyValuePair.Key;

                allGamesList.Add(gameId);
            }

            return allGamesList;
        }

        public static List<string> GetEnabledGamesList()
        {
            List<string> enabledGamesList = [];

            //_gameNameDictionary から Key と Value のペアをひとつずつ取得
            foreach (KeyValuePair<string, string> gameNameDictionaryKeyValuePair in _gameNameDictionary)
            {
                string gameId = gameNameDictionaryKeyValuePair.Key;

                if (!string.IsNullOrEmpty(GameFile.GetGameFilePath(gameId)))
                {
                    enabledGamesList.Add(gameId);
                }
            }

            return enabledGamesList;
        }

        public static string GetFirstEnableGame()
        {
            List<string> enabledGamesList = GetEnabledGamesList();
            if (enabledGamesList.Count > 0)
            {
                return enabledGamesList[0];
            }
            else
            {
                return string.Empty;
            }
        }
    }
}
