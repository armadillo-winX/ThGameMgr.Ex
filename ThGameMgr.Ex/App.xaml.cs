using Microsoft.Extensions.DependencyInjection;

namespace ThGameMgr.Ex
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        // DI コンテナプロバイダ
        public static IServiceProvider ServiceProvider { get; private set; } = null!;

        // OnStartup を override
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            ServiceCollection services = new ServiceCollection();
            // UserService をシングルトンとして追加
            services.AddSingleton<UserService>();
            // IUserService に UserService を関連付ける
            services.AddSingleton<IUserService>(provider => provider.GetRequiredService<UserService>());
            // IUserConfigurator に UserService を関連付ける
            services.AddSingleton<IUserConfigurator>(provider => provider.GetRequiredService<UserService>());
            // MainWindow を DI コンテナに追加
            services.AddTransient<MainWindow>();

            ServiceProvider = services.BuildServiceProvider();

            // MainWindow のコンストラクタへ UserService を注入
            MainWindow mainWindow = ServiceProvider.GetRequiredService<MainWindow>();
            mainWindow.Show();
        }
    }

}
