using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace ThGameMgr.Ex.Score.Th18
{
    internal class Th18ScoreData
    {
        private static readonly string[] _th18PlayersList = GamePlayers.GetGamePlayers(GameIndex.Th18).Split(',');

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
            string? gamePath = GameFile.GetGameFilePath(GameIndex.Th18);
            string? scorePath = ScoreFile.GetScoreFilePath(GameIndex.Th18);

            if (File.Exists(gamePath) && File.Exists(scorePath))
            {
                MemoryStream decodedData = new();
                bool decodeResult = ScoreDecoder.Decode(GameIndex.Th18, scorePath, decodedData);
                if (decodeResult)
                {
                    decodedData.Seek(0, SeekOrigin.Begin);
                    using (decodedData)
                    {
                        byte[] bytes = new byte[decodedData.Length];

                        _ = decodedData.Read(bytes, 0, (int)decodedData.Length);

                        int i = 40;
                        int size = 32;
                        for (int k = 0; k < 4; k++)
                        {
                            int l = 1;
                            int max = i + (32 * 60);
                            while (i < max)
                            {
                                int n = i + size;
                                byte[] highscoreData = bytes[i..n];
                                ScoreRecordData scoreRecordList = GetHighScoreData(highscoreData);
                                scoreRecordList.Level = LevelReplace(l);
                                scoreRecordList.Player = _th18PlayersList[k];

                                if (scoreRecordList.Name != "--------")
                                {
                                    ScoreData.ScoreRecordLists.Add(scoreRecordList);
                                }

                                i += size;
                                l++;
                            }
                            i += 76144;
                        }

                        for (int p = 1; p < 98; p++)
                        {
                            Dictionary<string, ObservableCollection<SpellCardRecordData>>
                                spellCardRecordLists =
                                GetAllSpellCardRecord(p, bytes);
                            ScoreData.SpellCardRecordLists.Add(spellCardRecordLists["all"][0]);

                            foreach (string player in GamePlayers.GetGamePlayers(GameIndex.Th18).Split(','))
                            {
                                ScoreData.SpellCardRecordsByPlayer[player].Add(spellCardRecordLists[player][0]);
                            }
                            ScoreData.SpellPracticeRecordLists.Add(spellCardRecordLists["all"][1]);
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
            _ = data[20..24];
            byte[] SLOW_DATA = data[24..28];
            _ = data[28..32];

            int continueCount = Convert.ToInt32(BitConverter.ToString(CONTINUE_DATA, 0), 16);

            string score = String.Format("{0:#,0}", BitConverter.ToUInt32(SCORE_DATA, 0) * 10 + continueCount);
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

        private static ObservableCollection<SpellCardRecordData> GetSpellCardRecordData(byte[] data)
        {
            byte[] CARD_NAME_DATA = data[0..128];
            byte[] GET_DATA = data[192..196];
            byte[] PRACTICE_GET_DATA = data[196..200];
            byte[] CHALLENGE_DATA = data[200..204];
            byte[] PRACTICE_CHALLENGE_DATA = data[204..208];
            byte[] CARD_ID_DATA = data[208..212];
            byte[] LEVEL_DATA = data[212..216];
            byte[] PRACTOCE_SCORE = data[216..220];

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

        private static Dictionary<string, ObservableCollection<SpellCardRecordData>> GetAllSpellCardRecord(
            int cardId, byte[] data)
        {
            int n = cardId - 1;

            //Reimu  Spell Card Record start at:   2280 bytes
            //Marisa Spell Card Record start at:  80344 bytes
            //Sakuya Spell Card Record start at: 158408 bytes
            //Sanae  Spell Card Record start at: 236472 bytes

            Dictionary<string, ObservableCollection<SpellCardRecordData>> spellCardRecordsDictionary = [];
            string[] players = GamePlayers.GetGamePlayers(GameIndex.Th18).Split(',');
            SpellCard spellCard = Th18SpellCard.GetSpellCardData(cardId);

            for (int playerIndex = 0; playerIndex < 4; playerIndex++)
            {
                int i = 2280 + (78064 * playerIndex) + (n * 220);

                byte[] PLAYER_SPELL_CARD_DATA = data[i..(i + 220)];

                ObservableCollection<SpellCardRecordData>
                    playerSpellCardRecordData = GetSpellCardRecordData(PLAYER_SPELL_CARD_DATA);

                int playerTryCount = Convert.ToInt32(playerSpellCardRecordData[0].TryCount);
                int playerGetCount = Convert.ToInt32(playerSpellCardRecordData[0].GetCount);

                int playerPracticeTryCount = Convert.ToInt32(playerSpellCardRecordData[1].TryCount);
                int playerPracticeGetCount = Convert.ToInt32(playerSpellCardRecordData[1].GetCount);

                string player = players[playerIndex];

                playerSpellCardRecordData[0].CardName = spellCard.CardName;
                playerSpellCardRecordData[1].CardName = spellCard.CardName;
                playerSpellCardRecordData[0].Rate
                    = ScoreCalculator.CalcSpellCardGetRate(playerGetCount, playerTryCount);
                playerSpellCardRecordData[1].Rate
                    = ScoreCalculator.CalcSpellCardGetRate(playerPracticeGetCount, playerPracticeTryCount);
                playerSpellCardRecordData[0].Enemy = spellCard.Enemy;
                playerSpellCardRecordData[1].Enemy = spellCard.Enemy;
                playerSpellCardRecordData[0].Place = spellCard.Place;
                playerSpellCardRecordData[1].Place = spellCard.Place;

                spellCardRecordsDictionary.Add(player, playerSpellCardRecordData);
            }

            int allTryCount = 0;
            int allGetCount = 0;

            int allPracticeTryCount = 0;
            int allPracticeGetCount = 0;

            foreach (
                KeyValuePair<string, ObservableCollection<SpellCardRecordData>> keyValuePair in spellCardRecordsDictionary)
            {
                allTryCount += Convert.ToInt32(keyValuePair.Value[0].TryCount);
                allGetCount += Convert.ToInt32(keyValuePair.Value[0].GetCount);
                allPracticeTryCount += Convert.ToInt32(keyValuePair.Value[1].TryCount);
                allPracticeGetCount += Convert.ToInt32(keyValuePair.Value[1].GetCount);
            }

            string allGetRate = ScoreCalculator.CalcSpellCardGetRate(allGetCount, allTryCount);
            string allPracticeGetRate = ScoreCalculator.CalcSpellCardGetRate(allPracticeGetCount, allPracticeTryCount);

            SpellCardRecordData allSpellCardRecord = new()
            {
                CardID = cardId.ToString(),
                CardName = spellCard.CardName,
                TryCount = allTryCount.ToString(),
                GetCount = allGetCount.ToString(),
                Rate = allGetRate,
                Level = spellCardRecordsDictionary.FirstOrDefault().Value[0].Level,
                Enemy = spellCard.Enemy,
                Place = spellCard.Place
            };

            SpellCardRecordData allSpellPracticeRecord = new()
            {
                CardID = cardId.ToString(),
                CardName = spellCard.CardName,
                TryCount = allPracticeTryCount.ToString(),
                GetCount = allPracticeGetCount.ToString(),
                Rate = allPracticeGetRate,
                Level = spellCardRecordsDictionary.FirstOrDefault().Value[1].Level,
                Enemy = spellCard.Enemy,
                Place = spellCard.Place
            };

            spellCardRecordsDictionary.Add("all", [allSpellCardRecord, allSpellPracticeRecord]);

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
