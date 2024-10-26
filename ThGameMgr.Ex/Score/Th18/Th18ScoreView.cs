﻿using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace ThGameMgr.Ex.Score.Th18
{
    internal class Th18ScoreView
    {
        public static string[] Th18PlayersList = GamePlayers.GetGamePlayers(GameIndex.Th18).Split(',');

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

        public static void GetScoreData(ScoreViewFilter filter, bool displayUnchallengedCard)
        {
            string? gamePath = GameFile.GetGameFilePath(GameIndex.Th18);
            string? scorePath = ScoreFile.GetScoreFilePath(GameIndex.Th18);

            string scoreLevelFilter = filter.ScoreLevelFilter;
            string playerFilter = filter.ScorePlayerFilter;
            string enemyFilter = filter.SpellCardEnemyFilter;

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
                                ScoreRecordList scoreRecordList = GetHighScoreData(highscoreData);
                                scoreRecordList.Level = LevelReplace(l);
                                scoreRecordList.Player = Th18PlayersList[k];

                                if (scoreLevelFilter.ToLower() == "all" && scoreRecordList.Name != "--------")
                                {
                                    if (playerFilter == "ALL" || scoreRecordList.Player == playerFilter)
                                    {
                                        ScoreView.ScoreRecordLists.Add(scoreRecordList);
                                    }
                                }
                                else if (
                                    scoreRecordList.Level == scoreLevelFilter && scoreRecordList.Name != "--------")
                                {
                                    if (playerFilter == "ALL" || scoreRecordList.Player == playerFilter)
                                    {
                                        ScoreView.ScoreRecordLists.Add(scoreRecordList);
                                    }
                                }

                                i += size;
                                l++;
                            }
                            i += 76144;
                        }

                        for (int p = 1; p < 98; p++)
                        {
                            ObservableCollection<SpellCardRecordList> spellCardRecordLists
                                = GetAllSpellCardRecord(p, bytes, displayUnchallengedCard);
                            if (enemyFilter == "ALL")
                            {
                                ScoreView.SpellCardRecordLists.Add(spellCardRecordLists[0]);
                            }
                            else if (spellCardRecordLists[0].Enemy == enemyFilter)
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

            int i0 = 2280 + (n * 220);
            int i1 = 80344 + (n * 220);
            int i2 = 158408 + (n * 220);
            int i3 = 236472 + (n * 220);

            int i0end = i0 + 220;
            int i1end = i1 + 220;
            int i2end = i2 + 220;
            int i3end = i3 + 220;

            ObservableCollection<SpellCardRecordList> cardDataReimu = GetSpellCardRecordData(data[i0..i0end]);
            ObservableCollection<SpellCardRecordList> cardDataMarisa = GetSpellCardRecordData(data[i1..i1end]);
            ObservableCollection<SpellCardRecordList> cardDataSakuya = GetSpellCardRecordData(data[i2..i2end]);
            ObservableCollection<SpellCardRecordList> cardDataSanae = GetSpellCardRecordData(data[i3..i3end]);

            int challengeReimu = int.Parse(cardDataReimu[0].Challenge);
            int challengeMarisa = int.Parse(cardDataMarisa[0].Challenge);
            int challengeSakuya = int.Parse(cardDataSakuya[0].Challenge);
            int challengeSanae = int.Parse(cardDataSanae[0].Challenge);

            int getReimu = int.Parse(cardDataReimu[0].Get);
            int getMarisa = int.Parse(cardDataMarisa[0].Get);
            int getSakuya = int.Parse(cardDataSakuya[0].Get);
            int getSanae = int.Parse(cardDataSanae[0].Get);

            int practiceChallengeReimu = int.Parse(cardDataReimu[1].Challenge);
            int practiceChallengeMarisa = int.Parse(cardDataMarisa[1].Challenge);
            int practiceChallengeSakuya = int.Parse(cardDataSakuya[1].Challenge);
            int practiceChallengeSanae = int.Parse(cardDataSanae[1].Challenge);

            int practiceGetReimu = int.Parse(cardDataReimu[1].Get);
            int practiceGetMarisa = int.Parse(cardDataMarisa[1].Get);
            int practiceGetSakuya = int.Parse(cardDataSakuya[1].Get);
            int practiceGetSanae = int.Parse(cardDataSanae[1].Get);

            int allChallengeCount = challengeReimu + challengeMarisa + challengeSakuya + challengeSanae;
            int allGetCount = getReimu + getMarisa + getSakuya + getSanae;

            int allPracticeChallengeCount = practiceChallengeReimu + practiceChallengeMarisa + practiceChallengeSakuya + practiceChallengeSanae;
            int allPracticeGetCount = practiceGetReimu + practiceGetMarisa + practiceGetSakuya + practiceGetSanae;

            SpellCard spellcardData = SpellCard.GetSpellCardData(GameIndex.Th18, cardId);

            string cardName = displayUnchallengedCard ?
                spellcardData.CardName :
                allChallengeCount != 0 ? spellcardData.CardName : "-------------------";
            string practiceCardName = displayUnchallengedCard ?
                spellcardData.CardName :
                allPracticeChallengeCount != 0 ? spellcardData.CardName : "-------------------";

            string allGetRate = ScoreCalculator.CalcSpellCardGetRate(allGetCount, allChallengeCount);
            string allPracticeGetRate = ScoreCalculator.CalcSpellCardGetRate(allPracticeGetCount, allPracticeChallengeCount);


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
                Get = allPracticeGetCount.ToString(),
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
