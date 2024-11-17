using System.Collections.ObjectModel;

namespace ThGameMgr.Ex.Score
{
    internal class ScoreData
    {
        public static ObservableCollection<ScoreRecordData>? ScoreRecordLists { get; set; }

        public static ObservableCollection<SpellCardRecordData>? SpellCardRecordLists { get; set; }

        public static ObservableCollection<SpellCardRecordData>? SpellPracticeRecordLists { get; set; }

        public static void Get(string gameId, bool displayUnchallengedCard)
        {
            ScoreRecordLists = new();
            SpellCardRecordLists = new();
            SpellPracticeRecordLists = new();

            if (gameId == GameIndex.Th06)
            {
                Th06.Th06ScoreView.GetScoreData(displayUnchallengedCard);
            }
            else if (gameId == GameIndex.Th07)
            {
                Th07.Th07ScoreView.GetScoreData(displayUnchallengedCard);
            }
            else if (gameId == GameIndex.Th08)
            {
                Th08.Th08ScoreView.GetScoreData(displayUnchallengedCard);
            }
            else if (gameId == GameIndex.Th09)
            {
                Th09.Th09ScoreView.GetScoreData();
            }
            else if (gameId == GameIndex.Th10)
            {
                Th10.Th10ScoreView.GetScoreData(displayUnchallengedCard);
            }
            else if (gameId == GameIndex.Th11)
            {
                Th11.Th11ScoreView.GetScoreData(displayUnchallengedCard);
            }
            else if (gameId == GameIndex.Th12)
            {
                Th12.Th12ScoreView.GetScoreData(displayUnchallengedCard);
            }
            else if (gameId == GameIndex.Th13)
            {
                Th13.Th13ScoreView.GetScoreData(displayUnchallengedCard);
            }
            else if (gameId == GameIndex.Th14)
            {
                Th14.Th14ScoreView.GetScoreData(displayUnchallengedCard);
            }
            else if (gameId == GameIndex.Th15)
            {
                Th15.Th15ScoreView.GetScoreData(displayUnchallengedCard);
            }
            else if (gameId == GameIndex.Th16)
            {
                Th16.Th16ScoreView.GetScoreData(displayUnchallengedCard);
            }
            else if (gameId == GameIndex.Th17)
            {
                Th17.Th17ScoreView.GetScoreData(displayUnchallengedCard);
            }
            else if (gameId == GameIndex.Th18)
            {
                Th18.Th18ScoreView.GetScoreData(displayUnchallengedCard);
            }
        }

        public static void ExportToTextFile(string gameId, string outputPath)
        {
            string data 
                = $"{GameIndex.GetGameName(gameId)}スコアデータ\r\nExported by {VersionInfo.AppName} Version.{VersionInfo.AppVersion}\r\n\r\n";
            
            if (ScoreRecordLists.Count > 0)
            {
                data += $"ハイスコア\r\n\r\n-----------------------------------------------------\r\n";
                foreach (ScoreRecordData scoreRecordData in ScoreRecordLists)
                {
                    data +=
                        $"スコア: {scoreRecordData.Score}\r\n自機:{scoreRecordData.Player}\r\n難易度:{scoreRecordData.Level}\r\n名前:{scoreRecordData.Name.TrimEnd('\0')}";
                    if (!string.IsNullOrEmpty(scoreRecordData.Progress))
                        data += $"\r\n到達面:{scoreRecordData.Progress}";
                    if (!string.IsNullOrEmpty(scoreRecordData.Date.TrimEnd('\0'))
                        && scoreRecordData.Date.TrimEnd('\0') != "--/--")
                        data += $"\r\n日時:{scoreRecordData.Date.TrimEnd('\0')}";
                    if (!string.IsNullOrEmpty(scoreRecordData.SlowRate)
                        && scoreRecordData.SlowRate != "-.--%")
                        data += $"\r\n処理落ち率:{scoreRecordData.SlowRate}";
                    if (!string.IsNullOrEmpty(scoreRecordData.OtherData))
                        data += $"\r\nその他\r\n{scoreRecordData.OtherData}";

                    data += "\r\n\r\n";
                }
            }

            if (SpellCardRecordLists.Count > 0)
            {
                data += $"御札戦歴\r\n\r\n-------------------------------------------------------\r\n";
                foreach (SpellCardRecordData spellCardRecordData in SpellCardRecordLists)
                {
                    data += $"No.{spellCardRecordData.CardID}\r\n{spellCardRecordData.CardName}\r\n取得数: {spellCardRecordData.GetCount}\r\n挑戦数: {spellCardRecordData.TryCount}\r\n取得率: {spellCardRecordData.Rate}\r\n発動場所: {spellCardRecordData.Place}\r\n術者: {spellCardRecordData.Enemy}\r\n\r\n";
                }
            }

            StreamWriter streamWriter = new(outputPath, false);
            streamWriter.Write(data);
            streamWriter.Close();
        }
    }
}
