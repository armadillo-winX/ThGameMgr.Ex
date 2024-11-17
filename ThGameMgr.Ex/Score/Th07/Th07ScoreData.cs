﻿using System.Collections.Generic;

namespace ThGameMgr.Ex.Score.Th07
{
    internal class Th07ScoreData
    {
        public static string[] _th07PlayersList = GamePlayers.GetGamePlayers(GameIndex.Th07).Split(',');

        private static readonly Dictionary<string, string> _levelDictionary =
            new()
            {
                { "00", "Easy" },
                { "01", "Normal" },
                { "02", "Hard" },
                { "03", "Lunatic" },
                { "04", "Extra" },
                { "05", "Phantasm" }
            };

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
                { "08", "Phantasm" },
                { "63", "All Clear" }
            };

        public static void Get(bool displayUnchallengedCard)
        {
            string? gamePath = GameFile.GetGameFilePath(GameIndex.Th07);
            string? scorePath = ScoreFile.GetScoreFilePath(GameIndex.Th07);

            if (File.Exists(gamePath) && File.Exists(scorePath))
            {
                MemoryStream decodedData = new();
                bool decodeResult = ScoreDecoder.Decode(GameIndex.Th07, scorePath, decodedData);
                if (decodeResult)
                {
                    decodedData.Seek(0, SeekOrigin.Begin);
                    using (decodedData)
                    {
                        byte[] bytes = new byte[decodedData.Length];

                        _ = decodedData.Read(bytes, 0, (int)decodedData.Length);

                        int i = 40;
                        while (i < decodedData.Length)
                        {
                            int n = i + 4;
                            int p = n + 2;
                            //レコードのデータサイズを取得
                            byte[] sizeData = bytes[n..p];
                            int size = BitConverter.ToInt16(sizeData, 0);

                            int r = i + size;
                            byte[] typeData = bytes[i..n];
                            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
                            string type = Encoding.GetEncoding("Shift_JIS").GetString(typeData);
                            if (type == "HSCR")
                            {
                                byte[] highscoreData = bytes[i..r];
                                ScoreRecordData scoreRecordList
                                    = GetHighScoreData(highscoreData);
                                ScoreData.ScoreRecordLists.Add(scoreRecordList);

                                i += size;
                            }
                            else if (type == "CLRD")
                            {
                                i += size;
                            }
                            else if (type == "CATK")
                            {
                                byte[] cardAttackData = bytes[i..r];
                                SpellCardRecordData spellCardRecordList
                                    = GetSpellCardRecord(cardAttackData, displayUnchallengedCard);
                                ScoreData.SpellCardRecordLists.Add(spellCardRecordList);

                                i += size;
                            }
                            else
                            {
                                break;
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

        public static ScoreRecordData GetHighScoreData(byte[] data)
        {
            byte[] HSCR_DATA = data[0..4];
            byte[] SIZE_DATA = data[4..6];
            byte[] SCORE_DATA = data[12..16];
            byte[] SLOW_DATA = data[16..20];
            byte[] PLAYER_DATA = data[20..21];
            byte[] LEVEL_DATA = data[21..22];
            byte[] PROGRESS_DATA = data[22..23];
            byte[] NAME_DATA = data[23..32];
            byte[] DATE_DATA = data[32..37];

            string score = String.Format("{0:#,0}", BitConverter.ToInt32(SCORE_DATA, 0));
            string slow = BitConverter.ToSingle(SLOW_DATA, 0).ToString("F3") + "%";
            string playerIndex = BitConverter.ToString(PLAYER_DATA, 0);
            string levelIndex = BitConverter.ToString(LEVEL_DATA, 0);
            string progressIndex = BitConverter.ToString(PROGRESS_DATA, 0);

            string player = _th07PlayersList[int.Parse(playerIndex)];
            string level = _levelDictionary.ContainsKey(levelIndex) ? _levelDictionary[levelIndex] : "Unknown";
            string progress = _progressDictionary.ContainsKey(progressIndex) ? _progressDictionary[progressIndex] : "Unknown";

            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            string name = Encoding.GetEncoding("Shift_JIS").GetString(NAME_DATA);
            string date = Encoding.GetEncoding("Shift_JIS").GetString(DATE_DATA);

            ScoreRecordData scoreRecordList = new()
            {
                Score = score,
                Player = player,
                Level = level,
                Name = name,
                Progress = progress,
                SlowRate = slow,
                Date = date
            };
            return scoreRecordList;
        }

        public static SpellCardRecordData GetSpellCardRecord(byte[] data, bool displayUnchallengedCard)
        {
            byte[] CATK_DATA = data[0..4];
            byte[] SIZE_DATA = data[4..6];
            byte[] CARD_ID_DATA = data[40..42];
            byte[] CARD_NAME_DATA = data[42..90];
            byte[] RA_CHALLANGE_DATA = data[92..94];
            byte[] RB_CHALANGE_DATA = data[94..96];
            byte[] MA_CHALLANGE_DATA = data[96..98];
            byte[] MB_CHALLANGE_DATA = data[98..100];
            byte[] SA_CHALLANGE_DATA = data[100..102];
            byte[] SB_CHALLANGE_DATA = data[102..104];
            byte[] ALL_CHALLANGE_DATA = data[104..106];
            byte[] RA_GET_DATA = data[106..108];
            byte[] RB_GET_DATA = data[108..110];
            byte[] MA_GET_DATA = data[110..112];
            byte[] MB_GET_DATA = data[112..114];
            byte[] SA_GET_DATA = data[114..116];
            byte[] SB_GET_DATA = data[116..118];
            byte[] ALL_GET_DATA = data[118..120];

            int cardId = BitConverter.ToInt16(CARD_ID_DATA, 0) + 1;

            int allChangeCount = BitConverter.ToInt16(ALL_CHALLANGE_DATA, 0);
            int allGetCount = BitConverter.ToInt16(ALL_GET_DATA, 0);

            SpellCard spellcardData = SpellCard.GetSpellCardData(GameIndex.Th07, cardId);
            string cardName
                = displayUnchallengedCard ? spellcardData.CardName : allChangeCount != 0 ? spellcardData.CardName : "-------------------";

            string rate = ScoreCalculator.CalcSpellCardGetRate(allGetCount, allChangeCount);

            SpellCardRecordData spellCardRecordList = new()
            {
                CardID = cardId.ToString(),
                CardName = cardName,
                TryCount = allChangeCount.ToString(),
                GetCount = allGetCount.ToString(),
                Rate = rate,
                Enemy = spellcardData.Enemy,
                Place = spellcardData.Place
            };
            return spellCardRecordList;
        }
    }
}