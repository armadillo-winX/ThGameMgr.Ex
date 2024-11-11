﻿using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace ThGameMgr.Ex.Score.Th13
{
    internal class Th13ScoreView
    {
        public static string[] _th13PlayersList = GamePlayers.GetGamePlayers(GameIndex.Th13).Split(',');

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
            string? gamePath = GameFile.GetGameFilePath(GameIndex.Th13);
            string? scorePath = ScoreFile.GetScoreFilePath(GameIndex.Th13);

            if (File.Exists(gamePath) && File.Exists(scorePath))
            {
                MemoryStream decodedData = new();
                bool decodeResult = ScoreDecoder.Decode(GameIndex.Th13, scorePath, decodedData);
                if (decodeResult)
                {
                    decodedData.Seek(0, SeekOrigin.Begin);
                    using (decodedData)
                    {
                        byte[] bytes = new byte[decodedData.Length];

                        _ = decodedData.Read(bytes, 0, (int)decodedData.Length);

                        int i = 40;
                        int size = 28;
                        for (int k = 0; k < 4; k++)
                        {
                            int l = 1;
                            int max = i + (28 * 50);
                            while (i < max)
                            {
                                int n = i + size;
                                byte[] highscoreData = bytes[i..n];
                                ScoreRecordList scoreRecordList = GetHighScoreData(highscoreData);
                                scoreRecordList.Level = LevelReplace(l);
                                scoreRecordList.Player = _th13PlayersList[k];

                                if (scoreRecordList.Name != "--------")
                                {
                                    ScoreView.ScoreRecordLists.Add(scoreRecordList);
                                }

                                i += size;
                                l++;
                            }
                            i += 20836;
                        }

                        for (int p = 1; p < 128; p++)
                        {
                            ObservableCollection<SpellCardRecordList> spellCardRecordLists
                                = GetAllSpellCardRecord(p, bytes, displayUnchallengedCard);
                            if (spellCardRecordLists[0].Place != "Over Drive")
                            {
                                ScoreView.SpellCardRecordLists.Add(spellCardRecordLists[0]);
                            }

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

        public static ScoreRecordList GetHighScoreData(byte[] data)
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

            ScoreRecordList scoreRecordList = new()
            {
                Score = score,
                Name = name,
                Progress = progress,
                SlowRate = slow,
                Date = date
            };
            return scoreRecordList;
        }

        public static ObservableCollection<SpellCardRecordList> GetSpellCardRecordData(byte[] data)
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

            SpellCardRecordList spellCardRecordList = new()
            {
                Challenge = challenge.ToString(),
                Get = get.ToString()
            };

            SpellCardRecordList practiceSpellCardRecordList = new()
            {
                Challenge = practiceChallenge.ToString(),
                Get = practiceGet.ToString()
            };

            ObservableCollection<SpellCardRecordList> spellCardRecordLists = new();
            spellCardRecordLists.Add(spellCardRecordList);
            spellCardRecordLists.Add(practiceSpellCardRecordList);
            return spellCardRecordLists;
        }

        public static ObservableCollection<SpellCardRecordList> GetAllSpellCardRecord(
            int cardId, byte[] data, bool displayUnchallengedCard)
        {
            int n = cardId - 1;

            int i0 = 2448 + (n * 156);
            int i1 = 24684 + (n * 156);
            int i2 = 46920 + (n * 156);
            int i3 = 69156 + (n * 156);

            int i0end = i0 + 156;
            int i1end = i1 + 156;
            int i2end = i2 + 156;
            int i3end = i3 + 156;

            ObservableCollection<SpellCardRecordList> cardDataReimu = GetSpellCardRecordData(data[i0..i0end]);
            ObservableCollection<SpellCardRecordList> cardDataMarisa = GetSpellCardRecordData(data[i1..i1end]);
            ObservableCollection<SpellCardRecordList> cardDataSanae = GetSpellCardRecordData(data[i2..i2end]);
            ObservableCollection<SpellCardRecordList> cardDataYoumu = GetSpellCardRecordData(data[i3..i3end]);

            int challengeReimu = int.Parse(cardDataReimu[0].Challenge);
            int challengeMarisa = int.Parse(cardDataMarisa[0].Challenge);
            int challengeSanae = int.Parse(cardDataSanae[0].Challenge);
            int challengeYoumu = int.Parse(cardDataYoumu[0].Challenge);

            int getReimu = int.Parse(cardDataReimu[0].Get);
            int getMarisa = int.Parse(cardDataMarisa[0].Get);
            int getSanae = int.Parse(cardDataSanae[0].Get);
            int getYoumu = int.Parse(cardDataYoumu[0].Get);

            int practiceChallengeReimu = int.Parse(cardDataReimu[1].Challenge);
            int practiceChallengeMarisa = int.Parse(cardDataMarisa[1].Challenge);
            int practiceChallengeSanae = int.Parse(cardDataSanae[1].Challenge);
            int practiceChallengeYoumu = int.Parse(cardDataYoumu[1].Challenge);

            int practiceGetReimu = int.Parse(cardDataReimu[1].Get);
            int practiceGetMarisa = int.Parse(cardDataMarisa[1].Get);
            int practiceGetSanae = int.Parse(cardDataSanae[1].Get);
            int practiceGetYoumu = int.Parse(cardDataYoumu[1].Get);

            int allChallengeCount
                = challengeReimu + challengeMarisa + challengeSanae + challengeYoumu;
            int allGetCount
                = getReimu + getMarisa + getSanae + getYoumu;

            int allPracticeChallengeCount
                = practiceChallengeReimu + practiceChallengeMarisa + practiceChallengeSanae + practiceChallengeYoumu;
            int allPracitceGetCount
                = practiceGetReimu + practiceGetMarisa + practiceGetSanae + practiceGetYoumu;

            SpellCard spellcardData = SpellCard.GetSpellCardData(GameIndex.Th13, cardId);

            string cardName = displayUnchallengedCard ?
                spellcardData.CardName :
                allChallengeCount != 0 ? spellcardData.CardName : "-------------------";
            string practiceCardName
                = displayUnchallengedCard ?
                spellcardData.CardName :
                allPracticeChallengeCount != 0 ? spellcardData.CardName : "-------------------";

            string allGetRate = ScoreCalculator.CalcSpellCardGetRate(allGetCount, allChallengeCount);
            string allPracticeGetRate = ScoreCalculator.CalcSpellCardGetRate(allPracitceGetCount, allPracticeChallengeCount);

            SpellCardRecordList allSpellCardRecordList = new()
            {
                CardID = cardId.ToString(),
                CardName = cardName,
                Challenge = allChallengeCount.ToString(),
                Get = allGetCount.ToString(),
                Rate = allGetRate,
                Enemy = spellcardData.Enemy,
                Place = spellcardData.Place
            };

            SpellCardRecordList allPracticeSpellCardRecordList = new()
            {
                CardID = cardId.ToString(),
                CardName = practiceCardName,
                Challenge = allPracticeChallengeCount.ToString(),
                Get = allPracitceGetCount.ToString(),
                Rate = allPracticeGetRate,
                Enemy = spellcardData.Enemy,
                Place = spellcardData.Place
            };

            ObservableCollection<SpellCardRecordList> allSpellCardRecordLists = new();
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
