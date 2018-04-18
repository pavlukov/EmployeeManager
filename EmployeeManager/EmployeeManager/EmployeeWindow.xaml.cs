using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.ComponentModel;
using System.Configuration;
using System.Windows.Media.Animation;

namespace EmployeeManager
{
    public partial class EmployeeWindow : Window
    {
        internal EmployeeList empList;
        internal DepartmentList depList;
        Window window;
        MainWindow mainWindow;
        bool changeMode;

        public EmployeeWindow(Window _window, MainWindow _mainWindow, EmployeeList _empList, DepartmentList _depList)
        {
            InitializeComponent();

            //горячие клавиши
            AppSettingsReader reader = new AppSettingsReader();
            KeyGestureConverter gestureConverter = new KeyGestureConverter();
            try
            {
                string tmp = (string)reader.GetValue("Save", typeof(string));
                KeyGesture saveKeyGesture = (KeyGesture)gestureConverter.ConvertFromString(tmp);
                KeyBinding saveKeyBinding = new KeyBinding(new RelayCommand(o =>
                {
                    FileManager.Save(empList, depList);
                }, o => true), saveKeyGesture);
                InputBindings.Add(saveKeyBinding);
            }
            catch { }

            try
            {
                string tmp = (string)reader.GetValue("Load", typeof(string));
                gestureConverter = new KeyGestureConverter();
                KeyGesture loadKeyGesture = (KeyGesture)gestureConverter.ConvertFromString(tmp);
                KeyBinding loadKeyBinding = new KeyBinding(new RelayCommand(o =>
                {
                    FileManager.Load(empList, depList);
                }, o => true), loadKeyGesture);
                InputBindings.Add(loadKeyBinding);
            }
            catch { }

            try
            {
                string tmp = (string)reader.GetValue("Plugins", typeof(string));
                gestureConverter = new KeyGestureConverter();
                KeyGesture pluginsKeyGesture = (KeyGesture)gestureConverter.ConvertFromString(tmp);
                KeyBinding pluginsKeyBinding = new KeyBinding(new RelayCommand(o =>
                {
                    PluginMenu window = new PluginMenu(mainWindow.pluginList);
                    window.Show();
                }, o => true), pluginsKeyGesture);
                InputBindings.Add(pluginsKeyBinding);
            }
            catch { }

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
                    animation.Completed += Window_Closed;
                    this.BeginAnimation(HeightProperty, animation);
                }, o => true), escKeyGesture);
                InputBindings.Add(escKeyBinding);
            }
            catch { }


            empList = _empList;
            depList = _depList;
            window = _window;
            mainWindow = _mainWindow;

            int[] day = new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16,
                                     17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31};

            string[] month = new string[] { "Январь", "Февраль", "Март", "Апрель",
                                            "Май", "Июнь", "Июль", "Август",
                                            "Сентябрь", "Октябрь", "Ноябрь", "Декабрь"};

            List<int> year = new List<int>();
            int currentDate = DateTime.Today.Year;
            for (int i = 0; currentDate >= 1982; i++)
            {
                year.Add(currentDate);
                currentDate--;
            }


            employmentDay.ItemsSource = day;
            employmentMonth.ItemsSource = month;
            employmentYear.ItemsSource = year;

            dismissalDay.ItemsSource = day;
            dismissalMonth.ItemsSource = month;
            dismissalYear.ItemsSource = year;

            empDepartment.ItemsSource = depList.depManager;
            empBoss.ItemsSource = empList.empManager;
            empSubWorkers.ItemsSource = empList.empManager;

            foreach (Employee item in empList.empManager)
            {
                employeeList.Items.Add(item);
            }
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            if (mainWindow == window)
                mainWindow.Show();
            Close();
        }

        private void BrowsePhoto(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.Filter = "*.BMP *.JPG *.PNG *.JPEG | *.bmp; *.jpg; *.png; *.jpeg";

            bool? result = dlg.ShowDialog();
            if (result == true)
            {
                string filename = dlg.FileName;
                empPhotoPath.Text = filename;
            }
        }

        private void IsFired_Click(object sender, RoutedEventArgs e)
        {
            if (isFired.IsChecked == true)
            {
                dismissalDay.IsEnabled = true;
                dismissalMonth.IsEnabled = true;
                dismissalYear.IsEnabled = true;
            }
            else
            {
                dismissalDay.IsEnabled = false;
                dismissalMonth.IsEnabled = false;
                dismissalYear.IsEnabled = false;
            }
        }

        private void HasPrevPos_Click(object sender, RoutedEventArgs e)
        {
            if (hasPrevPos.IsChecked == true)
            {
                prevPosName.IsEnabled = true;
                prevPosTerm.IsEnabled = true;
            }
            else
            {
                prevPosName.IsEnabled = false;
                prevPosTerm.IsEnabled = false;
            }
        }

        private void EmpSubWorkerAdd_Click(object sender, RoutedEventArgs e)
        {
            if (empSubWorkers.SelectedItem != null && subWorkerList.Items.Contains(empSubWorkers.SelectedItem) == false)
                subWorkerList.Items.Add(empSubWorkers.SelectedItem);
        }

        private void OK_Click(object sender, RoutedEventArgs e)
        {
            bool check = true;
            if (changeMode == false)
            {
                Employee emp = new Employee();

                if (empBoss.SelectedItem != null && subWorkerList.Items != null)
                {
                    foreach (Employee item in subWorkerList.Items)
                    {
                        if (empBoss.SelectedItem == item)
                        {
                            MessageBox.Show("Ошибка! Начальник не может быть подчиненным.");
                            check = false;

                            subWorkerList.Items.Remove(item);
                            empBoss.SelectedItem = null;
                            empSubWorkers.SelectedItem = null;
                            break;
                        }
                    }

                    if (check == true)
                    {
                        emp.Add(this);

                        empList.empManager.Add(emp);
                        employeeList.Items.Clear();
                        foreach (Employee employee in empList.empManager)
                        {
                            employeeList.Items.Add(employee);
                        }

                        empSurname.Clear();
                        empName.Clear();
                        empMiddlename.Clear();
                        empPhotoPath.Clear();
                        empPosition.Clear();
                        employmentDay.SelectedItem = null;
                        employmentMonth.SelectedItem = null;
                        employmentYear.SelectedItem = null;
                        isFired.IsChecked = false;
                        dismissalDay.SelectedItem = null;
                        dismissalMonth.SelectedItem = null;
                        dismissalYear.SelectedItem = null;
                        hasPrevPos.IsChecked = false;
                        prevPosName.Clear();
                        prevPosTerm.Clear();
                        empDepartment.SelectedItem = null;
                        empBoss.SelectedItem = null;
                        subWorkerList.Items.Clear();
                        empSubWorkers.SelectedItem = null;
                    }
                }
                else
                {
                    emp.Add(this);

                    empList.empManager.Add(emp);
                    employeeList.Items.Clear();
                    foreach (Employee employee in empList.empManager)
                    {
                        employeeList.Items.Add(employee);
                    }

                    empSurname.Clear();
                    empName.Clear();
                    empMiddlename.Clear();
                    empPhotoPath.Clear();
                    empPosition.Clear();
                    employmentDay.SelectedItem = null;
                    employmentMonth.SelectedItem = null;
                    employmentYear.SelectedItem = null;
                    isFired.IsChecked = false;
                    dismissalDay.SelectedItem = null;
                    dismissalMonth.SelectedItem = null;
                    dismissalYear.SelectedItem = null;
                    hasPrevPos.IsChecked = false;
                    prevPosName.Clear();
                    prevPosTerm.Clear();
                    empDepartment.SelectedItem = null;
                    empBoss.SelectedItem = null;
                    subWorkerList.Items.Clear();
                    empSubWorkers.SelectedItem = null;
                }
            }
            else
            {
                Employee emp = new Employee();

                if (empBoss.SelectedItem != null && subWorkerList.Items != null)
                {
                    check = true;
                    foreach (Employee item in subWorkerList.Items)
                    {
                        if (empBoss.SelectedItem == item)
                        {
                            MessageBox.Show("Ошибка! Начальник не может быть подчиненным.");
                            check = false;

                            subWorkerList.Items.Remove(item);
                            empBoss.SelectedItem = null;
                            empSubWorkers.SelectedItem = null;
                            break;
                        }
                    }

                    if (check == true)
                    {
                        emp.Add(this);

                        empList.empManager.Remove((Employee)employeeList.SelectedItem);
                        empList.empManager.Add(emp);
                        employeeList.Items.Clear();
                        foreach (Employee employee in empList.empManager)
                        {
                            employeeList.Items.Add(employee);
                        }
                        changeMode = false;

                        empSurname.Clear();
                        empName.Clear();
                        empMiddlename.Clear();
                        empPhotoPath.Clear();
                        empPosition.Clear();
                        employmentDay.SelectedItem = null;
                        employmentMonth.SelectedItem = null;
                        employmentYear.SelectedItem = null;
                        isFired.IsChecked = false;
                        dismissalDay.SelectedItem = null;
                        dismissalMonth.SelectedItem = null;
                        dismissalYear.SelectedItem = null;
                        hasPrevPos.IsChecked = false;
                        prevPosName.Clear();
                        prevPosTerm.Clear();
                        empDepartment.SelectedItem = null;
                        empBoss.SelectedItem = null;
                        subWorkerList.Items.Clear();
                        empSubWorkers.SelectedItem = null;
                    }
                }
                else
                {
                    emp.Add(this);

                    empList.empManager.Remove((Employee)employeeList.SelectedItem);
                    empList.empManager.Add(emp);
                    employeeList.Items.Clear();
                    foreach (Employee employee in empList.empManager)
                    {
                        employeeList.Items.Add(employee);
                    }
                    changeMode = false;

                    empSurname.Clear();
                    empName.Clear();
                    empMiddlename.Clear();
                    empPhotoPath.Clear();
                    empPosition.Clear();
                    employmentDay.SelectedItem = null;
                    employmentMonth.SelectedItem = null;
                    employmentYear.SelectedItem = null;
                    isFired.IsChecked = false;
                    dismissalDay.SelectedItem = null;
                    dismissalMonth.SelectedItem = null;
                    dismissalYear.SelectedItem = null;
                    hasPrevPos.IsChecked = false;
                    prevPosName.Clear();
                    prevPosTerm.Clear();
                    empDepartment.SelectedItem = null;
                    empBoss.SelectedItem = null;
                    subWorkerList.Items.Clear();
                    empSubWorkers.SelectedItem = null;
                }
            }
        }



        private void EmployeeList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (employeeList.SelectedItem != null)
            {
                ShowEmp showEmp = new ShowEmp(this, mainWindow, (Employee)employeeList.SelectedItem);
                showEmp.Show();
                Hide();
            }
        }

        private void EmployeeList_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete)
            {
                foreach (Employee emp in empList.empManager)
                {
                    if (emp.boss != null)
                        if (emp.boss == (Employee)employeeList.SelectedItem)
                            emp.boss = null;
                    if (emp.subWorker != null)
                        if (emp.subWorker.Contains((Employee)employeeList.SelectedItem))
                            emp.subWorker.Remove((Employee)employeeList.SelectedItem);
                }

                foreach (Department dep in depList.depManager)
                {
                    if (dep.boss != null)
                        if (dep.boss == (Employee)employeeList.SelectedItem)
                            dep.boss = null;
                }

                empList.empManager.Remove((Employee)employeeList.SelectedItem);
                employeeList.Items.Remove(employeeList.SelectedItem);
            }
        }

        private void Change_Click(object sender, RoutedEventArgs e)
        {
            if (employeeList.SelectedItem != null)
            {
                changeMode = true;
                Employee emp = (Employee)employeeList.SelectedItem;
                empSurname.Text = emp.Surname;
                empName.Text = emp.Name;
                empMiddlename.Text = emp.Middlename;
                empPhotoPath.Text = emp.PhotoPath;
                empPosition.Text = emp.Position;
                employmentDay.SelectedItem = emp.employmentDate.Day;
                employmentMonth.SelectedItem = emp.employmentDate.Month;
                employmentYear.SelectedItem = emp.employmentDate.Year;
                isFired.IsChecked = emp.IsFired;
                dismissalDay.SelectedItem = emp.dismissalDate.Day;
                dismissalMonth.SelectedItem = emp.dismissalDate.Month;
                dismissalYear.SelectedItem = emp.dismissalDate.Year;
                hasPrevPos.IsChecked = emp.HasPrevPos;
                prevPosName.Text = emp.prevPos.PosName;
                prevPosTerm.Text = emp.prevPos.Term.ToString();
                empDepartment.SelectedItem = emp.dep;
                empBoss.SelectedItem = emp.boss;
                if (emp.subWorker != null)
                    foreach (Employee item in emp.subWorker)
                        subWorkerList.Items.Add(item);
                empSubWorkers.SelectedItem = null;
            }
        }

        private void Return_Click(object sender, RoutedEventArgs e)
        {
            window.Show();
            Close();
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            FileManager.Save(empList, depList);
        }

        private void Load_Click(object sender, RoutedEventArgs e)
        {
            FileManager.Load(empList, depList);

            employeeList.Items.Clear();
            foreach (Employee employee in empList.empManager)
            {
                employeeList.Items.Add(employee);
            }
        }

        private void SubWorkerList_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete)
            {
                subWorkerList.Items.Remove(subWorkerList.SelectedItem);
            }
        }

        private void EmpSearch_GotFocus(object sender, RoutedEventArgs e)
        {
            empSearch.Clear();
        }

        private void EmpSearch_KeyUp(object sender, KeyEventArgs e)
        {
            employeeList.Items.Clear();
            var newEmpList = empList.empManager.Where(employee => employee.Surname.Contains(empSearch.Text)).AsParallel();
            foreach (Employee emp in newEmpList)
            {
                employeeList.Items.Add(emp);
            }
        }

        private void EmpSearch_LostFocus(object sender, RoutedEventArgs e)
        {
            empSearch.Text = "Поиск";
        }

        private void Sort(ListSortDirection direction)
        {
            if (direction == ListSortDirection.Ascending)
            {
                var newEmpList = empList.empManager.AsParallel().OrderBy(employee => employee.Surname);
                employeeList.Items.Clear();
                foreach (Employee emp in newEmpList)
                {
                    employeeList.Items.Add(emp);
                }
            }
            else if (direction == ListSortDirection.Descending)
            {
                var newEmpList = empList.empManager.AsParallel().OrderByDescending(employee => employee.Surname);
                employeeList.Items.Clear();
                foreach (Employee emp in newEmpList)
                {
                    employeeList.Items.Add(emp);
                }
            }
           
        }

        GridViewColumnHeader _lastHeaderClicked = null;
        ListSortDirection _lastDirection = ListSortDirection.Ascending;
        void GridViewColumnHeaderClickedHandler(object sender, RoutedEventArgs e)
        {
            GridViewColumnHeader headerClicked =
              e.OriginalSource as GridViewColumnHeader;
            ListSortDirection direction = _lastDirection;

            if (headerClicked != null)
            {
                if (headerClicked.Role != GridViewColumnHeaderRole.Padding)
                {
                    if (headerClicked != _lastHeaderClicked)
                    {
                        direction = ListSortDirection.Ascending;
                    }
                    else
                    {
                        if (_lastDirection == ListSortDirection.Ascending)
                        {
                            direction = ListSortDirection.Descending;
                        }
                        else if (_lastDirection == ListSortDirection.Descending)
                        {
                            direction = ListSortDirection.Ascending;
                        }
                    }

                    if (headerClicked.Content.ToString() == "Фамилия")
                        Sort(direction);

                    _lastDirection = direction;
                    _lastHeaderClicked = headerClicked;
                }
            }
        }
    }
}


