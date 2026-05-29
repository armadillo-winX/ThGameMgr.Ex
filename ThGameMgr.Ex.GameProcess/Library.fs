namespace ThGameMgr.Ex.GameProcess

open System
open System.Diagnostics
open System.IO
open ThGameMgr.Ex.Core

module GameProcessManager =

    let internal startGameProcessExecute (gameId: string) (gameExecutableFilePath: string) =
        if File.Exists(gameExecutableFilePath) = true then
            let gameDirectory : string option = Path.GetDirectoryName(gameExecutableFilePath) |> Option.ofObj
            match gameDirectory with
                | Some dir ->
                    let processStartInfo = ProcessStartInfo()
                    processStartInfo.FileName <- gameExecutableFilePath
                    processStartInfo.WorkingDirectory <- dir
                    processStartInfo.UseShellExecute <- true
                    Process.Start(processStartInfo)
                |None -> 
                    raise (InvalidOperationException($"Cannot find the installation directory of {GameNameIndex.GetGameNameFromId(gameId)}"))
        else
            raise (FileNotFoundException(
            $"The executable file of {GameNameIndex.GetGameNameFromId(gameId)} does not found."))

    let StartGameProcess (gameId: string) (gameExecutableFilePath: string) =
        let gameIdOption = Option.ofObj gameId
        let gameExecutableFilePathOption = Option.ofObj gameExecutableFilePath

        match (gameIdOption, gameExecutableFilePathOption) with
            | (Some id, Some path) -> startGameProcessExecute id path
            | (None, _) -> nullArg "gameId"
            | (_, None) -> nullArg "gameExecutableFilePath"
