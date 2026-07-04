using Masicalan.Core;
using Microsoft.FSharp.Collections;

namespace ThGameMgr.Ex.Dialogs
{
    /// <summary>
    /// MacroScriptEditorDialog.xaml の相互作用ロジック
    /// </summary>
    public partial class MacroScriptEditorDialog : Window
    {
        private readonly IUserService _userService;

        private readonly MacroManager _macroManager;

        private string _path;

        private string _script;

        public MacroScriptEditorDialog(
            IUserService userService, string path
            )
        {
            InitializeComponent();

            _userService = userService;
            MacroManager macroManager = new(userService);
            _macroManager = macroManager;
            _path = path;
            string script = macroManager.ReadScript(path);
            _script = script;
            EditorBox.Text = script;

            this.Title = $"{path} - Masicalan マクロスクリプトエディタ";
        }

        private bool Save()
        {
            string editorScript = EditorBox.Text;
            try
            {
                _macroManager.EditScript(editorScript, _path);
                _script = editorScript;
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, $"スクリプトの保存に失敗しました．\n{ex.Message}", "エラー",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }

        private void AboutMasicalanMenuItemClick(object sender, RoutedEventArgs e)
        {
            string coreLibraryAssemPath =
                $"{PathInfo.AssemblyFilePath}\\Masicalan.Core.dll";

            string? name = FileVersionInfo.GetVersionInfo(coreLibraryAssemPath).ProductName;
            string? version = FileVersionInfo.GetVersionInfo(coreLibraryAssemPath).ProductVersion;
            string? copyright = FileVersionInfo.GetVersionInfo(coreLibraryAssemPath).LegalCopyright;

            string info =
                $"{name}\nver.{version}\n{copyright}";

            MessageBox.Show(this, info, "Masicalan バージョン情報", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void CloseMenuItemClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void WindowClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            string editorScript = EditorBox.Text;
            if (editorScript != _script)
            {
                MessageBoxResult result =
                    MessageBox.Show(this, "スクリプトが保存されていません．\n保存して終了しますか?",
                    "エディタの終了",
                    MessageBoxButton.YesNoCancel, MessageBoxImage.Exclamation);

                if (result == MessageBoxResult.Cancel)
                {
                    e.Cancel = true;
                }
                else if (result == MessageBoxResult.OK)
                {
                    bool re = Save();
                    if (!re) { e.Cancel = true; }
                }
            }

            _ = this.Owner.Activate();
        }

        private void SaveScriptMenuItemClick(object sender, RoutedEventArgs e)
        {
            _ = Save();
        }

        private void AccessIOConfigMenuItemClick(object sender, RoutedEventArgs e)
        {
            MacroIOAccessConfigDialog macroIOAccessConfigDialog = new(_userService)
            {
                Owner = this
            };
            _ = macroIOAccessConfigDialog.ShowDialog();
        }

        private void RunMenuItemClick(object sender, RoutedEventArgs e)
        {
            MacroExtensionProvider extensionProvider = new(_userService);
            FSharpMap<string, Value> emptyMap = MapModule.Empty<string, Value>();
            var functionExtension = extensionProvider.CreateFunctionExtension(OutputBox);

            string editorScript = EditorBox.Text;

            try
            {
                HostInterpreter.Run(editorScript, emptyMap, functionExtension);
            }
            catch (Exception ex)
            {
                ErrorBox.Text = $"{ex.Message}";
                System.Media.SystemSounds.Hand.Play();
            }
        }
    }
}
