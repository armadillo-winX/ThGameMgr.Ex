using Masicalan.Core;
using Microsoft.FSharp.Collections;
using System.Collections.Generic;
using System.Windows.Controls;

namespace ThGameMgr.Ex
{
    internal class MacroExtensionProvider
    {

        private readonly IUserService _userService;

        internal MacroExtensionProvider(IUserService userService)
        {
            _userService = userService;
        }

        // writeLine(s)
        private void WriteLineExec(TextBox? outputTextBox, string s)
        {
            if (outputTextBox != null)
            {
                string formated = s + Environment.NewLine;

                outputTextBox.Dispatcher.Invoke(new Action(() =>
                {
                    outputTextBox.AppendText(formated);
                    outputTextBox.ScrollToEnd();
                }));
            }
            else
            {
                Console.WriteLine(s);
            }
        }

        // copyFile(sourceFile, destFile)
        private void CopyFileExec(List<string> accessableDirectories, string sourceFile, string destFile)
        {
            string? sourceDir = Path.GetDirectoryName(sourceFile);
            string? destDir = Path.GetDirectoryName(destFile);
            if (!string.IsNullOrWhiteSpace(sourceDir) && !string.IsNullOrWhiteSpace(destDir) &&
                accessableDirectories.Contains(sourceDir) &&
                accessableDirectories.Contains(destDir))
            {
                File.Copy(sourceFile, destFile);
            }
            else if (!string.IsNullOrWhiteSpace(sourceDir) && !string.IsNullOrWhiteSpace(destDir) &&
                !accessableDirectories.Contains(sourceDir))
            {
                throw new InvalidOperationException(
                    $"Access to the {sourceDir} is not permitted.");
            }
            else if (!string.IsNullOrWhiteSpace(sourceDir) && !string.IsNullOrWhiteSpace(destDir) &&
                !accessableDirectories.Contains(destDir))
            {
                throw new InvalidOperationException(
                    $"Access to the {destDir} is not permitted.");
            }
            else
            {
                throw new InvalidOperationException("copyFile: Invalid arguments");
            }
        }

        // moveFile(sourceFile, destFile)
        private void MoveFileExec(List<string> accessableDirectories, string sourceFile, string destFile)
        {
            string? sourceDir = Path.GetDirectoryName(sourceFile);
            string? destDir = Path.GetDirectoryName(destFile);
            if (!string.IsNullOrWhiteSpace(sourceDir) && !string.IsNullOrWhiteSpace(destDir) &&
                accessableDirectories.Contains(sourceDir) &&
                accessableDirectories.Contains(destDir))
            {
                File.Move(sourceFile, destFile);
            }
            else if (!string.IsNullOrWhiteSpace(sourceDir) && !string.IsNullOrWhiteSpace(destDir) &&
                !accessableDirectories.Contains(sourceDir))
            {
                throw new InvalidOperationException(
                    $"Access to the {sourceDir} is not permitted.");
            }
            else if (!string.IsNullOrWhiteSpace(sourceDir) && !string.IsNullOrWhiteSpace(destDir) &&
                !accessableDirectories.Contains(destDir))
            {
                throw new InvalidOperationException(
                    $"Access to the {destDir} is not permitted.");
            }
            else
            {
                throw new InvalidOperationException("moveFile: Invalid arguments");
            }
        }

        // writeTo(filePath, content)
        private void WriteToExec(List<string> accessableDirectories, string filePath, string content)
        {
            string? directory = Path.GetDirectoryName(filePath);
            if (!string.IsNullOrWhiteSpace(directory) &&
                accessableDirectories.Contains(directory))
            {
                File.WriteAllText(filePath, content);
            }
            else if (!string.IsNullOrWhiteSpace(directory) &&
                !accessableDirectories.Contains(directory))
            {
                throw new InvalidOperationException($"Access to the {directory} is not permitted.");
            }
            else
            {
                throw new InvalidOperationException("writeTo: Invalid arguments");
            }
        }

        // readFrom(filePath)
        private string ReadFromExec(List<string> accessableDirectories, string filePath)
        {
            string? directory = Path.GetDirectoryName(filePath);
            if (!string.IsNullOrWhiteSpace(directory) &&
                accessableDirectories.Contains(directory))
            {
                return File.ReadAllText(filePath);
            }
            else if (!string.IsNullOrWhiteSpace(directory) &&
                !accessableDirectories.Contains(directory))
            {
                throw new InvalidOperationException($"Access to the {directory} is not permitted.");
            }
            else
            {
                throw new InvalidOperationException("readFrom: Invalid arguments");
            }
        }

        // fileExists(filePath)
        private bool FileExistsExec(string filePath)
        {
            return File.Exists(filePath);
        }

        // getDateTimeNow(format)
        private string GetDateTimeNowExec(string format)
        {
            return DateTime.Now.ToString(format);
        }

        // getUtcDateTimeNow(format)
        private string GetUtcDateTimeNowExec(string format)
        {
            return DateTime.UtcNow.ToString(format);
        }

        // showInputBox(title)
        private string ShowInputBoxExec(string title)
        {
            InputDialog inputDialog = new(title);
            if (inputDialog.ShowDialog() == true)
            {
                return inputDialog.InputText;
            }
            else
            {
                return string.Empty;
            }
        }

        // showMessageBox(message, title)
        private void ShowMessageBoxExec(string message, string title)
        {
            MessageBox.Show(
                message, title, MessageBoxButton.OK, MessageBoxImage.Information);
        }

        // showErrorBox(message title)
        private void ShowErrorBoxExec(string message, string title)
        {
            MessageBox.Show(
                message, title, MessageBoxButton.OK, MessageBoxImage.Error);
        }

        public FSharpMap<string, Tuple<FSharpList<string>, Statement>> CreateFunctionExtension(
            TextBox? outputTextBox)
        {
            var macroManager = new MacroManager(_userService);
            List<string> accessableDirectories = macroManager.GetMacroIOAccessConfig();

            var builder = new ExtensionBuilder.FunctionEnvironmentBuilder();

            Action<string> writeLineFunc = (s) => WriteLineExec(outputTextBox, s);
            Action<string, string> copyFileFunc = (source, dest) => CopyFileExec(accessableDirectories, source, dest);
            Action<string, string> moveFileFunc = (source, dest) => MoveFileExec(accessableDirectories, source, dest);
            Action<string, string> writeToFunc = (path, content) => WriteToExec(accessableDirectories, path, content);
            Func<string, string> readFromFunc = (path) => ReadFromExec(accessableDirectories, path);
            Func<string, bool> fileExistsFunc = (path) => FileExistsExec(path);
            Func<string, string> getDateTimeNowFunc = (format) => GetDateTimeNowExec(format);
            Func<string, string> getUtcDateTimeNowFunc = (format) => GetUtcDateTimeNowExec(format);
            Func<string, string> showInputBoxFunc = (title) => ShowInputBoxExec(title);
            Action<string, string> showMessageBoxFunc = (message, title) => ShowMessageBoxExec(message, title);
            Action<string, string> showErrorBoxFunc = (message, title) => ShowErrorBoxExec(message, title);

            builder.Register("writeLine", ["s"], writeLineFunc);
            builder.Register("copyFile", ["sourceFile", "destFile"], copyFileFunc);
            builder.Register("moveFile", ["sourceFile", "destFile"], moveFileFunc);
            builder.Register("writeTo", ["filePath", "content"], writeToFunc);
            builder.Register("readFrom", ["filePath"], readFromFunc);
            builder.Register("fileExists", ["filePath"], fileExistsFunc);
            builder.Register("getDateTimeNow", ["format"], getDateTimeNowFunc);
            builder.Register("getUtcDateTimeNow", ["format"], getUtcDateTimeNowFunc);
            builder.Register("showInputBox", ["title"], showInputBoxFunc);
            builder.Register("showMessageBox", ["message", "title"], showMessageBoxFunc);
            builder.Register("showErrorBox", ["message", "title"], showErrorBoxFunc);

            return builder.Build();
        }
    }
}
