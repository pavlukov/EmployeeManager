using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace EmployeeManager
{
    public partial class PluginMenu : Window
    {
        delegate void LoadHandler();

        public PluginMenu(List<Plugin> list)
        {
            InitializeComponent();

            //горячие клавиши
            AppSettingsReader reader = new AppSettingsReader();
            KeyGestureConverter gestureConverter = new KeyGestureConverter();

            try
            {
                string tmp = (string)reader.GetValue("Esc", typeof(string));
                gestureConverter = new KeyGestureConverter();
                KeyGesture escKeyGesture = (KeyGesture)gestureConverter.ConvertFromString(tmp);
                KeyBinding escKeyBinding = new KeyBinding(new RelayCommand(o =>
                {

                    MinHeight = 0;
                    var animation = new DoubleAnimation();
                    animation.From = Height;
                    animation.To = MinHeight;
                    animation.Duration = TimeSpan.FromSeconds(0.5);
                    animation.Completed += animation_Completed;
                    this.BeginAnimation(HeightProperty, animation);
                }, o => true), escKeyGesture);
                InputBindings.Add(escKeyBinding);
            }
            catch { }

            PluginList.ItemsSource = list;
        }

        void animation_Completed(object sender, EventArgs e)
        {
            Close();
        }

        void LoadPlugins()
        {
            AppSettingsReader reader = new AppSettingsReader();
            AppDomainSetup setup = new AppDomainSetup();
            string path = Directory.GetCurrentDirectory() + (string)reader.GetValue("pluginPath", typeof(string));
            setup.ApplicationBase = path;

            AppDomain pluginDomain = AppDomain.CreateDomain("plugin", null, setup);
            pluginDomain.DoCallBack(PluginInvoker.InvokePlugin);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            LoadHandler handler = new LoadHandler(LoadPlugins);
            Dispatcher.BeginInvoke(handler);
        }
    }
}
