namespace ThGameMgr.Ex
{
    internal interface IUserService
    {
        string GetCurrentUserName();

        string GetCurrentUserSettingsDirectory();

        string GetCurrentUserScoreBackupDirectoy();

        string GetCurrentUserGamePlayLogRecordFilePath();
    }
}
