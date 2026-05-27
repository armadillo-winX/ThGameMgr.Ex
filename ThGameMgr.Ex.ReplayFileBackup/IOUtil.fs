namespace ThGameMgr.Ex.ReplayFileBackup

open System
open System.IO
open System.Reflection

module IOUtil =
    let private assemblyName = Assembly.GetExecutingAssembly().GetName().Name

    let internal createTempDirectory () =
        let tempDirectory = Path.GetTempPath()
        let timestamp = DateTime.Now.ToString("yyyy-MM-dd_hh-mm-ss")
        Path.Combine(tempDirectory, $"{assemblyName}_{timestamp}") 
        |> Directory.CreateDirectory 
        |> (fun d -> d.FullName)
