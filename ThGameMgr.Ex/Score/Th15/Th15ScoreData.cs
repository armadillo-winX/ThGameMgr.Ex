using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace ThGameMgr.Ex.Score.Th15
{
    internal class Th15ScoreData
    {
        private static readonly string[] _th15PlayersList = GamePlayers.GetGamePlayers(GameIndex.Th15).Split(',');

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
                { "08", "All Clear" },
                { "09", "All Clear" }
            };

        public static void Get()
        {
            string? gamePath = GameFile.GetGameFilePath(GameIndex.Th15);
            string? scorePath = ScoreFile.GetScoreFilePath(GameIndex.Th15);

            if (File.Exists(gamePath) && File.Exists(scorePath))
            {
                MemoryStream decodedData = new();
                bool decodeResult = ScoreDecoder.Decode(GameIndex.Th15, scorePath, decodedData);
                if (decodeResult)
                {
                    decodedData.Seek(0, SeekOrigin.Begin);
                    using (decodedData)
                    {
                        byte[] bytes = new byte[decodedData.Length];

                        _ = decodedData.Read(bytes, 0, (int)decodedData.Length);

                        int i = 40;
                        int size = 32;
                        for (int k = 0; k < 8; k++)
                        {
                            int l = 1;
                            int max = i + (32 * 60);
                            while (i < max)
                            {
                                int n = i + size;
                                byte[] highscoreData = bytes[i..n];
                                ScoreRecordData scoreRecordList = GetHighScoreData(highscoreData);
                                scoreRecordList.Level = LevelReplace(l);
                                scoreRecordList.Player = _th15PlayersList[k];

                                if (scoreRecordList.Name != "--------")
                                {
                                    ScoreData.ScoreRecordLists.Add(scoreRecordList);
                                }

                                i += size;
                                l++;
                            }
                            i = k % 2 == 0 ? i + 18952 : i + 19352;
                        }

                        for (int p = 1; p < 120; p++)
                        {
                            ObservableCollection<SpellCardRecordData> spellCardRecordLists
                                = GetAllSpellCardRecord(p, bytes);
                            ScoreData.SpellCardRecordLists.Add(spellCardRecordLists[0]);
                            ScoreData.SpellCardRecordLists.Add(spellCardRecordLists[1]);
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
            byte[] UNKNOWN_DATA = data[20..24];
            byte[] SLOW_DATA = data[24..28];
            byte[] RETRY_DATA = data[28..32];

            int continueCount = Convert.ToInt32(BitConverter.ToString(CONTINUE_DATA, 0), 16);

            string score = String.Format("{0:#,0}", BitConverter.ToInt32(SCORE_DATA, 0) * 10 + continueCount);
            string slow = BitConverter.ToSingle(SLOW_DATA, 0).ToString("F3") + "%";

            int retry = BitConverter.ToInt32(RETRY_DATA, 0);

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
                Date = date,
                OtherData = $"Retry: {retry}"
            };
            return scoreRecordList;
        }

        private static ObservableCollection<SpellCardRecordData> GetSpellCardRecordData(byte[] data)
        {
            byte[] CARD_NAME_DATA = data[0..128];
            byte[] GET_DATA = data[128..132];
            byte[] PRACTICE_GET_DATA = data[132..136];
            byte[] CHALLENGE_DATA = data[136..140];
            byte[] PRACTICE_CHALLENGE_DATA = data[140..144];
            byte[] CARD_ID_DATA = data[144..148];
            byte[] LEVEL_DATA = data[148..152];
            byte[] PRACTOCE_SCORE = data[152..156];

            int cardId = BitConverter.ToInt32(CARD_ID_DATA, 0) + 1;

            int get = BitConverter.ToInt32(GET_DATA, 0);
            int challenge = BitConverter.ToInt32(CHALLENGE_DATA, 0);

            int practiceGet = BitConverter.ToInt32(PRACTICE_GET_DATA, 0);
            int practiceChallenge = BitConverter.ToInt32(PRACTICE_CHALLENGE_DATA, 0);

            string level = SpellCard.ParseLevelData(LEVEL_DATA);

            SpellCardRecordData spellCardRecordList = new()
            {
                CardID = cardId.ToString(),
                TryCount = challenge.ToString(),
                GetCount = get.ToString(),
                Level = level
            };

            SpellCardRecordData practiceSpellCardRecordList = new()
            {
                CardID = cardId.ToString(),
                TryCount = practiceChallenge.ToString(),
                GetCount = practiceGet.ToString(),
                Level = level
            };

            ObservableCollection<SpellCardRecordData> spellCardRecordLists = [spellCardRecordList, practiceSpellCardRecordList];
            return spellCardRecordLists;
        }

        private static Dictionary<string, SpellCardRecordData> GetAllSpellCardRecord(
            int cardId, byte[] data)
        {
            int n = cardId - 1;

            //各自機の御札戦歴データ開始位置
            int[] spellCardRecordStartPositions = [2280, 23152, 44424, 65296, 86568, 107440, 128712, 149584];

            Dictionary<string, SpellCardRecordData> spellCardRecordsDictionary = [];
            string[] players = GamePlayers.GetGamePlayers(GameIndex.Th15).Split(',');
            SpellCard spellCard = Th15SpellCard.GetSpellCardData(cardId);

            for (int playerIndex = 0; playerIndex < 6; playerIndex++)
            {
                int i = spellCardRecordStartPositions[playerIndex] + (n * 156);

                byte[] PLAYER_SPELL_CARD_DATA = data[i..(i + 156)];

                ObservableCollection<SpellCardRecordData>
                    playerSpellCardRecordData = GetSpellCardRecordData(PLAYER_SPELL_CARD_DATA);

                int playerTryCount = Convert.ToInt32(playerSpellCardRecordData[0].TryCount);
                int playerGetCount = Convert.ToInt32(playerSpellCardRecordData[0].GetCount);

                string player = players[playerIndex];

                playerSpellCardRecordData[0].CardName = spellCard.CardName;
                playerSpellCardRecordData[0].Rate
                    = ScoreCalculator.CalcSpellCardGetRate(playerGetCount, playerTryCount);
                playerSpellCardRecordData[0].Enemy = spellCard.Enemy;
                playerSpellCardRecordData[0].Place = spellCard.Place;

                spellCardRecordsDictionary.Add(player, playerSpellCardRecordData[0]);
            }

            int allPerfectTryCount = 0;
            int allPerfectGetCount = 0;

            int allLegacyTryCount = 0;
            int allLegacyGetCount = 0;

            string[] perfectPlayers = ["博麗霊夢(P)", "霧雨魔理沙(P)", "東風谷早苗(P)", "鈴仙・優曇華院・イナバ(P)"];
            string[] legacyPlayers = ["博麗霊夢(L)", "霧雨魔理沙(L)", "東風谷早苗(L)", "鈴仙・優曇華院・イナバ(L)"];

            foreach (string perfectPlayer in perfectPlayers)
            {
                SpellCardRecordData perfectPlayerSpellCardRecord = spellCardRecordsDictionary[perfectPlayer];
                allPerfectTryCount += Convert.ToInt32(perfectPlayerSpellCardRecord.TryCount);
                allPerfectGetCount += Convert.ToInt32(perfectPlayerSpellCardRecord.GetCount);
            }

            foreach (string legacyPlayer in legacyPlayers)
            {
                SpellCardRecordData legacyPlayerSpellCardRecord = spellCardRecordsDictionary[legacyPlayer];
                allLegacyTryCount += Convert.ToInt32(legacyPlayerSpellCardRecord.TryCount);
                allLegacyGetCount += Convert.ToInt32(legacyPlayerSpellCardRecord.GetCount);
            }

            string allPerfectGetRate = ScoreCalculator.CalcSpellCardGetRate(allPerfectGetCount, allPerfectTryCount);
            string allLegacyGetRate = ScoreCalculator.CalcSpellCardGetRate(allLegacyGetCount, allLegacyTryCount);

            SpellCardRecordData allPerfectSpellCardRecord = new()
            {
                CardID = cardId.ToString(),
                CardName = $"{spellCard.CardName}(完全無欠)",
                TryCount = allPerfectTryCount.ToString(),
                GetCount = allPerfectGetCount.ToString(),
                Rate = allPerfectGetRate,
                Level = spellCardRecordsDictionary.FirstOrDefault().Value.Level,
                Enemy = spellCard.Enemy,
                Place = spellCard.Place
            };

            SpellCardRecordData allLegacySpellCardRecord = new()
            {
                CardID = cardId.ToString(),
                CardName = $"{spellCard.CardName}(レガシー)",
                TryCount = allLegacyTryCount.ToString(),
                GetCount = allLegacyGetCount.ToString(),
                Rate = allLegacyGetRate,
                Level = spellCardRecordsDictionary.FirstOrDefault().Value.Level,
                Enemy = spellCard.Enemy,
                Place = spellCard.Place
            };

            spellCardRecordsDictionary.Add("allPerfect", allPerfectSpellCardRecord);
            spellCardRecordsDictionary.Add("allLegacy", allLegacySpellCardRecord);

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
