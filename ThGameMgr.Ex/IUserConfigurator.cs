namespace ThGameMgr.Ex
{
    public interface IUserConfigurator : IUserService
    {
        public void SwitchUser(string userName);
    }
}
