using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThGameMgr.Ex.Score.Th09
{
    internal class Th09ScoreView
    {
        public static string[] _th09PlayersList = GamePlayers.GetGamePlayers(GameIndex.Th09).Split(',');
    }
}
