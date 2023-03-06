using SQLite;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using System.IO;

namespace TimeNetDUT
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            UserConfig user_settings = new AppConfigManager().GetConfig<UserConfig>();

            if (user_settings == null)
            {
                // TODO: Окно регистрации
            }
            else
            {
                // TODO: Стартовую страницу, если пользователь зарегестрирован
            }

            InitializeComponent();
        }
    }
}
