namespace ThGameMgr.Ex.ReplayFileBackup

open System.IO
open System.IO.Compression
open System.Xml.Serialization

module ReplayBackup =

    let private extensionName = "trpb"

    let internal makeReplayFileBackupInfoFile (replayBackupInfo: ReplayFileBackupInfo) (baseDirectory: string) =
        let serializer = XmlSerializer(typeof<ReplayFileBackupInfo>)
        let replayBackupFilePath = Path.Combine(baseDirectory, "ReplayFileBackupInfo.xml")
        let stream = new FileStream(replayBackupFilePath, FileMode.Create)
        serializer.Serialize(stream, replayBackupInfo)
        stream.Dispose()

    let MakeReplayBackupFile (replayBackupFileName: string) (replayBackupInfo: ReplayFileBackupInfo) (outputDirectory: string) =
        let tempDirectory = IOUtil.createTempDirectory()
        let backupTempDirectory = Path.Combine(tempDirectory, replayBackupFileName)
        Directory.CreateDirectory(backupTempDirectory) |> ignore

        if Directory.Exists(outputDirectory) = false then Directory.CreateDirectory(outputDirectory) |> ignore

        Path.Combine(backupTempDirectory, "rpy") 
        |> Directory.CreateDirectory 
        |> ignore

        let replayFilepath = replayBackupInfo.SourceReplayFilePath
        let destReplayFilePath =  Path.Combine(backupTempDirectory, "rpy", Path.GetFileName(replayFilepath))

        File.Copy(replayFilepath, destReplayFilePath)
        makeReplayFileBackupInfoFile replayBackupInfo backupTempDirectory
        let outputFilepath = Path.Combine(outputDirectory, $"{replayBackupFileName}.{extensionName}")
        ZipFile.CreateFromDirectory(backupTempDirectory, outputFilepath)

        try
            Directory.Delete(tempDirectory, true)
        with
            |_-> printfn "Failed to delete temporary folder."

        outputFilepath

    let GetReplayBackupFileInfo (replayBackupFilePath: string) =
        let archive = ZipFile.OpenRead(replayBackupFilePath)
        let infoFileEntry = archive.GetEntry($"ReplayFileBackupInfo.xml")

        let stream = infoFileEntry.Open()
        let serializer = new XmlSerializer(typeof<ReplayFileBackupInfo>)

        serializer.Deserialize(stream) 
        :?> ReplayFileBackupInfo

    let ExtractBackupFile (replayBackupFilePath: string) (outputFilePath: string) =
        let replayBackupInfo = GetReplayBackupFileInfo replayBackupFilePath

        let archive  = ZipFile.OpenRead(replayBackupFilePath)
        let replayFileEntry = 
            $"rpy/{Path.GetFileName(replayBackupInfo.SourceReplayFilePath)}" 
            |> archive.GetEntry

        replayFileEntry.ExtractToFile(outputFilePath, true)

        outputFilePath
