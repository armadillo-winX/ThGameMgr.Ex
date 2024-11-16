using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace ThGameMgr.Ex.Score.Th16
{
    internal class Th16ScoreView
    {
        public static string[] _th16PlayersList = GamePlayers.GetGamePlayers(GameIndex.Th16).Split(',');

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

        private static readonly Dictionary<int, string> _th16SeasonDictionary =
            new()
            {
                { 0, "春" },
                { 1, "夏" },
                { 2, "秋" },
                { 3, "冬" },
                { 4, "土用" }
            };

        public static void GetScoreData(bool displayUnchallengedCard)
        {
            string? gamePath = GameFile.GetGameFilePath(GameIndex.Th16);
            string? scorePath = ScoreFile.GetScoreFilePath(GameIndex.Th16);

            if (File.Exists(gamePath) && File.Exists(scorePath))
            {
                MemoryStream decodedData = new();
                bool decodeResult = ScoreDecoder.Decode(GameIndex.Th16, scorePath, decodedData);
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
                                scoreRecordList.Player = _th16PlayersList[k];

                                if (scoreRecordList.Name != "--------")
                                {
                                    ScoreView.ScoreRecordLists.Add(scoreRecordList);
                                }

                                i += size;
                                l++;
                            }
                            i += 19352;
                        }

                        for (int p = 1; p < 120; p++)
                        {
                            ObservableCollection<SpellCardRecordData> spellCardRecordLists
                                = GetAllSpellCardRecord(p, bytes, displayUnchallengedCard);
                            ScoreView.SpellCardRecordLists.Add(spellCardRecordLists[0]);
                            ScoreView.SpellPracticeRecordLists.Add(spellCardRecordLists[1]);
                        }
                    }
                }
                else
                {
                    throw new InvalidDataException("スコアファイルが不正です。");
                }
            }
        }

        public static ScoreRecordData GetHighScoreData(byte[] data)
        {
            byte[] SCORE_DATA = data[0..4];
            byte[] PROGRESS_DATA = data[4..5];
            byte[] CONTINUE_DATA = data[5..6];
            byte[] NAME_DATA = data[6..16];
            byte[] DATE_DATA = data[16..20];
            byte[] UNKNOWN_DATA = data[20..24];
            byte[] SLOW_DATA = data[24..28];
            byte[] SEASON_DATA = data[28..32];

            int continueCount = Convert.ToInt32(BitConverter.ToString(CONTINUE_DATA, 0), 16);

            string score = String.Format("{0:#,0}", BitConverter.ToUInt32(SCORE_DATA, 0) * 10 + continueCount);
            string slow = BitConverter.ToSingle(SLOW_DATA, 0).ToString("F3") + "%";

            string season = _th16SeasonDictionary[BitConverter.ToInt32(SEASON_DATA, 0)];

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
                OtherData = $"季節: {season}"
            };
            return scoreRecordList;
        }

        public static ObservableCollection<SpellCardRecordData> GetSpellCardRecordData(byte[] data)
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

            SpellCardRecordData spellCardRecordList = new()
            {
                Challenge = challenge.ToString(),
                Get = get.ToString()
            };

            SpellCardRecordData practiceSpellCardRecordList = new()
            {
                Challenge = practiceChallenge.ToString(),
                Get = practiceGet.ToString()
            };

            ObservableCollection<SpellCardRecordData> spellCardRecordLists = new();
            spellCardRecordLists.Add(spellCardRecordList);
            spellCardRecordLists.Add(practiceSpellCardRecordList);
            return spellCardRecordLists;
        }

        public static ObservableCollection<SpellCardRecordData> GetAllSpellCardRecord(
            int cardId, byte[] data, bool displayUnchallengedCard)
        {
            int n = cardId - 1;

            int i0 = 2280 + (n * 156);
            int i1 = 23552 + (n * 156);
            int i2 = 44824 + (n * 156);
            int i3 = 66096 + (n * 156);

            int i0end = i0 + 156;
            int i1end = i1 + 156;
            int i2end = i2 + 156;
            int i3end = i3 + 156;

            ObservableCollection<SpellCardRecordData> cardDataReimu = GetSpellCardRecordData(data[i0..i0end]);
            ObservableCollection<SpellCardRecordData> cardDataChirno = GetSpellCardRecordData(data[i1..i1end]);
            ObservableCollection<SpellCardRecordData> cardDataAya = GetSpellCardRecordData(data[i2..i2end]);
            ObservableCollection<SpellCardRecordData> cardDataMarisa = GetSpellCardRecordData(data[i3..i3end]);

            int challengeReimu = int.Parse(cardDataReimu[0].Challenge);
            int challengeChirno = int.Parse(cardDataChirno[0].Challenge);
            int challengeAya = int.Parse(cardDataAya[0].Challenge);
            int challengeMarisa = int.Parse(cardDataMarisa[0].Challenge);

            int getReimu = int.Parse(cardDataReimu[0].Get);
            int getChirno = int.Parse(cardDataChirno[0].Get);
            int getAya = int.Parse(cardDataAya[0].Get);
            int getMarisa = int.Parse(cardDataMarisa[0].Get);

            int practiceChallengeReimu = int.Parse(cardDataReimu[1].Challenge);
            int practiceChallengeChirno = int.Parse(cardDataChirno[1].Challenge);
            int practiceChallengeAya = int.Parse(cardDataAya[1].Challenge);
            int practiceChallengeMarisa = int.Parse(cardDataMarisa[1].Challenge);

            int practiceGetReimu = int.Parse(cardDataReimu[1].Get);
            int practiceGetChirno = int.Parse(cardDataChirno[1].Get);
            int practiceGetAya = int.Parse(cardDataAya[1].Get);
            int practiceGetMarisa = int.Parse(cardDataMarisa[1].Get);

            int allChallengeCount = challengeReimu + challengeChirno + challengeAya + challengeMarisa;
            int allGetCount = getReimu + getChirno + getAya + getMarisa;

            int allPracticeChallengeCount = practiceChallengeReimu + practiceChallengeChirno + practiceChallengeAya + practiceChallengeMarisa;
            int allPracticeGetCount = practiceGetReimu + practiceGetChirno + practiceGetAya + practiceGetMarisa;

            SpellCard spellcardData = SpellCard.GetSpellCardData(GameIndex.Th16, cardId);

            string cardName = displayUnchallengedCard ?
                spellcardData.CardName :
                allChallengeCount != 0 ? spellcardData.CardName : "-------------------";
            string practiceCardName = displayUnchallengedCard ?
                spellcardData.CardName :
                allPracticeChallengeCount != 0 ? spellcardData.CardName : "-------------------";

            string allGetRate = ScoreCalculator.CalcSpellCardGetRate(allGetCount, allChallengeCount);
            string allPracticeGetRate = ScoreCalculator.CalcSpellCardGetRate(allPracticeGetCount, allPracticeChallengeCount);

            SpellCardRecordData allSpellCardRecordList = new()
            {
                CardID = cardId.ToString(),
                CardName = cardName,
                Challenge = allChallengeCount.ToString(),
                Get = allGetCount.ToString(),
                Rate = allGetRate,
                Enemy = spellcardData.Enemy,
                Place = spellcardData.Place
            };

            SpellCardRecordData allPracticeSpellCardRecordList = new()
            {
                CardID = cardId.ToString(),
                CardName = practiceCardName,
                Challenge = allPracticeChallengeCount.ToString(),
                Get = allPracticeGetCount.ToString(),
                Rate = allPracticeGetRate,
                Enemy = spellcardData.Enemy,
                Place = spellcardData.Place
            };

            ObservableCollection<SpellCardRecordData> allSpellCardRecordLists = new();
            allSpellCardRecordLists.Add(allSpellCardRecordList);
            allSpellCardRecordLists.Add(allPracticeSpellCardRecordList);
            return allSpellCardRecordLists;
        }

        public static string LevelReplace(int l)
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
