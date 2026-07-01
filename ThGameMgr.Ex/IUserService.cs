namespace ThGameMgr.Ex
{
    public interface IUserService
    {
        string GetCurrentUserName();

        string GetCurrentUserSettingsDirectory();

        string GetCurrentUserScoreBackupDirectoy();

        string GetCurrentUserGamePlayLogRecordFilePath();

        string GetCurrentUserReplayBackupDirectory();

        string GetCurrentUserMacroVaultArchiveFilePath();

        string GetCurrentUserMacroArchiveEntropyConfigPath();
    }
}
