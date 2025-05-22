using System;
using System.Windows;
using Microsoft.Extensions.Configuration;

namespace CurConvApp
{
    public partial class App : Application
    {
        public static IConfiguration AppConfiguration { get; private set; } = null!;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Завантаження конфігурації (якщо треба)
            AppConfiguration = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            var main = new MainWindow();
            this.MainWindow = main;
            main.Show();
        }
    }
}