namespace ThGameMgr.Ex.GameProcess

open System
open System.Diagnostics
open System.IO
open ThGameMgr.Ex.Core
open ThGameMgr.Ex.GameProcess.Exceptions

module GameProcessManager =

    let internal startGameProcessExecute (gameId: string) (gameExecutableFilePath: string) =
        if File.Exists(gameExecutableFilePath) = true then
            let gameDirectoryOption : string option = Path.GetDirectoryName(gameExecutableFilePath) |> Option.ofObj
            match gameDirectoryOption with
                | Some dir ->
                    let processStartInfo = ProcessStartInfo()
                    processStartInfo.FileName <- gameExecutableFilePath
                    processStartInfo.WorkingDirectory <- dir
                    processStartInfo.UseShellExecute <- true

                    let gameProcessOption = Process.Start(processStartInfo) |> Option.ofObj
                    match gameProcessOption with
                        | Some p -> 
                            p.WaitForInputIdle() |> ignore
                            p
                        | None -> 
                            raise (ProcessNotFoundException(
                            $"Cannnot confirm that {gameId}: {GameNameIndex.GetGameNameFromId(gameId)} has started."))
                |None -> 
                    raise (InvalidOperationException(
                    $"Cannot find the installation directory of {gameId}: {GameNameIndex.GetGameNameFromId(gameId)}"))
        else
            raise (FileNotFoundException(
            $"The executable file of {gameId}: {GameNameIndex.GetGameNameFromId(gameId)} does not found."))

    let StartGameProcess (gameId: string) (gameExecutableFilePath: string) =
        let gameIdOption = Option.ofObj gameId
        let gameExecutableFilePathOption = Option.ofObj gameExecutableFilePath

        match (gameIdOption, gameExecutableFilePathOption) with
            | (Some id, Some path) -> startGameProcessExecute id path
            | (None, _) -> nullArg "gameId"
            | (_, None) -> nullArg "gameExecutableFilePath"
