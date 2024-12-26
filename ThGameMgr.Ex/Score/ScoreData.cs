using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace ThGameMgr.Ex.Score
{
    internal class ScoreData
    {
        public static string? GameId { get; set; }

        public static ObservableCollection<ScoreRecordData>? ScoreRecordLists { get; set; }

        public static ObservableCollection<SpellCardRecordData>? SpellCardRecordLists { get; set; }

        public static ObservableCollection<SpellCardRecordData>? SpellPracticeRecordLists { get; set; }

        public static void Get(string gameId)
        {
            GameId = gameId;

            ScoreRecordLists = [];
            SpellCardRecordLists = [];
            SpellPracticeRecordLists = [];

            if (gameId == GameIndex.Th06)
            {
                Th06.Th06ScoreData.Get();
            }
            else if (gameId == GameIndex.Th07)
            {
                Th07.Th07ScoreData.Get();
            }
            else if (gameId == GameIndex.Th08)
            {
                Th08.Th08ScoreData.Get();
            }
            else if (gameId == GameIndex.Th09)
            {
                Th09.Th09ScoreData.Get();
            }
            else if (gameId == GameIndex.Th10)
            {
                Th10.Th10ScoreData.Get();
            }
            else if (gameId == GameIndex.Th11)
            {
                Th11.Th11ScoreData.Get();
            }
            else if (gameId == GameIndex.Th12)
            {
                Th12.Th12ScoreData.Get();
            }
            else if (gameId == GameIndex.Th13)
            {
                Th13.Th13ScoreData.Get();
            }
            else if (gameId == GameIndex.Th14)
            {
                Th14.Th14ScoreData.Get();
            }
            else if (gameId == GameIndex.Th15)
            {
                Th15.Th15ScoreData.Get();
            }
            else if (gameId == GameIndex.Th16)
            {
                Th16.Th16ScoreData.Get();
            }
            else if (gameId == GameIndex.Th17)
            {
                Th17.Th17ScoreData.Get();
            }
            else if (gameId == GameIndex.Th18)
            {
                Th18.Th18ScoreData.Get();
            }
        }

        public static void ExportToTextFile(
            string outputPath, bool outputUntriedCardData, ScoreFilter scoreFilter,  string? comment)
        {
            string data 
                = $"{GameIndex.GetGameName(GameId)}スコアデータ\r\nExported by {VersionInfo.AppName} Version.{VersionInfo.AppVersion}\r\n\r\n";

            IEnumerable<ScoreRecordData> filteredScoreRecordLists = ScoreRecordLists;

            if (!string.IsNullOrEmpty(scoreFilter.Level) && scoreFilter.Level.ToLower() != "all")
            {
                filteredScoreRecordLists = filteredScoreRecordLists.Where(
                        x =>
                        {
                            return x.Level == scoreFilter.Level;
                        }
                    );
            }

            if (!string.IsNullOrEmpty(scoreFilter.Player) && scoreFilter.Player.ToLower() != "all")
            {
                filteredScoreRecordLists = filteredScoreRecordLists.Where(
                        x =>
                        {
                            return x.Player == scoreFilter.Player;
                        }
                    );
            }

            if (ScoreRecordLists.Count > 0)
            {
                data += $"ハイスコア\r\n-----------------------------------------------------\r\n";
                foreach (ScoreRecordData scoreRecordData in filteredScoreRecordLists)
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
                data += $"御札戦歴\r\n-----------------------------------------------------\r\n";
                foreach (SpellCardRecordData spellCardRecordData in SpellCardRecordLists)
                {
                    if (int.Parse(spellCardRecordData.TryCount) > 0)
                    {
                        data += $"No.{spellCardRecordData.CardID}\r\n{spellCardRecordData.CardName}\r\n取得数: {spellCardRecordData.GetCount}\r\n挑戦数: {spellCardRecordData.TryCount}\r\n取得率: {spellCardRecordData.Rate}\r\n発動場所: {spellCardRecordData.Place}\r\n術者: {spellCardRecordData.Enemy}\r\n\r\n";
                    }
                    else
                    {
                        if (outputUntriedCardData)
                        {
                            data += $"No.{spellCardRecordData.CardID}\r\n{spellCardRecordData.CardName}\r\n取得数: {spellCardRecordData.GetCount}\r\n挑戦数: {spellCardRecordData.TryCount}\r\n取得率: {spellCardRecordData.Rate}\r\n発動場所: {spellCardRecordData.Place}\r\n術者: {spellCardRecordData.Enemy}\r\n\r\n";
                        }
                    }
                }
            }

            SpellCardStatisticsData? spellCardStatisticsData = AnalyzeSpellCardStatisitcs();
            if (spellCardStatisticsData != null)
            {
                data += $"御札戦歴の統計\r\n-----------------------------------------------------\r\n";
                data +=
                    $"取得済みスペルカードの数: {spellCardStatisticsData.GetCardCount}\r\n" +
                    $"挑戦済みスペルカードの数: {spellCardStatisticsData.TriedCardCount}\r\n" +
                    $"取得済みスペルカードの挑戦済みスペルカードに対する割合: {spellCardStatisticsData.GetCardCountRate}\r\n\r\n" +
                    $"スペルカード取得数の合計: {spellCardStatisticsData.TotalGetCount}\r\n" +
                    $"スペルカード挑戦数の合計: {spellCardStatisticsData.TotalTryCount}\r\n" +
                    $"スペルカード合計取得数のスペルカード合計挑戦数に対する割合: {spellCardStatisticsData.TotalGetCountRate}\r\n\r\n";
            }

            if (SpellPracticeRecordLists.Count > 0)
            {
                data += $"スペルプラクティス\r\n-----------------------------------------------------\r\n";
                foreach (SpellCardRecordData spellCardRecordData in SpellPracticeRecordLists)
                {
                    if (int.Parse(spellCardRecordData.TryCount) > 0)
                    {
                        data += $"No.{spellCardRecordData.CardID}\r\n{spellCardRecordData.CardName}\r\n取得数: {spellCardRecordData.GetCount}\r\n挑戦数: {spellCardRecordData.TryCount}\r\n取得率: {spellCardRecordData.Rate}\r\n発動場所: {spellCardRecordData.Place}\r\n術者: {spellCardRecordData.Enemy}\r\n\r\n";
                    }
                    else
                    {
                        if (outputUntriedCardData)
                        {
                            data += $"No.{spellCardRecordData.CardID}\r\n{spellCardRecordData.CardName}\r\n取得数: {spellCardRecordData.GetCount}\r\n挑戦数: {spellCardRecordData.TryCount}\r\n取得率: {spellCardRecordData.Rate}\r\n発動場所: {spellCardRecordData.Place}\r\n術者: {spellCardRecordData.Enemy}\r\n\r\n";
                        }
                    }
                }
            }

            if (!string.IsNullOrEmpty(comment))
            {
                data += $"コメント\r\n-----------------------------------------------------\r\n{comment}\r\n";
            }

            StreamWriter streamWriter = new(outputPath, false);
            streamWriter.Write(data);
            streamWriter.Close();
        }

        public static SpellCardStatisticsData? AnalyzeSpellCardStatisitcs()
        {
            if (SpellCardRecordLists != null &&
                SpellCardRecordLists.Count > 0)
            {
                int challengedCardCount = 0;
                int getCardCount = 0;

                int totalChallengeCount = 0;
                int totalGetCount = 0;

                for (int i = 0; i < SpellCardRecordLists.Count; i++)
                {
                    SpellCardRecordData spellCardRecordList = SpellCardRecordLists[i];
                    int challengeCount = int.Parse(spellCardRecordList.TryCount);
                    int getCount = int.Parse(spellCardRecordList.GetCount);

                    if (challengeCount > 0) challengedCardCount++;
                    if (getCount > 0) getCardCount++;

                    totalChallengeCount += challengeCount;
                    totalGetCount += getCount;
                }

                string getCardRate = ScoreCalculator.CalcSpellCardGetRate(getCardCount, challengedCardCount);
                string totalGetRate = ScoreCalculator.CalcSpellCardGetRate(totalGetCount, totalChallengeCount);

                return new SpellCardStatisticsData
                {
                    GetCardCount = getCardCount,
                    TriedCardCount = challengedCardCount,
                    GetCardCountRate = getCardRate,
                    TotalGetCount = totalGetCount,
                    TotalTryCount = totalChallengeCount,
                    TotalGetCountRate = totalGetRate
                };
            }
            else
            {
                return null;
            }
        }
    }
}
