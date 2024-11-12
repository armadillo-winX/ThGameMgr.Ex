using System.Threading;

namespace ThGameMgr.Ex.Game
{
    internal class GameProcessHandler
    {
        public static Process StartGameProcess(string gameId)
        {
            string? gamePath = GameFile.GetGameFilePath(gameId);
            if (File.Exists(gamePath))
            {
                string gameDirectory = Path.GetDirectoryName(gamePath);

                ProcessStartInfo gameProcessStartInfo = new()
                {
                    FileName = gamePath,
                    WorkingDirectory = gameDirectory,
                    UseShellExecute = true
                };

                Process gameProcess = Process.Start(gameProcessStartInfo);

                if (gameProcess != null)
                {
                    gameProcess.WaitForInputIdle();

                    return gameProcess;
                }
                else
                {
                    throw new ProcessNotFoundException("ゲームプロセスの起動が確認できませんでした。");
                }
            }
            else
            {
                throw new FileNotFoundException("ゲーム実行ファイルが見つかりませんでした。");
            }
        }

        public static Process StartGameProcessWithApplyingTool(string gameId, string toolName)
        {
            string? gamePath = GameFile.GetGameFilePath(gameId);
            string gameDirectory = Path.GetDirectoryName(gamePath);
            string patchPath = $"{gameDirectory}\\{toolName}";
            if (File.Exists(gamePath) && File.Exists(patchPath))
            {
                ProcessStartInfo gameProcessStartInfo = new()
                {
                    FileName = patchPath,
                    WorkingDirectory = gameDirectory,
                    UseShellExecute = true
                };

                _ = Process.Start(gameProcessStartInfo);


                int i = 0;
                while (Process.GetProcessesByName(Path.GetFileNameWithoutExtension(gamePath)).Length == 0)
                {
                    Thread.Sleep(100);
                    if (i == 50)
                    {
                        throw new ProcessNotFoundException("ゲームプロセスの検出に失敗しました。");
                    }
                    i++;
                }

                Process gameProcess = Process.GetProcessesByName(Path.GetFileNameWithoutExtension(gamePath))[0];

                gameProcess.WaitForInputIdle();

                return gameProcess;
            }
            else if (!File.Exists(patchPath))
            {
                throw new FileNotFoundException($"'{toolName}' が見つかりませんでした。");
            }
            else
            {
                throw new FileNotFoundException("ゲーム実行ファイルが見つかりませんでした。");
            }
        }

        public static void StartCustomProgramProcess(string gameId)
        {
            string? gamePath = GameFile.GetGameFilePath(gameId);
            string gameDirectory = Path.GetDirectoryName(gamePath);
            string customProgramPath = $"{gameDirectory}\\custom.exe";
            if (File.Exists(gamePath) && File.Exists(customProgramPath))
            {
                ProcessStartInfo customProgramStartInfo = new()
                {
                    FileName = customProgramPath,
                    WorkingDirectory = gameDirectory
                };

                _ = Process.Start(customProgramStartInfo);
            }
            else if (!File.Exists(customProgramPath))
            {
                throw new FileNotFoundException("'custom.exe' が見つかりませんでした。");
            }
            else
            {
                throw new FileNotFoundException("ゲーム実行ファイルが見つかりませんでした。");
            }
        }
    }
}
