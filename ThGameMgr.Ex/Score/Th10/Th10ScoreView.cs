﻿using System.Collections.Generic;

namespace ThGameMgr.Ex.Score.Th10
{
    internal class Th10ScoreView
    {
        public static string[] Th10PlayersList = GamePlayers.GetGamePlayers(GameIndex.Th10).Split(',');

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

        public static void GetScoreData(ScoreViewFilter filter, bool displayUnchallengedCard)
        {
            string? gamePath = GameFile.GetGameFilePath(GameIndex.Th10);
            string? scorePath = ScoreFile.GetScoreFilePath(GameIndex.Th10);

            string scoreLevelFilter = filter.ScoreLevelFilter;
            string playerFilter = filter.ScorePlayerFilter;
            string enemyFilter = filter.SpellCardEnemyFilter;

            if (File.Exists(gamePath) && File.Exists(scorePath))
            {
                MemoryStream decodedData = new();
                bool decodeResult = ScoreDecoder.Decode(GameIndex.Th10, scorePath, decodedData);
                if (decodeResult)
                {
                    decodedData.Seek(0, SeekOrigin.Begin);
                    using (decodedData)
                    {
                        byte[] bytes = new byte[decodedData.Length];

                        _ = decodedData.Read(bytes, 0, (int)decodedData.Length);

                        int i = 40;
                        int size = 24;

                        for (int k = 0; k < 6; k++)
                        {
                            int l = 1;
                            int max = i + (24 * 50);
                            while (i < max)
                            {
                                int n = i + size;
                                byte[] highScoreData = bytes[i..n];
                                ScoreRecordList scoreRecordList = GetHighScoreData(highScoreData);
                                scoreRecordList.Level = LevelReplace(l);
                                scoreRecordList.Player = Th10PlayersList[k];

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
                            i = i + 220 + (144 * 110) + 16;
                        }

                        for (int p = 1; p < 111; p++)
                        {
                            SpellCardRecordList spellCardRecordList =
                                GetAllSpellCardRecord(p, bytes, displayUnchallengedCard);
                            if (enemyFilter == "ALL")
                            {
                                ScoreView.SpellCardRecordLists.Add(spellCardRecordList);
                            }
                            else if (spellCardRecordList.Enemy == enemyFilter)
                            {
                                ScoreView.SpellCardRecordLists.Add(spellCardRecordList);
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

        public static ScoreRecordList GetHighScoreData(byte[] data)
        {
            byte[] SCORE_DATA = data[0..4];
            byte[] PROGRESS_DATA = data[4..5];
            byte[] CONTINUE_DATA = data[5..6];
            byte[] NAME_DATA = data[6..16];
            byte[] DATE_DATA = data[16..20];
            byte[] SLOW_DATA = data[20..24];

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

        public static SpellCardRecordList GetSpellCardRecordData(byte[] data)
        {
            byte[] CARD_NAME_DATA = data[0..128];
            byte[] GET_DATA = data[128..132];
            byte[] CHALLENGE_DATA = data[132..136];
            byte[] CARD_ID_DATA = data[136..140];
            byte[] LEVEL_DATA = data[140..144];

            int cardId = BitConverter.ToInt32(CARD_ID_DATA, 0) + 1;

            string get = BitConverter.ToInt32(GET_DATA, 0).ToString();
            string challenge = BitConverter.ToInt32(CHALLENGE_DATA, 0).ToString();

            SpellCardRecordList spellCardRecordList = new()
            {
                CardID = cardId.ToString(),
                Get = get,
                Challenge = challenge
            };
            return spellCardRecordList;
        }

        public static SpellCardRecordList GetAllSpellCardRecord(int cardId, byte[] data, bool displayUnchallengedCard)
        {
            int n = cardId - 1;

            int i0 = 1460 + (n * 144);
            int i1 = 18736 + (n * 144);
            int i2 = 36012 + (n * 144);
            int i3 = 53288 + (n * 144);
            int i4 = 70564 + (n * 144);
            int i5 = 87840 + (n * 144);

            int i0end = i0 + 144;
            int i1end = i1 + 144;
            int i2end = i2 + 144;
            int i3end = i3 + 144;
            int i4end = i4 + 144;
            int i5end = i5 + 144;

            SpellCardRecordList cardDataReimuA = GetSpellCardRecordData(data[i0..i0end]);
            SpellCardRecordList cardDataReimuB = GetSpellCardRecordData(data[i1..i1end]);
            SpellCardRecordList cardDataReimuC = GetSpellCardRecordData(data[i2..i2end]);
            SpellCardRecordList cardDataMarisaA = GetSpellCardRecordData(data[i3..i3end]);
            SpellCardRecordList cardDataMarisaB = GetSpellCardRecordData(data[i4..i4end]);
            SpellCardRecordList cardDataMarisaC = GetSpellCardRecordData(data[i5..i5end]);

            int challengeReimuA = int.Parse(cardDataReimuA.Challenge);
            int challengeReimuB = int.Parse(cardDataReimuB.Challenge);
            int challengeReimuC = int.Parse(cardDataReimuC.Challenge);
            int challengeMarisaA = int.Parse(cardDataMarisaA.Challenge);
            int challengeMarisaB = int.Parse(cardDataMarisaB.Challenge);
            int challengeMarisaC = int.Parse(cardDataMarisaC.Challenge);

            int getReimuA = int.Parse(cardDataReimuA.Get);
            int getReimuB = int.Parse(cardDataReimuB.Get);
            int getReimuC = int.Parse(cardDataReimuC.Get);
            int getMarisaA = int.Parse(cardDataMarisaA.Get);
            int getMarisaB = int.Parse(cardDataMarisaB.Get);
            int getMarisaC = int.Parse(cardDataMarisaC.Get);

            int allChallenge
                = challengeReimuA + challengeReimuB + challengeReimuC + challengeMarisaA + challengeMarisaB + challengeMarisaC;
            int allGet
                = getReimuA + getReimuB + getReimuC + getMarisaA + getMarisaB + getMarisaC;

            SpellCard spellcardData = SpellCard.GetSpellCardData(GameIndex.Th10, cardId);
            string cardName
                = displayUnchallengedCard ? spellcardData.CardName : allChallenge != 0 ? spellcardData.CardName : "-------------------";

            string allGetRate = ScoreCalculator.CalcSpellCardGetRate(allGet, allChallenge);

            SpellCardRecordList allSpellCardRecordList = new()
            {
                CardID = cardId.ToString(),
                CardName = cardName,
                Challenge = allChallenge.ToString(),
                Get = allGet.ToString(),
                Rate = allGetRate,
                Enemy = spellcardData.Enemy,
                Place = spellcardData.Place
            };
            return allSpellCardRecordList;
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
