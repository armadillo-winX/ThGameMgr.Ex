namespace ThGameMgr.Ex.Core

open System.Collections.Generic

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

    let private getLegacyGameNameFromId (gameId: string) =
        match legacyGameNamesIndex |> Map.tryFind gameId with
            | Some name -> name
            | None -> ""

    let private getWinGameNameFromId (gameId: string) =
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

    let private getLegacyGameIdFromName (gameName: string) =
        let gameIdOption =
            legacyGameNamesIndex
            |> Map.tryFindKey (fun key value -> value = gameName)
        match gameIdOption with
            | Some id -> id
            | None -> ""

    let private getWinGameIdFromName (gameName: string) =
        let gameIdOption =
            winGameNamesIndex
            |> Map.tryFindKey (fun key value -> value = gameName)
        match gameIdOption with
            | Some id -> id
            | None -> ""

    let GetLegacyGameIdFromName (gameName: string) =
        let gameNameOption = Option.ofObj gameName
        match gameNameOption with
            | Some name -> getLegacyGameIdFromName name
            | None -> ""

    let GetWinGameIdFromName (gameName: string) =
        let gameNameOption = Option.ofObj gameName
        match gameNameOption with
            | Some name -> getWinGameIdFromName name
            | None -> ""

    let GetAllLegacyGamesList () =
        List<string>(legacyGameNamesIndex.Keys)

    let GetAllWinGamesList () =
        List<string>(winGameNamesIndex.Keys)
    