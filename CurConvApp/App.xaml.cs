using System.Configuration;
using System.Data;
using Microsoft.Extensions.Configuration;
using System.Windows;
using System.Threading.Tasks;

namespace CurConvApp
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static IConfiguration AppConfiguration { get; private set; } = null!;

        protected override async void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            AppConfiguration = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            // Показати SplashWindow
            var splash = new SplashWindow();
            splash.Show();

            // Затримка (наприклад, 5 сек)
            await Task.Delay(10000);

            splash.Close();

            // Запуск головного вікна
            var main = new MainWindow();
            main.Show();
        }
    }
}
