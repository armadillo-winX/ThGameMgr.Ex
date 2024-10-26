using System.Collections.Generic;

namespace ThGameMgr.Ex.Game
{
    class GameSpecificConfig
    {
        private static Dictionary<string, bool?>? _autoResizerConfigDictionary;

        private static Dictionary<string, string>? _scoreFilterPlayerDictionary;

        private static Dictionary<string, string>? _scoreFilterEnemyDictionary;

        private static Dictionary<string, string>? _scoreFilterLevelDictionary;

        /// <summary>
        /// ウィンドウリサイザの自動起動オプションを返す(bool?値)
        /// </summary>
        /// <param name="gameId"></param>
        /// <returns></returns>
        public static bool? GetAutoResizerConfig(string gameId)
        {
            if (_autoResizerConfigDictionary == null)
            {
                return false;
            }
            else
            {
                if (_autoResizerConfigDictionary.TryGetValue(gameId, out bool? config))
                {
                    return config;
                }
                else
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// ウィンドウリサイザの自動起動オプションを設定する
        /// </summary>
        /// <param name="gameId"></param>
        /// <param name="config"></param>
        public static void SetAutoResizerConfig(string gameId, bool? config)
        {
            if (_autoResizerConfigDictionary == null)
            {
                _autoResizerConfigDictionary = [];
                _autoResizerConfigDictionary.Add(gameId, config);
            }
            else
            {
                if (_autoResizerConfigDictionary.ContainsKey(gameId))
                {
                    _autoResizerConfigDictionary[gameId] = config;
                }
                else
                {
                    _autoResizerConfigDictionary.Add(gameId, config);
                }
            }
        }

        public static string GetScoreFilterPlayer(string gameId)
        {
            if (_scoreFilterPlayerDictionary == null)
            {
                return "ALL";
            }
            else
            {
                if (_scoreFilterPlayerDictionary.TryGetValue(gameId, out string? filterPlayer))
                {
                    return filterPlayer;
                }
                else
                {
                    return "ALL";
                }
            }
        }

        public static void SetScoreFilterPlayer(string gameId, string filterPlayer)
        {
            if (_scoreFilterPlayerDictionary == null)
            {
                _scoreFilterPlayerDictionary = [];
                _scoreFilterPlayerDictionary.Add(gameId, filterPlayer);
            }
            else
            {
                if (_scoreFilterPlayerDictionary.ContainsKey(gameId))
                {
                    _scoreFilterPlayerDictionary[gameId] = filterPlayer;
                }
                else
                {
                    _scoreFilterPlayerDictionary.Add(gameId, filterPlayer);
                }
            }
        }

        public static string GetScoreFilterEnemy(string gameId)
        {
            if (_scoreFilterEnemyDictionary == null)
            {
                return "ALL";
            }
            else
            {
                if (_scoreFilterEnemyDictionary.TryGetValue(gameId, out string? filterEnemy))
                {
                    return filterEnemy;
                }
                else
                {
                    return "ALL";
                }
            }
        }

        public static void SetScoreFilterEnemy(string gameId, string filterEnemy)
        {
            if (_scoreFilterEnemyDictionary == null)
            {
                _scoreFilterEnemyDictionary = [];
                _scoreFilterEnemyDictionary.Add(gameId, filterEnemy);
            }
            else
            {
                if (_scoreFilterEnemyDictionary.ContainsKey(gameId))
                {
                    _scoreFilterEnemyDictionary[gameId] = filterEnemy;
                }
                else
                {
                    _scoreFilterEnemyDictionary.Add(gameId, filterEnemy);
                }
            }
        }

        public static string GetScoreFilterLevel(string gameId)
        {
            if (_scoreFilterLevelDictionary == null)
            {
                return "ALL";
            }
            else
            {
                if (_scoreFilterLevelDictionary.TryGetValue(gameId, out string? filterLevel))
                {
                    return filterLevel;
                }
                else
                {
                    return "ALL";
                }
            }
        }

        public static void SetScoreFilterLevel(string gameId, string filterLevel)
        {
            if (_scoreFilterLevelDictionary == null)
            {
                _scoreFilterLevelDictionary = [];
                _scoreFilterLevelDictionary.Add(gameId, filterLevel);
            }
            else
            {
                if (_scoreFilterLevelDictionary.ContainsKey(gameId))
                {
                    _scoreFilterLevelDictionary[gameId] = filterLevel;
                }
                else
                {
                    _scoreFilterLevelDictionary.Add(gameId, filterLevel);
                }
            }
        }
    }
}
