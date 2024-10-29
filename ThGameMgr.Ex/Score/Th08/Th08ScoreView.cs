using System.Collections.Generic;

namespace ThGameMgr.Ex.Score.Th08
{
    internal class Th08ScoreView
    {
        public static string[] Th08PlayersList = GamePlayers.GetGamePlayers(GameIndex.Th08).Split(',');

        private static readonly Dictionary<string, string> LevelDictionary =
            new()
            {
                { "00", "Easy" },
                { "01", "Normal" },
                { "02", "Hard" },
                { "03", "Lunatic" },
                { "04", "Extra" }
            };

        private static readonly Dictionary<string, string> ProgressDictionary =
            new()
            {
                { "01", "Stage1" },
                { "02", "Stage2" },
                { "03", "Stage3" },
                { "04", "Stage4" },
                { "05", "Stage5" },
                { "06", "Stage6(Eirin)" },
                { "07", "Stage6(Kaguya)" },
                { "08", "Extra" },
                { "63", "All Clear" }
            };

        public static void GetScoreData(bool displayUnchallengedCard)
        {
            string? gamePath = GameFile.GetGameFilePath(GameIndex.Th08);
            string? scorePath = ScoreFile.GetScoreFilePath(GameIndex.Th08);

            if (File.Exists(gamePath) && File.Exists(scorePath))
            {
                MemoryStream decodedData = new();
                bool decodeResult = ScoreDecoder.Decode(GameIndex.Th08, scorePath, decodedData);
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
                                ScoreRecordList scoreRecordList
                                    = GetHighScoreData(highscoreData);
                                ScoreView.ScoreRecordLists.Add(scoreRecordList);

                                i += size;
                            }
                            else if (type == "CLRD")
                            {
                                i += size;
                            }
                            else if (type == "CATK")
                            {
                                byte[] cardAttackData = bytes[i..r];
                                SpellCardRecordList spellCardRecordList
                                    = GetSpellCardRecord(cardAttackData, displayUnchallengedCard);
                                ScoreView.SpellCardRecordLists.Add(spellCardRecordList);

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

        public static ScoreRecordList GetHighScoreData(byte[] data)
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

            string player = Th08PlayersList[int.Parse(playerIndex)];
            string level = LevelDictionary.ContainsKey(levelIndex) ? LevelDictionary[levelIndex] : "Unknown";
            string progress = ProgressDictionary.ContainsKey(progressIndex) ? ProgressDictionary[progressIndex] : "Unknown";

            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            string name = Encoding.GetEncoding("Shift_JIS").GetString(NAME_DATA);
            string date = Encoding.GetEncoding("Shift_JIS").GetString(DATE_DATA);

            ScoreRecordList scoreRecordList = new()
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

        public static SpellCardRecordList GetSpellCardRecord(byte[] data, bool displayUnchallengedCard)
        {
            byte[] CATK_DATA = data[0..4];
            byte[] SIZE_DATA = data[4..6];
            byte[] CARD_ID_DATA = data[12..14];
            byte[] CARD_NAME_DATA = data[16..64];
            byte[] ENEMY_NAME_DATA = data[64..112];
            byte[] COMMENT_DATA = data[112..240];
            byte[] ALL_MAXBONUS_DATA = data[288..292];
            byte[] ALL_CHALLANGE_DATA = data[340..344];
            byte[] ALL_GET_DATA = data[392..396];

            int cardId = BitConverter.ToInt16(CARD_ID_DATA, 0) + 1;

            int allChangeCount = Convert.ToInt32(BitConverter.ToString(ALL_CHALLANGE_DATA, 0).Replace("-", ""), 16);
            int allGetCount = Convert.ToInt32(BitConverter.ToString(ALL_GET_DATA, 0).Replace("-", ""), 16);

            SpellCard spellcardData = SpellCard.GetSpellCardData(GameIndex.Th08, cardId);
            string cardName
                = displayUnchallengedCard ? spellcardData.CardName : allChangeCount != 0 ? spellcardData.CardName : "-------------------";

            string rate = ScoreCalculator.CalcSpellCardGetRate(allGetCount, allChangeCount);

            SpellCardRecordList spellCardRecordList = new()
            {
                CardID = cardId.ToString(),
                CardName = cardName,
                Challenge = allChangeCount.ToString(),
                Get = allGetCount.ToString(),
                Rate = rate,
                Enemy = spellcardData.Enemy,
                Place = spellcardData.Place
            };
            return spellCardRecordList;
        }
    }
}
