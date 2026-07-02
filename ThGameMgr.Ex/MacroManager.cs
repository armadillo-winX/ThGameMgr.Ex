using Masicalan.VaultVfs;
using System.Security.Cryptography;
using System.Collections.Generic;
using System.Text.Json;

namespace ThGameMgr.Ex
{
    internal class MacroManager
    {
        private readonly IUserService _userService;
        public MacroManager(IUserService userService)
        {
            _userService = userService;
        }

        internal string GenerateVfsEntropyName()
        {
            string guid = Guid.NewGuid().ToString("N");
            string entropyName = $"ThGameMgr.Ex.{guid}";

            return entropyName;
        }

        internal void SaveVfsEntropy(string entropyName)
        {
            string configFilePath = _userService.GetCurrentUserMacroArchiveEntropyConfigPath();
            byte[] entropy = Encoding.UTF8.GetBytes(entropyName);
            byte[] encrypted = ProtectedData.Protect(entropy, null, DataProtectionScope.CurrentUser);
            File.WriteAllBytes(configFilePath, encrypted);
        }

        internal string GetVfsEntropyName()
        {
            string configFilePath = _userService.GetCurrentUserMacroArchiveEntropyConfigPath();
            if (File.Exists(configFilePath))
            {
                byte[] encrypted = File.ReadAllBytes(configFilePath);
                byte[] entropy = ProtectedData.Unprotect(encrypted, null, DataProtectionScope.CurrentUser);
                return Encoding.UTF8.GetString(entropy);
            }
            else
            {
                string entropyName = GenerateVfsEntropyName();
                SaveVfsEntropy(entropyName);
                return entropyName;
            }
        }

        internal void SaveMacroIOAccessConfig(List<string> accessableDirectories)
        {
            string configFile = _userService.GetCurrentUserMacroIoAccessConfigPath();

            string json = JsonSerializer.Serialize(accessableDirectories);
            byte[] data = Encoding.UTF8.GetBytes(json);
            byte[] encrypted = ProtectedData.Protect(data, null, DataProtectionScope.CurrentUser);
            File.WriteAllBytes(configFile, encrypted);
        }

        internal List<string> GetMacroIOAccessConfig()
        {
            string configFile = _userService.GetCurrentUserMacroIoAccessConfigPath();

            byte[] encrypted = File.ReadAllBytes(configFile);
            byte[] data = ProtectedData.Unprotect(encrypted, null, DataProtectionScope.CurrentUser);
            string json = Encoding.UTF8.GetString(data);

            List<string>? dirs = JsonSerializer.Deserialize<List<string>>(json);
            if (dirs != null)
            {
                return dirs;
            }
            else
            {
                return [];
            }
        }

        internal string CreateVfs()
        {
            string vfsArchiveFilePath = _userService.GetCurrentUserMacroVaultArchiveFilePath();
            string entropyName = GetVfsEntropyName();
            return VfsManager.Create(vfsArchiveFilePath, entropyName);
        }

        internal void AddScript(string script, string directory, string fileName)
        {
            string vfsFilePath = _userService.GetCurrentUserMacroVaultArchiveFilePath();
            if (!File.Exists(vfsFilePath)) { _= CreateVfs(); }

            string entropyName = GetVfsEntropyName();
            _ = VfsIO.Add(vfsFilePath, entropyName, directory, fileName, script, VfsAttribute.Executable);
        }

        internal string ReadScript(string path)
        {
            string vfsFilePath = _userService.GetCurrentUserMacroVaultArchiveFilePath();
            string entropyName = GetVfsEntropyName();
            VfsFileData vfd = VfsIO.Read(vfsFilePath, entropyName, path);
            return vfd.Script;
        }

        internal void EditScript(string script, string path)
        {
            string vfsFilePath = _userService.GetCurrentUserMacroVaultArchiveFilePath();
            string entropyName = GetVfsEntropyName();
            _ = VfsIO.Edit(vfsFilePath, entropyName, path, script);
        }

        internal void DeleteScript(string path)
        {
            string vfsFilePath = _userService.GetCurrentUserMacroVaultArchiveFilePath();
            string entropyName = GetVfsEntropyName();
            _ = VfsIO.Delete(vfsFilePath, entropyName, path);
        }
    }
}
