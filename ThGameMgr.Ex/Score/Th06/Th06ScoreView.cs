using System.Collections.Generic;

namespace ThGameMgr.Ex.Score.Th06
{
    internal class Th06ScoreView
    {
        public static string[] Th06PlayersList = GamePlayers.GetGamePlayers(GameIndex.Th06).Split(',');

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
                { "06", "Stage6" },
                { "07", "Extra" },
                { "63", "All Clear" }
            };

        public static void GetScoreData(ScoreViewFilter filter, bool displayUnchallengedCard)
        {
            string? gamePath = GameFile.GetGameFilePath(GameIndex.Th06);
            string? scorePath = ScoreFile.GetScoreFilePath(GameIndex.Th06);

            string scoreLevelFilter = filter.ScoreLevelFilter;
            string playerFilter = filter.ScorePlayerFilter;
            string enemyFilter = filter.SpellCardEnemyFilter;

            if (File.Exists(gamePath) && File.Exists(scorePath))
            {
                MemoryStream decodedData = new();
                bool decodedResult = ScoreDecoder.Decode(GameIndex.Th06, scorePath, decodedData);
                if (decodedResult)
                {
                    decodedData.Seek(0, SeekOrigin.Begin);
                    using (decodedData)
                    {
                        byte[] bytes = new byte[decodedData.Length];
                        _ = decodedData.Read(bytes, 0, (int)decodedData.Length);

                        int i = 32;
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
                                if (scoreLevelFilter.ToLower() == "all" && playerFilter == "ALL")
                                {
                                    ScoreView.ScoreRecordLists.Add(scoreRecordList);
                                }
                                else if (scoreRecordList.Level == scoreLevelFilter && playerFilter == "ALL")
                                {
                                    ScoreView.ScoreRecordLists.Add(scoreRecordList);
                                }
                                else if (scoreLevelFilter.ToLower() == "all" && scoreRecordList.Player == playerFilter)
                                {
                                    ScoreView.ScoreRecordLists.Add(scoreRecordList);
                                }
                                else if (scoreRecordList.Level == scoreLevelFilter && scoreRecordList.Player == playerFilter)
                                {
                                    ScoreView.ScoreRecordLists.Add(scoreRecordList);
                                }

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
                                if (enemyFilter == "ALL")
                                {
                                    ScoreView.SpellCardRecordLists.Add(spellCardRecordList);
                                }
                                else if (spellCardRecordList.Enemy == enemyFilter)
                                {
                                    ScoreView.SpellCardRecordLists.Add(spellCardRecordList);
                                }

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
            byte[] PLAYER_DATA = data[16..17];
            byte[] LEVEL_DATA = data[17..18];
            byte[] PROGRESS_DATA = data[18..19];
            byte[] NAME_DATA = data[19..28];

            string score = string.Format("{0:#,0}", BitConverter.ToInt32(SCORE_DATA, 0));
            int playerIndex = int.Parse(BitConverter.ToString(PLAYER_DATA, 0));
            string player = Th06PlayersList[playerIndex];
            string levelIndex = BitConverter.ToString(LEVEL_DATA, 0);
            string level = LevelDictionary[levelIndex];
            string progressIndex = BitConverter.ToString(PROGRESS_DATA, 0);
            string progress = ProgressDictionary[progressIndex];

            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            string name = Encoding.GetEncoding("Shift_JIS").GetString(NAME_DATA);

            ScoreRecordList scoreRecordList = new()
            {
                Score = score,
                Player = player,
                Level = level,
                Name = name,
                Progress = progress,
                SlowRate = "-.--%",
                Date = "--/--"
            };
            return scoreRecordList;
        }

        public static SpellCardRecordList GetSpellCardRecord(byte[] data, bool displayUnchallengedCard)
        {
            byte[] CATK_DATA = data[0..4];
            byte[] SIZE_DATA = data[4..6];
            byte[] CARD_ID_DATA = data[16..18];
            byte[] CARD_NAME_DATA = data[24..60];
            byte[] CHALLANGE_DATA = data[60..62];
            byte[] GET_DATA = data[62..64];

            int cardId = BitConverter.ToInt16(CARD_ID_DATA, 0) + 1;
            int challenge = BitConverter.ToInt16(CHALLANGE_DATA, 0);
            int get = BitConverter.ToInt16(GET_DATA, 0);

            SpellCard spellcardData = SpellCard.GetSpellCardData(GameIndex.Th06, cardId);
            string cardName
                = displayUnchallengedCard ? spellcardData.CardName : challenge != 0 ? spellcardData.CardName : "-------------------";

            string rate = ScoreCalculator.CalcSpellCardGetRate(get, challenge);

            SpellCardRecordList spellCardRecordList = new()
            {
                CardID = cardId.ToString(),
                CardName = cardName,
                Get = get.ToString(),
                Challenge = challenge.ToString(),
                Rate = rate,
                Enemy = spellcardData.Enemy,
                Place = spellcardData.Place
            };
            return spellCardRecordList;
        }
    }
}
