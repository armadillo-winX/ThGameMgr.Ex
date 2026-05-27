namespace ThGameMgr.Ex
{
    public interface IUserStateConfigurator : IUserService
    {
        public void SwitchUser(string userName);
    }
}
