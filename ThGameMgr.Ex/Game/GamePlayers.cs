using System.Collections.Generic;

namespace ThGameMgr.Ex.Game
{
    internal class GamePlayers
    {
        private readonly static Dictionary<string, string> _gamePlayersDictionary = new()
        {
            {"Th06", "博麗霊夢(霊),博麗霊夢(夢),霧雨魔理沙(魔),霧雨魔理沙(恋)"},
            {"Th07", "博麗霊夢(霊),博麗霊夢(夢),霧雨魔理沙(魔),霧雨魔理沙(恋),十六夜咲夜(幻),十六夜咲夜(時)"},
            {"Th08", "霊夢＆紫,魔理沙＆アリス,咲夜＆レミリア,妖夢＆幽々子,博麗霊夢,八雲紫,霧雨魔理沙,アリス・マーガトロイド,十六夜咲夜,レミリア・スカーレット,魂魄妖夢,西行寺幽々子"},
            {"Th09", ""},
            {"Th10", "霊夢(誘導),霊夢(前方集中),霊夢(封印),魔理沙(高威力),魔理沙(貫通),魔理沙(魔法使い)"},
            {"Th11", "霊夢&紫,霊夢&萃香,霊夢&文,魔理沙&アリス,魔理沙&パチュリー,魔理沙&にとり"},
            {"Th12", "博麗霊夢(霊),博麗霊夢(夢),霧雨魔理沙(恋),霧雨魔理沙(魔),東風谷早苗(蛇),東風谷早苗(蛙)"},
            {"Th13", "博麗霊夢,霧雨魔理沙,東風谷早苗,魂魄妖夢"},
            {"Th14", "博麗霊夢(A),博麗霊夢(B),霧雨魔理沙(A),霧雨魔理沙(B),十六夜咲夜(A),十六夜咲夜(B)"},
            {"Th15", "博麗霊夢(P),博麗霊夢(L),霧雨魔理沙(P),霧雨魔理沙(L),東風谷早苗(P),東風谷早苗(L),鈴仙・優曇華院・イナバ(P),鈴仙・優曇華院・イナバ(L)"},
            {"Th16", "博麗霊夢,日焼けしたチルノ,射命丸文,霧雨魔理沙"},
            {"Th17", "博麗霊夢(狼),博麗霊夢(獺),博麗霊夢(鷲),霧雨魔理沙(狼),霧雨魔理沙(獺),霧雨魔理沙(鷲),魂魄妖夢(狼),魂魄妖夢(獺),魂魄妖夢(鷲)"},
            {"Th18", "博麗霊夢,霧雨魔理沙,十六夜咲夜,東風谷早苗"}
        };

        public static string GetGamePlayers(string gameId)
        {
            if (_gamePlayersDictionary.TryGetValue(gameId, out var gamePlayers))
            {
                return gamePlayers;
            }
            else
            {
                return string.Empty;
            }
        }
    }
}
