open System
open System.IO
open System.Xml

let createUserIndex(thGameMgrDir: string) =
    let userIndexFilePath = $"{thGameMgrDir}\\UsersIndex.xml"
    if File.Exists(userIndexFilePath) = false then
        let userIndexDocument = new XmlDocument()
        let documentNode = userIndexDocument.CreateXmlDeclaration("1.0", "UTF-8", null)
        userIndexDocument.AppendChild(documentNode) |> ignore
        let rootNode = userIndexDocument.CreateElement("UserIndex")
        userIndexDocument.AppendChild(rootNode) |> ignore
        userIndexDocument.Save(userIndexFilePath)
        0
    else 
        1

let userExist(userName: string, userIndexFilePath: string) =
    if File.Exists(userIndexFilePath) then
        let userIndexDocument = new XmlDocument()
        userIndexDocument.Load(userIndexFilePath)
        let node = userIndexDocument.DocumentElement.SelectSingleNode($"//User[@Index='{userName}']")

        if node = null then false else true
    else
        false

let addUser(userName: string, userIndexFilePath: string, usersDirectory: string) =
    if Directory.Exists(usersDirectory) = false then 
        Directory.CreateDirectory(usersDirectory) |> ignore

    if File.Exists(userIndexFilePath) = false then createUserIndex |> ignore
    
    let mutable i = 0
    let mutable newUserDirectoryName = $"user{i}"
    while Directory.Exists($"{usersDirectory}\\{newUserDirectoryName}") do
        i <- i + 1
        newUserDirectoryName <- $"user{i}"

    Directory.CreateDirectory($"{usersDirectory}\\{newUserDirectoryName}") |> ignore
    Directory.CreateDirectory($"{usersDirectory}\\{newUserDirectoryName}\\Settings") |> ignore

    let userIndexDocument = new XmlDocument()
    userIndexDocument.Load(userIndexFilePath)
    let rootNode = userIndexDocument.DocumentElement // ルートノード取得
    let userElement = userIndexDocument.CreateElement("User") 
    let index = userIndexDocument.CreateAttribute("Index")
    index.Value <- userName
    userElement.Attributes.Append(index) |> ignore
    rootNode.AppendChild(userElement) |> ignore

    let nameElement = userIndexDocument.CreateElement("Name")
    userIndexDocument.CreateTextNode(userName) |> nameElement.AppendChild |> ignore
    userElement.AppendChild(nameElement) |> ignore
    
    let pathElement = userIndexDocument.CreateElement("DirectoryName")
    userIndexDocument.CreateTextNode(newUserDirectoryName) |> pathElement.AppendChild |> ignore
    userElement.AppendChild(pathElement) |> ignore

    userIndexDocument.Save(userIndexFilePath)
    
    0

let deleteUser(userName: string, userIndexFilePath: string, usersDirectory: string) =
    if File.Exists(userIndexFilePath) then
        let userIndexDocument = new XmlDocument()
        userIndexDocument.Load(userIndexFilePath)

        let userDirectoryName = userIndexDocument.SelectSingleNode($"//User[@Index='{userName}']/DirectoryName").InnerText

        let rootNode = userIndexDocument.DocumentElement
        let userNode = userIndexDocument.SelectSingleNode($"//User[@Index='{userName}']")
        rootNode.RemoveChild(userNode) |> ignore
        userIndexDocument.Save(userIndexFilePath)

        0
    else
        1

let getUsersList(usersIndexFilePath: string, usersDirectory: string) =
    let users = new ResizeArray<string>()
    if File.Exists(usersIndexFilePath) then
        let userIndexDocument = new XmlDocument()
        userIndexDocument.Load(usersIndexFilePath)
        let userNodeList = userIndexDocument.SelectNodes("//User")
        if userNodeList <> null && userNodeList.Count > 0 then
            let mutable i = 0
            while i < userNodeList.Count do
                let userNode = userNodeList.Item(i)
                let userName = userNode.SelectSingleNode("Name").InnerText
                users.Add(userName)
                i <- i + 1
            
            users
        else
            users
    else
        users

//スクリプトのある場所を取得する
let scriptDirectory = __SOURCE_DIRECTORY__

printfn "東方管制塔 EX ユーザープロファイルユーティリティ"
printfn "by 珠音茉白/東方管制塔開発部"
printfn "================================================="
printfn "スクリプトのある場所: %s \n" scriptDirectory

let userIndexFilePath = $"{scriptDirectory}\\UsersIndex.xml"
let usersDirectory = $"{scriptDirectory}\\Users"

if File.Exists($"{scriptDirectory}\\ThGameMgr.Ex.exe") then
    while true do
        Console.WriteLine("操作を選択して数字を入力")
        Console.WriteLine("[0] 新しいユーザーを追加する")
        Console.WriteLine("[1] 複数の新しいユーザーを追加する")
        Console.WriteLine("[2] ユーザーを削除する")
        Console.WriteLine("[3] ユーザーのリストを取得する")
        Console.WriteLine("[9] スクリプトを終了する")

        let input = Console.ReadLine()
        if input = "0" then
            Console.WriteLine("新規追加するユーザー名を入力してください:")
            let userName = Console.ReadLine()
            if userExist(userName, userIndexFilePath) then
                printfn "ユーザー '%s' は既に存在します．\n" userName
            else
                addUser(userName, userIndexFilePath, usersDirectory) |> ignore
                printfn "ユーザーを追加しました．\n"
        elif input = "1" then
            Console.WriteLine("追加したいユーザー名のベース名を入力してください:")
            let userBaseName = Console.ReadLine()
            Console.WriteLine("追加する数を入力してください:")
            let input = Console.ReadLine()
            let parseResult, count = Int32.TryParse(input)
            if parseResult then
                let mutable j: int32 = 0
                let mutable k: int32 = 0
                while k < count do
                    let userName = $"{userBaseName}_{j}"
                    if userExist(userName, userIndexFilePath) then
                        printfn "'%s' は既に存在するのでスキップされました．" userName
                    else
                        addUser(userName, userIndexFilePath, usersDirectory) |> ignore
                        printfn "ユーザーを追加しました: %s" userName
                        k <- k + 1

                    j <- j + 1
                printfn ""
            else
                printfn "入力が不正です．入力を数字にできませんでした．\n"
        elif input = "2" then
            Console.WriteLine("削除するユーザー名を入力してください:")
            let userName = Console.ReadLine()
            if userExist(userName, userIndexFilePath) then
                let result = deleteUser(userName, userIndexFilePath, usersDirectory)
                if result = 0 then
                    printfn "ユーザーを削除しました．\n"
                else
                    printfn "UsersIndex.xml が存在しなかったため，ユーザーを削除できませんでした．\n"
            else
                printfn "ユーザー '%s' は存在しません．\n" userName
        elif input = "3" then
            let users = getUsersList(userIndexFilePath, usersDirectory)
            if users <> null &&  users.Count > 0 then
                printfn "ユーザーのリスト:"
                for user in users do
                    printfn "- %s" user
                printfn ""
            else
                printfn "ユーザーが存在しません．\n"
        elif input = "9" then
            exit 0
        else 
            printfn "不正な入力です．\n"
else
    printfn "東方管制塔 EX が見つかりません．\nこのスクリプトは，東方管制塔 EX (ThGameMgr.Ex.exe)と同じディレクトリに配置して使わなければいけません．"
    printfn "何かキーを入力すると終了..."
    Console.ReadKey() |> ignore
    exit 1
