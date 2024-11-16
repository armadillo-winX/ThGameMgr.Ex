using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace ThGameMgr.Ex.Score.Th17
{
    internal class Th17ScoreView
    {
        public static string[] _th17PlayersList = GamePlayers.GetGamePlayers(GameIndex.Th17).Split(',');

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

        public static void GetScoreData(bool displayUnchallengedCard)
        {
            string? gamePath = GameFile.GetGameFilePath(GameIndex.Th17);
            string? scorePath = ScoreFile.GetScoreFilePath(GameIndex.Th17);

            if (File.Exists(gamePath) && File.Exists(scorePath))
            {
                MemoryStream decodedData = new();
                bool decodeResult = ScoreDecoder.Decode(GameIndex.Th17, scorePath, decodedData);
                if (decodeResult)
                {
                    decodedData.Seek(0, SeekOrigin.Begin);
                    using (decodedData)
                    {
                        byte[] bytes = new byte[decodedData.Length];

                        _ = decodedData.Read(bytes, 0, (int)decodedData.Length);

                        int i = 40;
                        int size = 32;
                        for (int k = 0; k < 9; k++)
                        {
                            int l = 1;
                            int max = i + (32 * 60);
                            while (i < max)
                            {
                                int n = i + size;
                                byte[] highscoreData = bytes[i..n];
                                ScoreRecordData scoreRecordList = GetHighScoreData(highscoreData);
                                scoreRecordList.Level = LevelReplace(l);
                                scoreRecordList.Player = _th17PlayersList[k];

                                if (scoreRecordList.Name != "--------")
                                {
                                    ScoreView.ScoreRecordLists.Add(scoreRecordList);
                                }

                                i += size;
                                l++;
                            }
                            i += 16544;
                        }

                        for (int p = 1; p < 102; p++)
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
                TryCount = challenge.ToString(),
                GetCount = get.ToString()
            };

            SpellCardRecordData practiceSpellCardRecordList = new()
            {
                TryCount = practiceChallenge.ToString(),
                GetCount = practiceGet.ToString()
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
            int i1 = 20744 + (n * 156);
            int i2 = 39208 + (n * 156);
            int i3 = 57672 + (n * 156);
            int i4 = 76136 + (n * 156);
            int i5 = 94600 + (n * 156);
            int i6 = 113064 + (n * 156);
            int i7 = 131528 + (n * 156);
            int i8 = 149992 + (n * 156);

            int i0end = i0 + 156;
            int i1end = i1 + 156;
            int i2end = i2 + 156;
            int i3end = i3 + 156;
            int i4end = i4 + 156;
            int i5end = i5 + 156;
            int i6end = i6 + 156;
            int i7end = i7 + 156;
            int i8end = i8 + 156;

            ObservableCollection<SpellCardRecordData> cardDataReimuA = GetSpellCardRecordData(data[i0..i0end]);
            ObservableCollection<SpellCardRecordData> cardDataReimuB = GetSpellCardRecordData(data[i1..i1end]);
            ObservableCollection<SpellCardRecordData> cardDataReimuC = GetSpellCardRecordData(data[i2..i2end]);
            ObservableCollection<SpellCardRecordData> cardDataMarisaA = GetSpellCardRecordData(data[i3..i3end]);
            ObservableCollection<SpellCardRecordData> cardDataMarisaB = GetSpellCardRecordData(data[i4..i4end]);
            ObservableCollection<SpellCardRecordData> cardDataMarisaC = GetSpellCardRecordData(data[i5..i5end]);
            ObservableCollection<SpellCardRecordData> cardDataYoumuA = GetSpellCardRecordData(data[i6..i6end]);
            ObservableCollection<SpellCardRecordData> cardDataYoumuB = GetSpellCardRecordData(data[i7..i7end]);
            ObservableCollection<SpellCardRecordData> cardDataYoumuC = GetSpellCardRecordData(data[i8..i8end]);

            int challengeReimuA = int.Parse(cardDataReimuA[0].TryCount);
            int challengeReimuB = int.Parse(cardDataReimuB[0].TryCount);
            int challengeReimuC = int.Parse(cardDataReimuC[0].TryCount);
            int challengeMarisaA = int.Parse(cardDataMarisaA[0].TryCount);
            int challengeMarisaB = int.Parse(cardDataMarisaB[0].TryCount);
            int challengeMarisaC = int.Parse(cardDataMarisaC[0].TryCount);
            int challengeYoumuA = int.Parse(cardDataYoumuA[0].TryCount);
            int challengeYoumuB = int.Parse(cardDataYoumuB[0].TryCount);
            int challengeYoumuC = int.Parse(cardDataYoumuC[0].TryCount);

            int getReimuA = int.Parse(cardDataReimuA[0].GetCount);
            int getReimuB = int.Parse(cardDataReimuB[0].GetCount);
            int getReimuC = int.Parse(cardDataReimuC[0].GetCount);
            int getMarisaA = int.Parse(cardDataMarisaA[0].GetCount);
            int getMarisaB = int.Parse(cardDataMarisaB[0].GetCount);
            int getMarisaC = int.Parse(cardDataMarisaC[0].GetCount);
            int getYoumuA = int.Parse(cardDataYoumuA[0].GetCount);
            int getYoumuB = int.Parse(cardDataYoumuB[0].GetCount);
            int getYoumuC = int.Parse(cardDataYoumuC[0].GetCount);

            int practiceChallengeReimuA = int.Parse(cardDataReimuA[1].TryCount);
            int practiceChallengeReimuB = int.Parse(cardDataReimuB[1].TryCount);
            int practiceChallengeReimuC = int.Parse(cardDataReimuC[1].TryCount);
            int practiceChallengeMarisaA = int.Parse(cardDataMarisaA[1].TryCount);
            int practiceChallengeMarisaB = int.Parse(cardDataMarisaB[1].TryCount);
            int practiceChallengeMarisaC = int.Parse(cardDataMarisaC[1].TryCount);
            int practiceChallengeYoumuA = int.Parse(cardDataYoumuA[1].TryCount);
            int practiceChallengeYoumuB = int.Parse(cardDataYoumuB[1].TryCount);
            int practiceChallengeYoumuC = int.Parse(cardDataYoumuC[1].TryCount);

            int practiceGetReimuA = int.Parse(cardDataReimuA[1].GetCount);
            int practiceGetReimuB = int.Parse(cardDataReimuB[1].GetCount);
            int practiceGetReimuC = int.Parse(cardDataReimuC[1].GetCount);
            int practiceGetMarisaA = int.Parse(cardDataMarisaA[1].GetCount);
            int practiceGetMarisaB = int.Parse(cardDataMarisaB[1].GetCount);
            int practiceGetMarisaC = int.Parse(cardDataMarisaC[1].GetCount);
            int practiceGetYoumuA = int.Parse(cardDataYoumuA[1].GetCount);
            int practiceGetYoumuB = int.Parse(cardDataYoumuB[1].GetCount);
            int practiceGetYoumuC = int.Parse(cardDataYoumuC[1].GetCount);

            int allChallengeCount
                = challengeReimuA + challengeReimuB + challengeReimuC + challengeMarisaA + challengeMarisaB + challengeMarisaC + challengeYoumuA + challengeYoumuB + challengeYoumuC;
            int allGetCount = getReimuA + getReimuB + getReimuC + getMarisaA + getMarisaB + getMarisaC + getYoumuA + getYoumuB + getYoumuC;

            int allPracticeChallengeCount
                = practiceChallengeReimuA + practiceChallengeReimuB + practiceChallengeReimuC + practiceChallengeMarisaA + practiceChallengeMarisaB + practiceChallengeMarisaC + practiceChallengeYoumuA + practiceChallengeYoumuB + practiceChallengeYoumuC;
            int allPracticeGetCount
                = practiceGetReimuA + practiceGetReimuB + practiceGetReimuC + practiceGetMarisaA + practiceGetMarisaB + practiceGetMarisaC + practiceGetYoumuA + practiceGetYoumuB + practiceGetYoumuC;

            SpellCard spellcardData = SpellCard.GetSpellCardData(GameIndex.Th17, cardId);

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
                TryCount = allChallengeCount.ToString(),
                GetCount = allGetCount.ToString(),
                Rate = allGetRate,
                Enemy = spellcardData.Enemy,
                Place = spellcardData.Place
            };

            SpellCardRecordData allPracticeSpellCardRecordList = new()
            {
                CardID = cardId.ToString(),
                CardName = practiceCardName,
                TryCount = allPracticeChallengeCount.ToString(),
                GetCount = allPracticeGetCount.ToString(),
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
