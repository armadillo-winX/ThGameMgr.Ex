namespace ThGameMgr.Ex
{
    public class UserService : IUserService
    {
        private string CurrentUserName { get; set; }

        private string CurrentUserDirectoryName { get; set; }

        public UserService()
        {
            this.CurrentUserName = string.Empty;
            this.CurrentUserDirectoryName = string.Empty;
        }

        /// <summary>
        /// アクティブなユーザーを切り替えます．
        /// </summary>
        /// <param name="userName"></param>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="UserNotFoundException"></exception>
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

        /// <summary>
        /// 現在アクティブなユーザーのユーザー名を取得します．
        /// </summary>
        /// <returns></returns>
        public string GetCurrentUserName()
        {
            return this.CurrentUserName;
        }

        /// <summary>
        /// 現在のユーザーの設定保存ディレクトリのパスを取得します．
        /// </summary>
        /// <returns></returns>
        public string GetCurrentUserSettingsDirectory()
        {
            return Path.Combine(PathInfo.UsersDirectory, this.CurrentUserDirectoryName, "Settings");
        }

        /// <summary>
        /// 現在のユーザーのスコアデータファイルのバックアップ格納ディレクトリのパスを取得します．
        /// </summary>
        /// <returns></returns>
        public string GetCurrentUserScoreBackupDirectoy()
        {
            return Path.Combine(PathInfo.UsersDirectory, this.CurrentUserDirectoryName, "backup");
        }

        /// <summary>
        /// 現在のユーザーのゲーム実行履歴ファイルを取得します．
        /// </summary>
        /// <returns></returns>
        public string GetCurrentUserGamePlayLogRecordFilePath()
        {
            return Path.Combine(PathInfo.UsersDirectory, this.CurrentUserDirectoryName, "GamePlayLog.xml");
        }
    }
}
