using System.Collections.Generic;

namespace ThGameMgr.Ex.Score
{
    internal class SpellCard
    {
        public string? CardID { get; set; }

        public string? CardName { get; set; }

        public string? Enemy { get; set; }

        public string? Place { get; set; }

        /// <summary>
        /// Th10以降のスコアデータで、バイナリ形式のレベルデータからレベルを string 値で返します。
        /// </summary>
        /// <param name="levelData"></param>
        /// <returns></returns>
        public static string ParseLevelData(byte[] levelData)
        {
            try
            {
                Dictionary<int, string> levelDictionary = new()
                {
                    { 0, "Easy"     },
                    { 1, "Normal"   },
                    { 2, "Hard"     },
                    { 3, "Lunatic"  },
                    { 4, "Extra"    },
                    { 5, "Phantasm" }
                };

                int index = Convert.ToInt32(levelData);
                if (levelDictionary.TryGetValue(index, out string level))
                {
                    return level;
                }
                else
                {
                    return string.Empty;
                }
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }
    }
}
