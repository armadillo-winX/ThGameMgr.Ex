namespace ThGameMgr.Ex.GameProcess.Exceptions

type ProcessNotFoundException(message: string) =
    inherit System.Exception(message)
