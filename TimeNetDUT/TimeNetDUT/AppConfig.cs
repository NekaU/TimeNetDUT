using Newtonsoft.Json;
using System;
using System.IO;

namespace TimeNetDUT
{
    public class AppConfig
    {
        public string BackgroundColor { get; set; }
        public string SecondaryColor { get; set; }
        public string FontColor { get; set; }
        public int FontSize { get; set; }
    }

    public class UserConfig
    {
        public int FacultyId { get; set; }
        public int Course { get; set; }
        public int GroupId { get; set; }
        public int StudentId { get; set; }
    }

    public class AppConfigManager
    {
        /*
            var appConfigManager = new AppConfigManager();
            var appConfig = appConfigManager.GetConfig<AppConfig>();
            var userConfig = appConfigManager.GetConfig<UserConfig>();
            appConfigManager.SaveConfig(appConfig);
            appConfigManager.SaveConfig(userConfig);
         */
        private const string ConfigFileName = "config.json"; // Имя файла конфигурации

        private readonly string configFilePath; // Путь к файлу конфигурации

        // Конструктор класса, инициализирует путь к файлу конфигурации
        public AppConfigManager()
        {
            string folderPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            configFilePath = Path.Combine(folderPath, ConfigFileName);
        }

        // Метод для чтения конфигурации из файла
        public T GetConfig<T>() where T : class, new()
        {
            // Если файл существует, то читаем содержимое и десериализуем в объект типа T
            if (File.Exists(configFilePath))
            {
                string configJson = File.ReadAllText(configFilePath);
                return JsonConvert.DeserializeObject<T>(configJson);
            }
            else // Если файла нет, то возвращаем null
            {
                return null;
            }
        }

        // Метод для сохранения конфигурации в файл
        public void SaveConfig<T>(T config)
        {
            string configJson = JsonConvert.SerializeObject(config);
            File.WriteAllText(configFilePath, configJson);
        }
    }
}
