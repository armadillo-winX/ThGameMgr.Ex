﻿using System.Collections.Generic;

namespace ThGameMgr.Ex.Score.Th08
{
    internal class Th08ScoreData
    {
        private static readonly string[] _th08PlayersList = GamePlayers.GetGamePlayers(GameIndex.Th08).Split(',');

        private static readonly Dictionary<string, string> _levelDictionary =
            new()
            {
                { "00", "Easy" },
                { "01", "Normal" },
                { "02", "Hard" },
                { "03", "Lunatic" },
                { "04", "Extra" }
            };

        private static readonly Dictionary<string, string> _progressDictionary =
            new()
            {
                { "00", "Stage1" },
                { "01", "Stage2" },
                { "02", "Stage3" },
                { "03", "Stage4" },
                { "04", "Stage5" },
                { "05", "Stage6(Eirin)" },
                { "06", "Stage6(Kaguya)" },
                { "07", "Extra" },
                { "63", "All Clear" }
            };

        public static void Get()
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
                                Dictionary<string, SpellCardRecordData>
                                    spellCardRecordList
                                    = GetSpellCardRecord(cardAttackData);
                                ScoreData.SpellCardRecordLists.Add(spellCardRecordList["all"]);
                                foreach (string player in GamePlayers.GetGamePlayers(GameIndex.Th08).Split(','))
                                {
                                    ScoreData.SpellCardRecordsByPlayer[player].Add(spellCardRecordList[player]);
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

        private static ScoreRecordData GetHighScoreData(byte[] data)
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

            string player = _th08PlayersList[int.Parse(playerIndex)];
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

        private static Dictionary<string, SpellCardRecordData> GetSpellCardRecord(byte[] data)
        {
            byte[] CATK_DATA = data[0..4];
            byte[] SIZE_DATA = data[4..6];
            byte[] CARD_ID_DATA = data[12..14];
            byte[] LEVEL_DATA = data[15..16];
            byte[] CARD_NAME_DATA = data[16..64];
            byte[] ENEMY_NAME_DATA = data[64..112];
            byte[] COMMENT_DATA = data[112..240];
            byte[] ALL_MAXBONUS_DATA = data[288..292];
            byte[] CHALLENGE_DATA_BY_PLAYER = data[292..340]; //自機別の挑戦数データ(4bytesごと)
            byte[] ALL_CHALLANGE_DATA = data[340..344];
            byte[] GET_DATA_BY_PLAYER = data[344..392]; //自機別の取得数データ(4bytesごと)
            byte[] ALL_GET_DATA = data[392..396];

            int cardId = BitConverter.ToInt16(CARD_ID_DATA, 0) + 1;

            int allChangeCount = BitConverter.ToInt32(ALL_CHALLANGE_DATA);
            int allGetCount = BitConverter.ToInt32(ALL_GET_DATA);

            SpellCard spellcardData = Th08SpellCard.GetSpellCardData(cardId);
            string cardName
                = spellcardData.CardName;

            string rate = ScoreCalculator.CalcSpellCardGetRate(allGetCount, allChangeCount);

            string level = "";
            if (allChangeCount > 0 &&
                _levelDictionary.TryGetValue(BitConverter.ToString(LEVEL_DATA, 0), out string levelName) &&
                spellcardData.Place != "Last Word")
            {
                level = levelName;
            }


            SpellCardRecordData allSpellCardRecordList = new()
            {
                CardID = cardId.ToString(),
                CardName = cardName,
                TryCount = allChangeCount.ToString(),
                GetCount = allGetCount.ToString(),
                Rate = rate,
                Level = level,
                Enemy = spellcardData.Enemy,
                Place = spellcardData.Place
            };

            Dictionary<string, SpellCardRecordData> spellCardRecordsDictionary = [];
            spellCardRecordsDictionary.Add("all", allSpellCardRecordList);

            string[] players = GamePlayers.GetGamePlayers(GameIndex.Th08).Split(',');

            int playerIndex = 0;
            for (int i = 0; i <= 44; i += 4)
            {
                byte[] PLAYER_CHALLENGE_DATA = CHALLENGE_DATA_BY_PLAYER[i..(i + 4)];
                byte[] PLAYER_GET_DATA = GET_DATA_BY_PLAYER[i..(i + 4)];

                int playerChallengeCount = BitConverter.ToInt32(PLAYER_CHALLENGE_DATA, 0);
                int playerGetCount = BitConverter.ToInt32(PLAYER_GET_DATA, 0);

                SpellCardRecordData playerSpellCardRecordData = new()
                {
                    CardID = cardId.ToString(),
                    CardName = cardName,
                    TryCount = playerChallengeCount.ToString(),
                    GetCount = playerGetCount.ToString(),
                    Rate = ScoreCalculator.CalcSpellCardGetRate(playerGetCount, playerChallengeCount),
                    Level = level,
                    Enemy = spellcardData.Enemy,
                    Place = spellcardData.Place
                };

                string player = players[playerIndex];
                playerIndex++;

                spellCardRecordsDictionary.Add(player, playerSpellCardRecordData);
            }

            return spellCardRecordsDictionary;
        }
    }
}
