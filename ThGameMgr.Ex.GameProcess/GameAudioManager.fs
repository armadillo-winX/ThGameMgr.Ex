namespace ThGameMgr.Ex.GameProcess

open NAudio.CoreAudioApi
open System.Diagnostics

module GameAudioManager =
    
    let rec private getGameAudioVolumeFromCollection (gameProcess: Process) (index: int) (sessionCollection: SessionCollection) =
        if index < sessionCollection.Count then
            let session = sessionCollection[index]
            match session with
            | s when int s.GetProcessID = gameProcess.Id ->
                float s.SimpleAudioVolume.Volume
            |_-> getGameAudioVolumeFromCollection gameProcess (index+1) sessionCollection
        else
            0.0

    let private getGameAudioVolumeExecute (gameProcess: Process) =
        let enumerator = new MMDeviceEnumerator()
        let defaultAudioDevice = enumerator.GetDefaultAudioEndpoint(DataFlow.Render, Role.Console)
        let audioSessionManager = defaultAudioDevice.AudioSessionManager
        let sessionCollection = audioSessionManager.Sessions
        getGameAudioVolumeFromCollection gameProcess 0 sessionCollection