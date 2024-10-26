﻿using System.Collections.ObjectModel;

namespace ThGameMgr.Ex.Score
{
    internal class ScoreView
    {
        public static ObservableCollection<ScoreRecordList>? ScoreRecordLists { get; set; }

        public static ObservableCollection<SpellCardRecordList>? SpellCardRecordLists { get; set; }

        public static ObservableCollection<SpellCardRecordList>? SpellPracticeRecordLists { get; set; }

        public static void GetScoreData(string gameId, ScoreViewFilter filter, bool displayUnchallengedCard)
        {
            ScoreRecordLists = new();
            SpellCardRecordLists = new();
            SpellPracticeRecordLists = new();

            if (gameId == GameIndex.Th06)
            {
                Th06.Th06ScoreView.GetScoreData(filter, displayUnchallengedCard);
            }
            else if (gameId == GameIndex.Th07)
            {
                Th07.Th07ScoreView.GetScoreData(filter, displayUnchallengedCard);
            }
            else if (gameId == GameIndex.Th08)
            {
                Th08.Th08ScoreView.GetScoreData(filter, displayUnchallengedCard);
            }
            else if (gameId == GameIndex.Th10)
            {
                Th10.Th10ScoreView.GetScoreData(filter, displayUnchallengedCard);
            }
            else if (gameId == GameIndex.Th11)
            {
                Th11.Th11ScoreView.GetScoreData(filter, displayUnchallengedCard);
            }
            else if (gameId == GameIndex.Th12)
            {
                Th12.Th12ScoreView.GetScoreData(filter, displayUnchallengedCard);
            }
            else if (gameId == GameIndex.Th13)
            {
                Th13.Th13ScoreView.GetScoreData(filter, displayUnchallengedCard);
            }
            else if (gameId == GameIndex.Th14)
            {
                Th14.Th14ScoreView.GetScoreData(filter, displayUnchallengedCard);
            }
            else if (gameId == GameIndex.Th15)
            {
                Th15.Th15ScoreView.GetScoreData(filter, displayUnchallengedCard);
            }
            else if (gameId == GameIndex.Th16)
            {
                Th16.Th16ScoreView.GetScoreData(filter, displayUnchallengedCard);
            }
            else if (gameId == GameIndex.Th17)
            {
                Th17.Th17ScoreView.GetScoreData(filter, displayUnchallengedCard);
            }
            else if (gameId == GameIndex.Th18)
            {
                Th18.Th18ScoreView.GetScoreData(filter, displayUnchallengedCard);
            }
        }
    }
}
