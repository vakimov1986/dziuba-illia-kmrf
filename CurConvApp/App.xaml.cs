﻿using System;
using System.Windows;
using Microsoft.Extensions.Configuration;

namespace CurConvApp
{
    public partial class App : Application
    {
        public static IConfiguration AppConfiguration { get; private set; } = null!;

        protected override void OnStartup(StartupEventArgs e)
        {
            WPFLocalizeExtension.Engine.LocalizeDictionary.Instance.Culture = new System.Globalization.CultureInfo("uk-UA"); // або "en"

            base.OnStartup(e);

            // Завантаження конфігурації (якщо треба)
            AppConfiguration = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            var main = new MainWindow();
            main.Show();
        }
    }
}