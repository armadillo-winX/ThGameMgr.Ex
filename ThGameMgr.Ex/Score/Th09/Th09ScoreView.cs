using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThGameMgr.Ex.Score.Th09
{
    internal class Th09ScoreView
    {
        public static string[] _th09PlayersList = GamePlayers.GetGamePlayers(GameIndex.Th09).Split(',');

        private static readonly Dictionary<string, string> _levelDictionary =
            new()
            {
                { "00", "Easy" },
                { "01", "Normal" },
                { "02", "Hard" },
                { "03", "Lunatic" },
                { "04", "Extra" }
            };

        public static void GetScoreData(bool displayUnchallengedCard)
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
                                ScoreRecordList scoreRecordList
                                    = GetHighScoreData(highscoreData);
                                ScoreView.ScoreRecordLists.Add(scoreRecordList);

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

        public static ScoreRecordList GetHighScoreData(byte[] data)
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

            string score = String.Format("{0:#,0}", BitConverter.ToInt32(SCORE_DATA, 0));
            string playerIndex = BitConverter.ToString(PLAYER_DATA, 0);
            string levelIndex = BitConverter.ToString(LEVEL_DATA, 0);

            string player = _th09PlayersList[int.Parse(playerIndex)];
            string level = _levelDictionary.ContainsKey(levelIndex) ? _levelDictionary[levelIndex] : "Unknown";

            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            string name = Encoding.GetEncoding("Shift_JIS").GetString(NAME_DATA);
            string date = Encoding.GetEncoding("Shift_JIS").GetString(DATE_DATA);

            ScoreRecordList scoreRecordList = new()
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
