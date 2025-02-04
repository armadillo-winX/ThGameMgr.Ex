﻿using System.Collections.Generic;

namespace ThGameMgr.Ex.Score.Th09
{
    internal class Th09ScoreData
    {
        private static readonly string[] _th09PlayersList = GamePlayers.GetGamePlayers(GameIndex.Th09).Split(',');

        private static readonly Dictionary<string, string> _levelDictionary =
            new()
            {
                { "00", "Easy" },
                { "01", "Normal" },
                { "02", "Hard" },
                { "03", "Lunatic" },
                { "04", "Extra" }
            };

        public static void Get()
        {
            string? gamePath = GameFile.GetGameFilePath(GameIndex.Th09);
            string? scorePath = ScoreFile.GetScoreFilePath(GameIndex.Th09);

            if (File.Exists(gamePath) && File.Exists(scorePath))
            {
                MemoryStream decodedData = new();
                bool decodedResult = ScoreDecoder.Decode(GameIndex.Th09, scorePath, decodedData);
                if (decodedResult)
                {
                    decodedData.Seek(0, SeekOrigin.Begin);
                    using (decodedData)
                    {
                        byte[] bytes = new byte[decodedData.Length];

                        _ = decodedData.Read(bytes, 0, (int)decodedData.Length);

                        int i = 36;
                        while (i < decodedData.Length)
                        {
                            int n = i + 4;
                            int p = n + 2;
                            //レコードのサイズデータを取得
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
                                if (scoreRecordList.Date.TrimEnd('\0') != "--/--")
                                    ScoreData.ScoreRecordLists.Add(scoreRecordList);

                                i += size;
                            }
                            else
                            {
                                break;
                            }
                        }
                    }
                }
            }
        }

        private static ScoreRecordData GetHighScoreData(byte[] data)
        {
            byte[] HSCR_DATA = data[0..4];
            byte[] SIZE_DATA = data[4..6];
            byte[] SCORE_DATA = data[12..16];
            byte[] PLAYER_DATA = data[20..21];
            byte[] LEVEL_DATA = data[21..22];
            byte[] RANK_DATA = data[22..24];
            byte[] NAME_DATA = data[24..33];
            byte[] DATE_DATA = data[33..41];
            byte[] CONTINUE_DATA = data[43..44];

            int continueCount = Convert.ToInt32(BitConverter.ToString(CONTINUE_DATA, 0), 16);

            string score = String.Format("{0:#,0}", BitConverter.ToInt32(SCORE_DATA, 0) * 10 + continueCount);
            string playerIndex = BitConverter.ToString(PLAYER_DATA, 0);
            string levelIndex = BitConverter.ToString(LEVEL_DATA, 0);

            string player = string.Empty;
            if (Convert.ToInt16(playerIndex, 16) < _th09PlayersList.Length)
            {
                player = _th09PlayersList[Convert.ToInt16(playerIndex, 16)];
            }

            string level = _levelDictionary.ContainsKey(levelIndex) ? _levelDictionary[levelIndex] : "Unknown";

            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            string name = Encoding.GetEncoding("Shift_JIS").GetString(NAME_DATA);
            string date = Encoding.GetEncoding("Shift_JIS").GetString(DATE_DATA);

            ScoreRecordData scoreRecordList = new()
            {
                Score = score,
                Player = player,
                Level = level,
                Name = name,
                Date = date
            };

            return scoreRecordList;
        }
    }
}
