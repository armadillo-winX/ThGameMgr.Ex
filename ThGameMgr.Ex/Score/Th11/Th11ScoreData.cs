using System.Collections.Generic;
using System.Linq;

namespace ThGameMgr.Ex.Score.Th11
{
    internal class Th11ScoreData
    {
        private static readonly string[] _th11PlayersList = GamePlayers.GetGamePlayers(GameIndex.Th11).Split(',');

        private static readonly Dictionary<string, string> _progressDictionary =
            new()
            {
                { "01", "Stage1" },
                { "02", "Stage2" },
                { "03", "Stage3" },
                { "04", "Stage4" },
                { "05", "Stage5" },
                { "06", "Stage6" },
                { "07", "Extra" },
                { "08", "All Clear" }
            };

        public static void Get()
        {
            string? gamePath = GameFile.GetGameFilePath(GameIndex.Th11);
            string? scorePath = ScoreFile.GetScoreFilePath(GameIndex.Th11);

            if (File.Exists(gamePath) && File.Exists(scorePath))
            {
                MemoryStream decodedData = new();
                bool decodeResult = ScoreDecoder.Decode(GameIndex.Th11, scorePath, decodedData);
                if (decodeResult)
                {
                    decodedData.Seek(0, SeekOrigin.Begin);
                    using (decodedData)
                    {
                        byte[] bytes = new byte[decodedData.Length];

                        _ = decodedData.Read(bytes, 0, (int)decodedData.Length);

                        int i = 40;
                        int size = 28;

                        for (int k = 0; k < 6; k++)
                        {
                            int l = 1;
                            int max = i + (28 * 50);
                            while (i < max)
                            {
                                int n = i + size;
                                byte[] highScoreData = bytes[i..n];
                                ScoreRecordData scoreRecordList = GetHighScoreData(highScoreData);
                                scoreRecordList.Level = LevelReplace(l);
                                scoreRecordList.Player = _th11PlayersList[k];

                                if (scoreRecordList.Name != "--------")
                                {
                                    ScoreData.ScoreRecordLists.Add(scoreRecordList);
                                }

                                i += size;
                                l++;
                            }
                            i = i + 220 + (144 * 175) + 16;
                        }

                        for (int p = 1; p < 176; p++)
                        {
                            Dictionary<string, SpellCardRecordData>
                                spellCardRecordList =
                                GetAllSpellCardRecord(p, bytes);
                            ScoreData.SpellCardRecordLists.Add(spellCardRecordList["all"]);

                            foreach (string player in GamePlayers.GetGamePlayers(GameIndex.Th11).Split(','))
                            {
                                ScoreData.SpellCardRecordsByPlayer[player].Add(spellCardRecordList[player]);
                            }
                        }
                    }
                }
                else
                {
                    throw new InvalidDataException("スコアファイルが不正です。");
                }
            }
        }

        private static ScoreRecordData GetHighScoreData(byte[] data)
        {
            byte[] SCORE_DATA = data[0..4];
            byte[] PROGRESS_DATA = data[4..5];
            byte[] CONTINUE_DATA = data[5..6];
            byte[] NAME_DATA = data[6..16];
            byte[] DATE_DATA = data[16..20];
            byte[] SLOW_DATA = data[20..24];
            byte[] UNKNOWN_DATA = data[24..28];

            int continueCount = Convert.ToInt32(BitConverter.ToString(CONTINUE_DATA, 0), 16);

            string score = String.Format("{0:#,0}", BitConverter.ToInt32(SCORE_DATA, 0) * 10 + continueCount);
            string slow = BitConverter.ToSingle(SLOW_DATA, 0).ToString("F3") + "%";

            int unixTime = BitConverter.ToInt32(DATE_DATA, 0);
            string date = unixTime.TranslateUnixTime();

            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            string name = Encoding.GetEncoding("Shift_JIS").GetString(NAME_DATA).TrimEnd('\0');

            string progressIndex = BitConverter.ToString(PROGRESS_DATA, 0);
            string progress
                = name != "--------" ?
                _progressDictionary.ContainsKey(progressIndex) ? _progressDictionary[progressIndex] : "Unknown"
                : "No Record";

            ScoreRecordData scoreRecordList = new()
            {
                Score = score,
                Name = name,
                Progress = progress,
                SlowRate = slow,
                Date = date
            };
            return scoreRecordList;
        }

        private static SpellCardRecordData GetSpellCardRecordData(byte[] data)
        {
            byte[] CARD_NAME_DATA = data[0..128];
            byte[] GET_DATA = data[128..132];
            byte[] CHALLENGE_DATA = data[132..136];
            byte[] CARD_ID_DATA = data[136..140];
            byte[] LEVEL_DATA = data[140..144];

            int cardId = BitConverter.ToInt32(CARD_ID_DATA, 0) + 1;

            string get = BitConverter.ToInt32(GET_DATA, 0).ToString();
            string challenge = BitConverter.ToInt32(CHALLENGE_DATA, 0).ToString();

            string level = SpellCard.ParseLevelData(LEVEL_DATA);

            SpellCardRecordData spellCardRecordList = new()
            {
                CardID = cardId.ToString(),
                GetCount = get,
                TryCount = challenge,
                Level = level
            };
            return spellCardRecordList;
        }

        private static Dictionary<string, SpellCardRecordData> GetAllSpellCardRecord(int cardId, byte[] data)
        {
            int n = cardId - 1;

            //ReimuA  Spell Card Record start at:   1660 bytes
            //ReimuB  Spell Card Record start at:  28496 bytes
            //ReimuB  Spell Card Record start at:  55332 bytes
            //MarisaA Spell Card Record start at:  82168 bytes
            //MarisaB Spell Card Record start at: 109004 bytes
            //MarisaC Spell Card Record start at: 135840 bytes

            Dictionary<string, SpellCardRecordData> spellCardRecordsDictionary = [];
            string[] players = GamePlayers.GetGamePlayers(GameIndex.Th11).Split(',');
            SpellCard spellCard = Th11SpellCard.GetSpellCardData(cardId);

            for (int playerIndex = 0; playerIndex < 6; playerIndex++)
            {
                int i = 1660 + (26836 * playerIndex) + (n * 144);

                byte[] PLAYER_SPELL_CARD_DATA = data[i..(i + 144)];

                SpellCardRecordData playerSpellCardRecordData = GetSpellCardRecordData(PLAYER_SPELL_CARD_DATA);

                int playerTryCount = Convert.ToInt32(playerSpellCardRecordData.TryCount);
                int playerGetCount = Convert.ToInt32(playerSpellCardRecordData.GetCount);

                string player = players[playerIndex];

                playerSpellCardRecordData.CardName = spellCard.CardName;
                playerSpellCardRecordData.Rate = ScoreCalculator.CalcSpellCardGetRate(playerGetCount, playerTryCount);
                playerSpellCardRecordData.Enemy = spellCard.Enemy;
                playerSpellCardRecordData.Place = spellCard.Place;

                spellCardRecordsDictionary.Add(player, playerSpellCardRecordData);
            }

            int allTryCount = 0;
            int allGetCount = 0;

            foreach (KeyValuePair<string, SpellCardRecordData> keyValuePair in spellCardRecordsDictionary)
            {
                allTryCount += Convert.ToInt32(keyValuePair.Value.TryCount);
                allGetCount += Convert.ToInt32(keyValuePair.Value.GetCount);
            }

            string allGetRate = ScoreCalculator.CalcSpellCardGetRate(allGetCount, allTryCount);

            SpellCardRecordData allSpellCardRecordList = new()
            {
                CardID = cardId.ToString(),
                CardName = spellCard.CardName,
                TryCount = allTryCount.ToString(),
                GetCount = allGetCount.ToString(),
                Rate = allGetRate,
                Level = spellCardRecordsDictionary.FirstOrDefault().Value.Level,
                Enemy = spellCard.Enemy,
                Place = spellCard.Place
            };

            spellCardRecordsDictionary.Add("all", allSpellCardRecordList);

            return spellCardRecordsDictionary;
        }

        private static string LevelReplace(int l)
        {
            if (0 < l && l < 11)
            {
                return "Easy";
            }
            else if (10 < l && l < 21)
            {
                return "Normal";
            }
            else if (20 < l && l < 31)
            {
                return "Hard";
            }
            else if (30 < l && l < 41)
            {
                return "Lunatic";
            }
            else if (40 < l && l < 51)
            {
                return "Extra";
            }
            else
            {
                return "Unknown";
            }
        }
    }
}
