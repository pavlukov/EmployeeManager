using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace EmployeeManager
{
    public partial class DepPage : Page
    {
        Department dep;
        Window window;
        MainWindow mainWindow;

        public DepPage(Window _window, MainWindow _mainWindow, Department _dep)
        {
            InitializeComponent();

            dep = _dep;
            window = _window;
            mainWindow = _mainWindow;

            Show(dep);
        }

        public void Show(Department dep)
        {
            name.Content = dep.Name;
            activity.Content = dep.Activity;
            year.Content = dep.Year;

            if (dep.boss != null)
            {
                bossSurname.Content = dep.boss.Surname;
                bossName.Content = dep.boss.Name;
                bossMiddlename.Content = dep.boss.Middlename;

                if (dep.boss.PhotoPath != "" && dep.boss.PhotoPath != null)
                {
                    BitmapImage photo = new BitmapImage();
                    photo.BeginInit();
                    photo.UriSource = new Uri(dep.boss.PhotoPath, UriKind.Relative);
                    photo.CacheOption = BitmapCacheOption.OnLoad;
                    photo.EndInit();

                    bossPhoto.Source = photo;
                }
            }
            else
            {
                bossPhoto.Visibility = Visibility.Hidden;
                bossSurname.Visibility = Visibility.Hidden;
                bossName.Visibility = Visibility.Hidden;
                bossMiddlename.Visibility = Visibility.Hidden;
            }
        }

    }
}
