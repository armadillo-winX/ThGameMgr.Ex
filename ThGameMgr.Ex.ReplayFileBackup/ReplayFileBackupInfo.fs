namespace ThGameMgr.Ex.ReplayFileBackup

open System.Xml.Serialization

[<CLIMutable>]
[<XmlRoot("ReplayFileBackupInfo")>]
type ReplayFileBackupInfo = {
    [<XmlElement("GameId")>] GameId: string
    [<XmlElement("GameName")>] GameName: string
    [<XmlElement("SourceReplayFilePath")>] SourceReplayFilePath: string
    [<XmlElement("BackupName")>] BackupName: string
    [<XmlElement("Timestamp")>] Timestamp: string
    [<XmlElement("Comment")>] Comment: string
    [<XmlElement("ApplicationName")>] ApplicationName: string
}
