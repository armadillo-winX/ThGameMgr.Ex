namespace ThGameMgr.Ex.Core

module GameNameIndex =

    let private legacyGameNamesIndex = 
        Map [
        (GameIdIndex.Th01, "東方靈異伝")
        (GameIdIndex.Th02, "東方封魔録")
        (GameIdIndex.Th03, "東方夢時空")
        (GameIdIndex.Th04, "東方幻想郷")
        (GameIdIndex.Th05, "東方怪綺談")
        ]
    
    let private winGameNamesIndex = 
        Map [
        (GameIdIndex.Th06, "東方紅魔郷")
        (GameIdIndex.Th07, "東方妖々夢")
        (GameIdIndex.Th08, "東方永夜抄") 
        (GameIdIndex.Th09, "東方花映塚")
        (GameIdIndex.Th10, "東方風神録")
        (GameIdIndex.Th11, "東方地霊殿")
        (GameIdIndex.Th12, "東方星蓮船")
        (GameIdIndex.Th13, "東方神霊廟")
        (GameIdIndex.Th14, "東方輝針城")
        (GameIdIndex.Th15, "東方紺珠伝")
        (GameIdIndex.Th16, "東方天空璋")
        (GameIdIndex.Th17, "東方鬼形獣")
        (GameIdIndex.Th18, "東方虹龍洞")
        (GameIdIndex.Th19, "東方獣王園")
        ]

    let getLegacyGameNameFromId (gameId: string) =
        match legacyGameNamesIndex |> Map.tryFind gameId with
            | Some name -> name
            | None -> ""

    let getWinGameNameFromId (gameId: string) =
        match winGameNamesIndex |> Map.tryFind gameId with
            | Some name -> name
            | None -> ""

    let GetGameNameFromId (gameId: string) =
        let gameidOption = Option.ofObj gameId
        match gameidOption with
            | Some id -> 
                if id |> GameIdIndex.isLegacy = false then
                    getWinGameNameFromId id
                else
                    getLegacyGameNameFromId id
            | None -> ""
    