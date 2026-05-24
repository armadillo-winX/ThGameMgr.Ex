namespace ThGameMgr.Ex
{
    internal class UserService : IUserService
    {
        public string CurrentUserName { get; set; }

        public string CurrentUserDirectoryName { get; set; }

        public UserService()
        {
            this.CurrentUserName = string.Empty;
            this.CurrentUserDirectoryName = string.Empty;
        }

        public string GetCurrentUserSettingsDirectory()
        {
            return Path.Combine(PathInfo.AppLocation, this.CurrentUserDirectoryName, "Settings");
        }

        public string GetCurrentUserScoreBackupDirectoy()
        {
            return Path.Combine(PathInfo.AppLocation, this.CurrentUserDirectoryName, "backup");
        }

        public string GetCurrentUserGamePlayLogRecordFilePath()
        {
            return Path.Combine(PathInfo.AppLocation, this.CurrentUserDirectoryName, "GamePlayLog.xml");
        }
    }
}
