namespace ThGameMgr.Ex.Core

open System.Collections.Generic

module GameName =

    let private legacyGameNamesIndex = 
        Map [
        (GameId.Th01, "東方靈異伝")
        (GameId.Th02, "東方封魔録")
        (GameId.Th03, "東方夢時空")
        (GameId.Th04, "東方幻想郷")
        (GameId.Th05, "東方怪綺談")
        ]
    
    let private gameforWinNamesIndex = 
        Map [
        (GameId.Th06, "東方紅魔郷")
        (GameId.Th07, "東方妖々夢")
        (GameId.Th08, "東方永夜抄") 
        (GameId.Th09, "東方花映塚")
        (GameId.Th10, "東方風神録")
        (GameId.Th11, "東方地霊殿")
        (GameId.Th12, "東方星蓮船")
        (GameId.Th13, "東方神霊廟")
        (GameId.Th14, "東方輝針城")
        (GameId.Th15, "東方紺珠伝")
        (GameId.Th16, "東方天空璋")
        (GameId.Th17, "東方鬼形獣")
        (GameId.Th18, "東方虹龍洞")
        (GameId.Th19, "東方獣王園")
        ]
