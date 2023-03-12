using System;
using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TimeNetDUT
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            Utils.UserConfig user_settings = Utils.UserConfigManager.LoadConfig(); // Загружаем настройки пользователя из файла конфигурации

            // Проверяем, зарегистрирован ли пользователь, читая файл конфигурации
            if (user_settings.StudentId == -1)
            {
                // Если пользователь не зарегистрирован, отображаем страницу регистрации
                MainPage = new NavigationPage(new Views.RegistrationPage());
            }
            else
            {
                // Если пользователь уже зарегистрирован, отображаем главную страницу
                MainPage = new NavigationPage(new MainPage());
            }                        
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
