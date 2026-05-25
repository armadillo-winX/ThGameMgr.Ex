namespace ThGameMgr.Ex
{
    internal interface IUserService
    {
        string CurrentUserName { get; set; }

        string CurrentUserDirectoryName { get; set; }

        public void SwitchUser(string userName);

        string GetCurrentUserSettingsDirectory();

        string GetCurrentUserScoreBackupDirectoy();

        string GetCurrentUserGamePlayLogRecordFilePath();
    }
}
