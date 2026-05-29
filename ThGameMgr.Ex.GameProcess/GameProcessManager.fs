namespace ThGameMgr.Ex.GameProcess

open System
open System.Diagnostics
open System.IO
open System.Threading
open ThGameMgr.Ex.Core
open ThGameMgr.Ex.GameProcess.Exceptions

module GameProcessManager =

    let private startGameProcessExecute (gameId: string) (gameExecutableFilePath: string) =
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
                            $"Failed to confirm that {gameId}: {GameNameIndex.GetGameNameFromId(gameId)} has started."))
                | None -> 
                    raise (InvalidOperationException(
                    $"Failed to find the installation directory of {gameId}: {GameNameIndex.GetGameNameFromId(gameId)}."))
        else
            raise (FileNotFoundException(
            $"Failed to find the executable file of {gameId}: {GameNameIndex.GetGameNameFromId(gameId)}."))

    let private startGameProcessWithApplyingToolExecute (gameId: string) (gameExecutableFilePath: string) (toolFileName: string) =
        if File.Exists(gameExecutableFilePath) = true then
            let gameDirectoryOption : string option = Path.GetDirectoryName(gameExecutableFilePath) |> Option.ofObj
            match gameDirectoryOption with
                | Some dir ->
                    let toolPath = Path.Combine(dir, toolFileName)
                    if File.Exists (toolPath) = false then raise (FileNotFoundException($"Failed to find '{toolFileName}'."))

                    let processStartInfo = ProcessStartInfo()
                    processStartInfo.FileName <- toolPath
                    processStartInfo.WorkingDirectory <- dir
                    processStartInfo.UseShellExecute <- true

                    Process.Start(processStartInfo) |> ignore
                    let processName = Path.GetFileNameWithoutExtension(gameExecutableFilePath)

                    let mutable i = 0
                    while Process.GetProcessesByName(processName).Length = 0 do
                        if i = 50 then 
                            raise (ProcessNotFoundException(
                            $"Failed to find the process of {gameId}: {GameNameIndex.GetGameNameFromId(gameId)}"))
                        Thread.Sleep(100)
                        i <- i + 1
                    
                    Process.GetProcessesByName(processName)[0]
                | None -> 
                    raise (InvalidOperationException(
                    $"Failed to find the installation directory of {gameId}: {GameNameIndex.GetGameNameFromId(gameId)}."))
        else
            raise (FileNotFoundException(
            $"Failed to find the executable file of {gameId}: {GameNameIndex.GetGameNameFromId(gameId)}."))

    let private startCustomProgramExecute (gameId: string) (gameExecutableFilePath: string) =
        if File.Exists(gameExecutableFilePath) = true then
            let gameDirectoryOption : string option = Path.GetDirectoryName(gameExecutableFilePath) |> Option.ofObj
            match gameDirectoryOption with
                | Some dir ->
                    let customProgramFilepath = Path.Combine(dir, "custom.exe")
                    if File.Exists(customProgramFilepath) = false then raise (FileNotFoundException("Failed to find 'custom.exe'."))

                    let processStartInfo = ProcessStartInfo()
                    processStartInfo.FileName <- customProgramFilepath
                    processStartInfo.WorkingDirectory <- dir
                    processStartInfo.UseShellExecute <- true

                    Process.Start(processStartInfo) |> ignore
                | None -> 
                    raise (InvalidOperationException(
                    $"Failed to find the installation directory of {gameId}: {GameNameIndex.GetGameNameFromId(gameId)}."))
        else
            raise (FileNotFoundException(
            $"Failed to find the executable file of {gameId}: {GameNameIndex.GetGameNameFromId(gameId)}."))

    let StartGameProcess (gameId: string) (gameExecutableFilePath: string) =
        let gameIdOption = Option.ofObj gameId
        let gameExecutableFilePathOption = Option.ofObj gameExecutableFilePath

        match (gameIdOption, gameExecutableFilePathOption) with
            | (Some id, Some path) -> startGameProcessExecute id path
            | (None, _) -> nullArg "gameId"
            | (_, None) -> nullArg "gameExecutableFilePath"

    let StartGameProcessWithApplyingTool (gameId: string) (gameExecutableFilePath: string) (toolFileName: string) =
        let gameIdOption = Option.ofObj gameId
        let gameExecutableFilePathOption = Option.ofObj gameExecutableFilePath
        let toolFileNameOption = Option.ofObj toolFileName

        match (gameIdOption, gameExecutableFilePathOption, toolFileNameOption) with
            | (Some id, Some path, Some toolname) -> startGameProcessWithApplyingToolExecute id path toolname
            | (None, _, _) -> nullArg "gameId"
            | (_, None, _) -> nullArg "gameExecutableFilePathOption"
            | (_, _, None) -> nullArg "toolFileName"

    let StartCustomProgram (gameId: string) (gameExecutableFilePath) =
        let gameIdOption = Option.ofObj gameId
        let gameExecutableFilePathOption = Option.ofObj gameExecutableFilePath

        match (gameIdOption, gameExecutableFilePathOption) with
            | (Some id, Some path) -> startCustomProgramExecute id path
            | (None, _) -> nullArg "gameId"
            | (_, None) -> nullArg "gameExecutableFilePath"
