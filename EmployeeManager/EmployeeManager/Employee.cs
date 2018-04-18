using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace EmployeeManager
{
    public struct Date
    {
        public int Day { get; set; }
        public string Month { get; set; }
        public int Year { get; set; }
    }

    public struct Position
    {
        public string PosName { get; set; }
        public int Term { get; set; }
    }

    [DataContract]
    public class Employee
    {
        public Employee()
        {
            subWorker = new List<Employee>();
            ClassName = "Employee";
            BossIndex = -1;
            DepIndex = -1;
            SubWorkerIndex = new int[256];
        }

        [DataMember]
        public string ClassName { get; set; }
        [DataMember]
        public string Surname { get; set; }
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public string Middlename { get; set; }
        [DataMember]
        public string PhotoPath { get; set; }
        [DataMember]
        public string Position { get; set; }
        [DataMember]
        public Date employmentDate;
        [DataMember]
        public Date dismissalDate;
        [DataMember]
        public bool IsFired { get; set; }
        [DataMember]
        public Position prevPos;
        [DataMember]
        public bool HasPrevPos { get; set; }
        public Department dep;
        [DataMember]
        public int DepIndex { get; set; }
        public Employee boss;
        [DataMember]
        public int BossIndex { get; set; }
        public List<Employee> subWorker;
        public int[] SubWorkerIndex { get; set; }

        public void Add(EmployeeWindow window)
        {
            if (window.empSurname.Text != "")
                Surname = window.empSurname.Text;
            else
                Surname = "Пусто";

            if (window.empName.Text != "")
                Name = window.empName.Text;
            else
                Name = "Пусто";
            if (window.empMiddlename.Text != "")
                Middlename = window.empMiddlename.Text;
            else
                Middlename = "Пусто";

            PhotoPath = window.empPhotoPath.Text;
            Position = window.empPosition.Text;

            if(window.employmentDay.SelectedItem != null)
                employmentDate.Day = int.Parse(window.employmentDay.Text);
            employmentDate.Month = window.employmentMonth.Text;
            if (window.employmentYear.SelectedItem != null)
                employmentDate.Year = int.Parse(window.employmentYear.Text);

            if(window.isFired.IsChecked == true && window.dismissalDay.Text != null && window.dismissalMonth.Text != null && window.dismissalYear.Text != null)
            {
                dismissalDate.Day = int.Parse(window.dismissalDay.Text);
                dismissalDate.Month = window.dismissalMonth.Text;
                dismissalDate.Year = int.Parse(window.dismissalYear.Text);
                IsFired = true;
            }

            if (window.hasPrevPos.IsChecked == true && window.prevPosName.Text != null)
            {
                prevPos.PosName = window.prevPosName.Text;

                if (int.TryParse(window.prevPosTerm.Text, out int tmp) == true)
                    prevPos.Term = tmp;
                else
                    System.Windows.MessageBox.Show("Ошибка! Некорректный ввод(сроку работы будет присвоено значение 0)");
                HasPrevPos = false;
            }

            if (window.empDepartment.SelectedItem != null)
            {
                dep = (Department)window.empDepartment.SelectedItem;
                DepIndex = window.depList.depManager.IndexOf(dep);
            }

            if (window.empBoss.SelectedItem != null)
            {
                boss = (Employee)window.empBoss.SelectedItem;
                boss.subWorker.Add(this);
                BossIndex = window.empList.empManager.IndexOf(boss);
            }

            if (window.subWorkerList.Items != null)
            {
                int i = 0;
                foreach (Employee item in window.subWorkerList.Items)
                {
                    item.boss = this;
                    subWorker.Add(item);
                    item.SubWorkerIndex = new int[256];
                    item.SubWorkerIndex[i] = window.empList.empManager.IndexOf(item);
                    i++;
                }
            }
        }

        /*public void Show(ShowEmp window, Employee emp)
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

                    window.empPhoto.Source = photo;
                }
            }
            catch
            {
                BitmapImage photo = new BitmapImage();
                photo.BeginInit();
                photo.UriSource = new Uri("Data//IMG//avatar.gif", UriKind.Relative);
                photo.CacheOption = BitmapCacheOption.OnLoad;
                photo.EndInit();

                window.empPhoto.Source = photo;
            }

            window.empSurname.Content = emp.Surname;
            window.empName.Content = emp.Name;
            window.empMiddlename.Content = emp.Middlename;
            window.empPos.Content = emp.Position;

            if(emp.employmentDate.Month != null && emp.employmentDate.Day != 0 && emp.employmentDate.Year != 0)
                window.employmentDate.Content = emp.employmentDate.Month + ", " + emp.employmentDate.Day + ", " + emp.employmentDate.Year;
            else
            {
                window.employmentDate.Content = "Информация отсутствует";
            }
            if (emp.IsFired == true)
            {
                if (emp.dismissalDate.Month != null && emp.dismissalDate.Day != 0 && emp.dismissalDate.Year != 0)
                    window.dismissalDate.Content = emp.dismissalDate.Month + ", " + emp.dismissalDate.Day + ", " + emp.dismissalDate.Year;
                else
                    window.dismissalDate.Content = "Информация отсутствует";
            }
            else
            {
                window.dismissalDate.Content = "Не уволен";
            }

            if(emp.HasPrevPos == true)
            {
                window.prevPos.Content = emp.prevPos.PosName + ", " + emp.prevPos.Term + " года(лет)";
            }
            else
            {
                window.prevPos.Content = "Информация отсутствует";
            }

            if(emp.dep != null)
                window.empDepName.Content = emp.dep.Name;
            else
            {
                window.empDep.Visibility = System.Windows.Visibility.Hidden;
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

                    window.bossPhoto.Source = photo;
                }
                window.bossSurname.Content = emp.boss.Surname;
                window.bossName.Content = emp.boss.Name;
                window.bossMiddlename.Content = emp.boss.Middlename;
            }
            else
            {
                window.empBoss.Visibility = System.Windows.Visibility.Hidden;
            }

            if (emp.subWorker != null)
                window.SubworkerList.ItemsSource = emp.subWorker;
        }*/
    }
}
