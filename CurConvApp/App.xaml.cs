using System.Configuration;
using System.Data;
using Microsoft.Extensions.Configuration;
using System.Windows;
using System;

namespace CurConvApp
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static IConfiguration AppConfiguration { get; private set; } = null!;


        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            AppConfiguration = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();
        }
    }
}
