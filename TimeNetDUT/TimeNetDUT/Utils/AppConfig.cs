using System;
using System.IO;
using Newtonsoft.Json;

namespace TimeNetDUT.Utils
{
    internal class AppConfig
    {
        public string BackgroundColor { get; set; } // Цвет фона приложения

        public string SecondaryColor { get; set; } // Вторичный цвет приложения

        public string TextFont { get; set; } // Шрифт текста приложения

        public int TextFontSize { get; set; } // Размер шрифта текста приложения

        public string TextColor { get; set; } // Цвет текста приложения
    }

    internal class AppConfigManager
    {
        private static readonly string ConfigFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "app_config.json"); // Путь к файлу конфигурации приложения

        // Метод сохранения конфигурации приложения
        public static void SaveConfig(AppConfig config)
        {
            var configJson = JsonConvert.SerializeObject(config); // Сериализуем конфигурацию в формат JSON

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

            var configJson = File.ReadAllText(ConfigFilePath); // Загружаем сериализованную конфигурацию из файла

            var config = JsonConvert.DeserializeObject<AppConfig>(configJson); // Десериализуем конфигурацию из формата JSON в экземпляр AppConfig
            
            return config; // Возвращаем загруженную конфигурацию
        }
    }

}
