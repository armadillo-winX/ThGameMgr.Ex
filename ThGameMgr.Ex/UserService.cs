namespace ThGameMgr.Ex
{
    public class UserService : IUserService
    {
        public string CurrentUserName { get; set; }

        public string CurrentUserDirectoryName { get; set; }

        public UserService()
        {
            this.CurrentUserName = string.Empty;
            this.CurrentUserDirectoryName = string.Empty;
        }

        public void SwitchUser(string userName)
        {
            if (string.IsNullOrWhiteSpace(userName))
                throw new ArgumentException("ユーザー名が不正です。");
            if (!UserConfigurator.Exists(userName))
                throw new UserNotFoundException(userName, $"ユーザー '{userName}' は存在しません。");

            string userDirectoryName = UserConfigurator.GetUserDirectoryName(userName);
            this.CurrentUserName = userName;
            this.CurrentUserDirectoryName = userDirectoryName;
        }

        public string GetCurrentUserSettingsDirectory()
        {
            return Path.Combine(PathInfo.UsersDirectory, this.CurrentUserDirectoryName, "Settings");
        }

        public string GetCurrentUserScoreBackupDirectoy()
        {
            return Path.Combine(PathInfo.UsersDirectory, this.CurrentUserDirectoryName, "backup");
        }

        public string GetCurrentUserGamePlayLogRecordFilePath()
        {
            return Path.Combine(PathInfo.UsersDirectory, this.CurrentUserDirectoryName, "GamePlayLog.xml");
        }
    }
}
