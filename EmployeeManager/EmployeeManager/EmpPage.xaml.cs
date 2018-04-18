using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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
    public partial class EmpPage : Page
    {
        Employee emp;
        public Window window;
        MainWindow mainWindow;

        public EmpPage(Window _window, MainWindow _mainWindow, Employee _emp)
        {
            InitializeComponent();

            window = _window;
            mainWindow = _mainWindow;
            emp = _emp;

            Show(emp);
        }

        public void Show(Employee emp)
        {
            try
            {
                if (emp.PhotoPath != "" && emp.PhotoPath != null)
                {
                    BitmapImage photo = new BitmapImage();
                    photo.BeginInit();
                    photo.UriSource = new Uri(emp.PhotoPath, UriKind.Relative);
                    photo.CacheOption = BitmapCacheOption.OnLoad;
                    photo.EndInit();

                    empPhoto.Source = photo;
                }
            }
            catch
            {
                BitmapImage photo = new BitmapImage();
                photo.BeginInit();
                photo.UriSource = new Uri("Data//IMG//avatar.gif", UriKind.Relative);
                photo.CacheOption = BitmapCacheOption.OnLoad;
                photo.EndInit();

                empPhoto.Source = photo;
            }

            empSurname.Content = emp.Surname;
            empName.Content = emp.Name;
            empMiddlename.Content = emp.Middlename;
            empPos.Content = emp.Position;

            if (emp.employmentDate.Month != null && emp.employmentDate.Day != 0 && emp.employmentDate.Year != 0)
                employmentDate.Content = emp.employmentDate.Month + ", " + emp.employmentDate.Day + ", " + emp.employmentDate.Year;
            else
            {
                employmentDate.Content = "Информация отсутствует";
            }
            if (emp.IsFired == true)
            {
                if (emp.dismissalDate.Month != null && emp.dismissalDate.Day != 0 && emp.dismissalDate.Year != 0)
                    dismissalDate.Content = emp.dismissalDate.Month + ", " + emp.dismissalDate.Day + ", " + emp.dismissalDate.Year;
                else
                    dismissalDate.Content = "Информация отсутствует";
            }
            else
            {
                dismissalDate.Content = "Не уволен";
            }

            if (emp.HasPrevPos == true)
            {
                prevPos.Content = emp.prevPos.PosName + ", " + emp.prevPos.Term + " года(лет)";
            }
            else
            {
                prevPos.Content = "Информация отсутствует";
            }

            if (emp.dep != null)
                empDepName.Content = emp.dep.Name;
            else
            {
                empDepName.Visibility = Visibility.Hidden;
            }

            if (emp.boss != null)
            {
                if (emp.boss.PhotoPath != "")
                {
                    BitmapImage photo = new BitmapImage();
                    photo.BeginInit();
                    photo.UriSource = new Uri(emp.boss.PhotoPath, UriKind.Relative);
                    photo.CacheOption = BitmapCacheOption.OnLoad;
                    photo.EndInit();

                    bossPhoto.Source = photo;
                }
                bossSurname.Content = emp.boss.Surname;
                bossName.Content = emp.boss.Name;
                bossMiddlename.Content = emp.boss.Middlename;
            }
            else
            {
                empBoss.Visibility = Visibility.Hidden;
            }

            if (emp.subWorker != null)
                SubworkerList.ItemsSource = emp.subWorker;
        }
    }
}
