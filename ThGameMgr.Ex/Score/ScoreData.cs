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
                Th06.Th06ScoreData.Get(displayUnchallengedCard);
            }
            else if (gameId == GameIndex.Th07)
            {
                Th07.Th07ScoreData.Get(displayUnchallengedCard);
            }
            else if (gameId == GameIndex.Th08)
            {
                Th08.Th08ScoreData.Get(displayUnchallengedCard);
            }
            else if (gameId == GameIndex.Th09)
            {
                Th09.Th09ScoreData.Get();
            }
            else if (gameId == GameIndex.Th10)
            {
                Th10.Th10ScoreData.Get(displayUnchallengedCard);
            }
            else if (gameId == GameIndex.Th11)
            {
                Th11.Th11ScoreData.Get(displayUnchallengedCard);
            }
            else if (gameId == GameIndex.Th12)
            {
                Th12.Th12ScoreData.Get(displayUnchallengedCard);
            }
            else if (gameId == GameIndex.Th13)
            {
                Th13.Th13ScoreData.Get(displayUnchallengedCard);
            }
            else if (gameId == GameIndex.Th14)
            {
                Th14.Th14ScoreData.Get(displayUnchallengedCard);
            }
            else if (gameId == GameIndex.Th15)
            {
                Th15.Th15ScoreData.Get(displayUnchallengedCard);
            }
            else if (gameId == GameIndex.Th16)
            {
                Th16.Th16ScoreData.Get(displayUnchallengedCard);
            }
            else if (gameId == GameIndex.Th17)
            {
                Th17.Th17ScoreData.Get(displayUnchallengedCard);
            }
            else if (gameId == GameIndex.Th18)
            {
                Th18.Th18ScoreData.Get(displayUnchallengedCard);
            }
        }
    }
}
