using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimeNetDUT.Utils;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TimeNetDUT.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SchedulePage : ContentView
    {
        UserConfig user = UserConfigManager.LoadConfig();
        public SchedulePage()
        {
            InitializeComponent();
        }
    }
}