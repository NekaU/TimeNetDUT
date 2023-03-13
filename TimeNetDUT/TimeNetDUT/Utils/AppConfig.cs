using System;
using System.IO;
using Newtonsoft.Json;
using Xamarin.Forms;

namespace TimeNetDUT.Utils
{
    public class AppConfig
    {
        // Цветовая схема
        public Color PrimaryColor { get; set; }
        public Color SecondaryColor { get; set; }
        public Color BackgroundColor { get; set; }
        public Color TextColor { get; set; }

        // Размеры шрифтов
        public double SmallFontSize { get; set; }
        public double MediumFontSize { get; set; }
        public double LargeFontSize { get; set; }

        // Границы и отступы элементов интерфейса
        public Thickness PagePadding { get; set; }
        public Thickness ElementMargin { get; set; }
        public double ElementSpacing { get; set; }

        // Размеры элементов интерфейса
        public double ButtonHeight { get; set; }
        public double ButtonCornerRadius { get; set; }
        public double EntryHeight { get; set; }
        public double EntryCornerRadius { get; set; }
        public double PickerHeight { get; set; }
        public double SwitchHeight { get; set; }
        public double SwitchThumbRadius { get; set; }
    }


    internal static class AppConfigManager
    {
        private static readonly string ConfigFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "app_config.json"); // Путь к файлу конфигурации приложения

        // Метод сохранения конфигурации приложения
        public static void SaveConfig(AppConfig config)
        {
            string configJson = JsonConvert.SerializeObject(config); // Сериализуем конфигурацию в формат JSON

            File.WriteAllText(ConfigFilePath, configJson); // Записываем сериализованную конфигурацию в файл
        }

        // Метод загрузки конфигурации приложения
        public static AppConfig LoadConfig()
        {
            // Если файл конфигурации не существует, возвращаем пустой экземпляр AppConfig
            if (!File.Exists(ConfigFilePath))
            {
                return new AppConfig();
            }

            string configJson = File.ReadAllText(ConfigFilePath); // Загружаем сериализованную конфигурацию из файла

            AppConfig config = JsonConvert.DeserializeObject<AppConfig>(configJson); // Десериализуем конфигурацию из формата JSON в экземпляр AppConfig
            
            return config; // Возвращаем загруженную конфигурацию
        }
    }

}
