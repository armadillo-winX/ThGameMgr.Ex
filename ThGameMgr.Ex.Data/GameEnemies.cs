﻿using System.Collections.Generic;

namespace ThGameMgr.Ex.Data
{
    internal class GameEnemies
    {
        private readonly static Dictionary<string, string> _gameEnemiesDictionary = new()
        {
            {"Th06", "ルーミア,チルノ,紅美鈴,パチュリー・ノーレッジ,十六夜咲夜,レミリア・スカーレット,フランドール・スカーレット"},
            {"Th07", "チルノ,レティ・ホワイトロック,橙,アリス・マーガトロイド,プリズムリバー三姉妹,魂魄妖夢,西行寺幽々子,八雲藍,八雲紫"},
            {"Th08", "リグル・ナイトバグ,ミスティア・ローレライ,上白沢慧音,因幡てゐ,鈴仙・優曇華院・イナバ,八意永琳,蓬莱山輝夜,藤原妹紅,博麗霊夢,霧雨魔理沙,十六夜咲夜,魂魄妖夢,アリス・マーガトロイド,レミリア・スカーレット,西行寺幽々子,八雲紫"},
            {"Th09", ""},
            {"Th10", "秋静葉,秋穣子,鍵山雛,河城にとり,射命丸文,東風谷早苗,八坂神奈子,洩矢諏訪子"},
            {"Th11", "キスメ,黒谷ヤマメ,水橋パルスィ,星熊勇儀,古明地さとり,火焔猫燐,霊烏路空,東風谷早苗,古明地こいし"},
            {"Th12", "ナズーリン,多々良小傘,雲居一輪＆雲山,村紗水蜜,寅丸星,聖白蓮,封獣ぬえ"},
            {"Th13", "西行寺幽々子,幽谷響子,多々良小傘,宮古芳香,霍青娥,蘇我屠自古,物部布都,豊聡耳神子,封獣ぬえ,二ッ岩マミゾウ"},
            {"Th14", "チルノ,わかさぎ姫,赤蛮奇,今泉影狼,九十九弁々,九十九八橋,九十九姉妹,鬼人正邪,少名針妙丸,堀川雷鼓"},
            {"Th15", "清蘭,鈴瑚,ドレミー・スイート,稀神サグメ,クラウンピース,純狐,ヘカーティア・ラピスラズリ"},
            {"Th16", "エタニティラルバ,坂田ネムノ,リリーホワイト,高麗野あうん,矢田寺成美,爾子田里乃＆丁礼田舞,摩多羅隠岐奈"},
            {"Th17", "戎瓔花,牛崎潤美,庭渡久侘歌,吉弔八千慧,杖刀偶磨弓,埴安神袿姫,驪駒早鬼"},
            {"Th18", "豪徳寺ミケ,山城たかね,駒草山如,玉造魅須丸,飯綱丸龍,天弓千亦,菅牧典,姫虫百々世"}
        };

        public static string GetGameEnemies(string gameId)
        {
            if (_gameEnemiesDictionary.TryGetValue(gameId, out var gameEnemies))
            {
                return gameEnemies;
            }
            else
            {
                return string.Empty;
            }
        }
    }
}
