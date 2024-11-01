﻿using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace ThGameMgr.Ex.Score.Th15
{
    internal class Th15ScoreView
    {
        public static string[] Th15PlayersList = GamePlayers.GetGamePlayers(GameIndex.Th15).Split(',');

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
                                ScoreRecordList scoreRecordList = GetHighScoreData(highscoreData);
                                scoreRecordList.Level = LevelReplace(l);
                                scoreRecordList.Player = Th15PlayersList[k];

                                if (scoreRecordList.Name != "--------")
                                {
                                    ScoreView.ScoreRecordLists.Add(scoreRecordList);
                                }

                                i += size;
                                l++;
                            }
                            i = k % 2 == 0 ? i + 18952 : i + 19352;
                        }

                        for (int p = 1; p < 120; p++)
                        {
                            ObservableCollection<SpellCardRecordList> spellCardRecordLists
                                = GetAllSpellCardRecord(p, bytes, displayUnchallengedCard);
                            ScoreView.SpellCardRecordLists.Add(spellCardRecordLists[0]);
                            ScoreView.SpellCardRecordLists.Add(spellCardRecordLists[1]);
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

            ScoreRecordList scoreRecordList = new()
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

            int i0 = 2280 + (n * 156);
            int i1 = 23152 + (n * 156);
            int i2 = 44424 + (n * 156);
            int i3 = 65296 + (n * 156);
            int i4 = 86568 + (n * 156);
            int i5 = 107440 + (n * 156);
            int i6 = 128712 + (n * 156);
            int i7 = 149584 + (n * 156);

            int i0end = i0 + 156;
            int i1end = i1 + 156;
            int i2end = i2 + 156;
            int i3end = i3 + 156;
            int i4end = i4 + 156;
            int i5end = i5 + 156;
            int i6end = i6 + 156;
            int i7end = i7 + 156;

            ObservableCollection<SpellCardRecordList> cardDataReimuA = GetSpellCardRecordData(data[i0..i0end]);
            ObservableCollection<SpellCardRecordList> cardDataReimuB = GetSpellCardRecordData(data[i1..i1end]);
            ObservableCollection<SpellCardRecordList> cardDataMarisaA = GetSpellCardRecordData(data[i2..i2end]);
            ObservableCollection<SpellCardRecordList> cardDataMarisaB = GetSpellCardRecordData(data[i3..i3end]);
            ObservableCollection<SpellCardRecordList> cardDataSanaeA = GetSpellCardRecordData(data[i4..i4end]);
            ObservableCollection<SpellCardRecordList> cardDataSanaeB = GetSpellCardRecordData(data[i5..i5end]);
            ObservableCollection<SpellCardRecordList> cardDataUdonA = GetSpellCardRecordData(data[i6..i6end]);
            ObservableCollection<SpellCardRecordList> cardDataUdonB = GetSpellCardRecordData(data[i7..i7end]);

            int challengeReimuA = int.Parse(cardDataReimuA[0].Challenge);
            int challengeReimuB = int.Parse(cardDataReimuB[0].Challenge);
            int challengeMarisaA = int.Parse(cardDataMarisaA[0].Challenge);
            int challengeMarisaB = int.Parse(cardDataMarisaB[0].Challenge);
            int challengeSanaeA = int.Parse(cardDataSanaeA[0].Challenge);
            int challengeSanaeB = int.Parse(cardDataSanaeB[0].Challenge);
            int challengeUdonA = int.Parse(cardDataUdonA[0].Challenge);
            int challengeUdonB = int.Parse(cardDataUdonB[0].Challenge);

            int getReimuA = int.Parse(cardDataReimuA[0].Get);
            int getReimuB = int.Parse(cardDataReimuB[0].Get);
            int getMarisaA = int.Parse(cardDataMarisaA[0].Get);
            int getMarisaB = int.Parse(cardDataMarisaB[0].Get);
            int getSanaeA = int.Parse(cardDataSanaeA[0].Get);
            int getSanaeB = int.Parse(cardDataSanaeB[0].Get);
            int getUdonA = int.Parse(cardDataUdonA[0].Get);
            int getUdonB = int.Parse(cardDataUdonB[0].Get);

            int allPerfectChallenge = challengeReimuA + challengeMarisaA + challengeSanaeA + challengeUdonA;
            int allPerfectGet = getReimuA + getMarisaA + getSanaeA + getUdonA;

            int allLegacyChallenge = challengeReimuB + challengeMarisaB + challengeSanaeB + challengeUdonB;
            int allLegacyGet = getReimuB + getMarisaB + getSanaeB + getUdonB;

            SpellCard spellcardData = SpellCard.GetSpellCardData(GameIndex.Th15, cardId);

            string perfectCardName = displayUnchallengedCard ?
                $"{spellcardData.CardName}(P)" :
                allPerfectChallenge != 0 ? $"{spellcardData.CardName}(P)" : "-------------------(P)";
            string legacyCardName = displayUnchallengedCard ?
                $"{spellcardData.CardName}(L)" :
                allLegacyChallenge != 0 ? $"{spellcardData.CardName}(L)" : "-------------------(L)";

            string allPerfectGetRate = ScoreCalculator.CalcSpellCardGetRate(allPerfectGet, allPerfectChallenge);
            string allLegacyGetRate = ScoreCalculator.CalcSpellCardGetRate(allLegacyGet, allLegacyChallenge);

            SpellCardRecordList perfectSpellCardRecordList = new()
            {
                CardID = cardId.ToString(),
                CardName = perfectCardName,
                Challenge = allPerfectChallenge.ToString(),
                Get = allPerfectGet.ToString(),
                Rate = allPerfectGetRate,
                Enemy = spellcardData.Enemy,
                Place = spellcardData.Place
            };

            SpellCardRecordList legacySpellCardRecordList = new()
            {
                CardID = cardId.ToString(),
                CardName = legacyCardName,
                Challenge = allLegacyChallenge.ToString(),
                Get = allLegacyGet.ToString(),
                Rate = allLegacyGetRate,
                Enemy = spellcardData.Enemy,
                Place = spellcardData.Place
            };

            ObservableCollection<SpellCardRecordList> allSpellCardRecordLists = new();
            allSpellCardRecordLists.Add(perfectSpellCardRecordList);
            allSpellCardRecordLists.Add(legacySpellCardRecordList);
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
