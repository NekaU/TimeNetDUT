using System;
using System.IO;
using Newtonsoft.Json;

namespace TimeNetDUT.Utils
{
    internal class UserConfig
    {
        public int FacultyId { get; set; } = -1;
        public int Course { get; set; } = -1;
        public int GroupId { get; set; } = -1;
        public int StudentId { get; set; } = -1;
    }

    internal static class UserConfigManager
    {
        private static readonly string ConfigFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "user_config.json"); // Путь к файлу конфигурации

        // Метод сохранения конфигурации пользователя
        public static void SaveConfig(UserConfig config)
        {
            string configJson = JsonConvert.SerializeObject(config); // Сериализуем конфигурацию в формат JSON

            File.WriteAllText(ConfigFilePath, configJson); // Записываем сериализованную конфигурацию в файл
        }

        // Метод загрузки конфигурации пользователя
        public static UserConfig LoadConfig()
        {
            // Если файл конфигурации не существует, возвращаем пустой экземпляр UserConfig
            if (!File.Exists(ConfigFilePath))
            {
                return new UserConfig();
            }

            string configJson = File.ReadAllText(ConfigFilePath); // Загружаем сериализованную конфигурацию из файла

            UserConfig config = JsonConvert.DeserializeObject<UserConfig>(configJson); // Десериализуем конфигурацию из формата JSON в экземпляр UserConfig

            return config; // Возвращаем загруженную конфигурацию
        }
    }
}
