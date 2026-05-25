namespace ThGameMgr.Ex
{
    internal interface IUserService
    {
        string CurrentUserName { get; set; }

        string CurrentUserDirectoryName { get; set; }

        string GetCurrentUserSettingsDirectory();

        string GetCurrentUserScoreBackupDirectoy();

        string GetCurrentUserGamePlayLogRecordFilePath();
    }
}
