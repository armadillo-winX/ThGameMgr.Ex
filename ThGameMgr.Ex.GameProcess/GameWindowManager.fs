namespace ThGameMgr.Ex.GameProcess

open System
open ThGameMgr.Ex.Win32API

module GameWindowManager =

    let GetGameWindowSizes (gameWindow: IntPtr) =
        let result, rect = User32.GetWindowRect(gameWindow)
        let width = rect.right - rect.left
        let height = rect.top - rect.bottom

        let gameWindowSizes: GameWindowSizes = {
            Width = width
            Height = height
        }
        gameWindowSizes

    let GetGameWindowPosition (gameWindow: IntPtr) =
        let result, rect = User32.GetWindowRect(gameWindow)

        let gameWindowPosition: GameWindowPosition = {
            X = rect.left
            Y = rect.top
        }
        gameWindowPosition

    let ResizeGameWindow (gameWindow: IntPtr) (width: int) (height: int) =
        let position = GetGameWindowPosition gameWindow
        User32.MoveWindow(gameWindow, position.X, position.Y, width, height, 1) |> ignore
        User32.SetForegroundWindow(gameWindow) |> ignore
