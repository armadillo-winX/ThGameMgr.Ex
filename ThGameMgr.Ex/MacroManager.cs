using Masicalan.VaultVfs;
using System.Security.Cryptography;

namespace ThGameMgr.Ex
{
    internal class MacroManager
    {
        private IUserService _userService;
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

        internal string CreateVfs()
        {
            string vfsArchiveFilePath = _userService.GetCurrentUserMacroVaultArchiveFilePath();
            string entropyName = GetVfsEntropyName();
            return VfsManager.Create(vfsArchiveFilePath, entropyName);
        }
    }
}
