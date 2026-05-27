namespace ThGameMgr.Ex.ReplayFileBackup

open System.IO
open System.IO.Compression
open System.Xml.Serialization

module ReplayFileBackup =
    let internal makeReplayFileBackupInfoFile (replayBackupInfo: ReplayFileBackupInfo) (baseDirectory: string) =
        let serializer = XmlSerializer(typeof<ReplayFileBackupInfo>)
        let replayBackupFilePath = Path.Combine(baseDirectory, "ReplayFileBackupInfo.xml")
        let stream = new FileStream(replayBackupFilePath, FileMode.Create)
        serializer.Serialize(stream, replayBackupInfo)
        stream.Dispose()
