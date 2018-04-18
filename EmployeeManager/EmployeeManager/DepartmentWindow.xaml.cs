using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;

namespace EmployeeManager
{
    public partial class DepartmentWindow : Window
    {
        DataContractJsonSerializer JsonFormatterEmp = new DataContractJsonSerializer(typeof(List<Employee>));
        DataContractJsonSerializer JsonFormatterDep = new DataContractJsonSerializer(typeof(List<Department>));
        internal EmployeeList empList;
        DepartmentList depList;
        Window window;
        MainWindow mainWindow;
        bool changeMode;

        public DepartmentWindow(Window _window, MainWindow _mainWindow, EmployeeList _empList, DepartmentList _depList)
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

            List<int> year = new List<int>();
            int currentDate = DateTime.Today.Year;
            for (int i = 0; currentDate >= 1982; i++)
            {
                year.Add(currentDate);
                currentDate--;
            }

            depYear.ItemsSource = year;
            depBoss.ItemsSource = empList.empManager;

            foreach (Department item in depList.depManager)
            {
                departmentList.Items.Add(item);
            }
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            if (mainWindow == window)
                mainWindow.Show();
            Close();
        }

        private void DepartmentList_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete)
            {
                foreach (Employee emp in empList.empManager)
                {
                    if (emp.dep != null)
                        if (emp.dep == (Department)departmentList.SelectedItem)
                            emp.dep = null;
                }
                depList.depManager.Remove((Department)departmentList.SelectedItem);
                departmentList.Items.Remove(departmentList.SelectedItem);
            }
        }

        private void DepartmentList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (departmentList.SelectedItem != null)
            {
                ShowDep showDep = new ShowDep(this, mainWindow, (Department)departmentList.SelectedItem);
                showDep.Show();
                Hide();
            }
        }

        private void OK_Click(object sender, RoutedEventArgs e)
        {
            bool add = true;
            if (changeMode == false)
            {
                Department dep = new Department();

                foreach (Department item in depList.depManager)
                {
                    if (item.Name == depName.Text && depName.Text != "Пусто")
                        add = false;
                }

                if (add == true)
                {
                    dep.Add(this);

                    depList.depManager.Add(dep);
                    departmentList.Items.Clear();
                    foreach (Department item in depList.depManager)
                    {
                        departmentList.Items.Add(item);
                    }
                }
            }
            else
            {
                Department dep = new Department();

                foreach (Department item in depList.depManager)
                {
                    if (item.Name == depName.Text && depName.Text != "Пусто")
                        add = false;
                }

                if (add == true)
                {
                    dep.Add(this);

                    depList.depManager.Remove((Department)departmentList.SelectedItem);
                    depList.depManager.Add(dep);
                    departmentList.Items.Clear();
                    foreach (Department item in depList.depManager)
                    {
                        departmentList.Items.Add(item);
                    }
                    changeMode = false;
                }
            }

            depName.Clear();
            depYear.SelectedItem = null;
            depActivity.Clear();
            depBoss.SelectedItem = null;
        }

        private void Change_Click(object sender, RoutedEventArgs e)
        {
            if (departmentList.SelectedItem != null)
            {
                changeMode = true;
                Department dep = (Department)departmentList.SelectedItem;
                depName.Text = dep.Name;
                depYear.SelectedItem = dep.Year;
                depActivity.Text = dep.Activity;
                depBoss.SelectedItem = dep.boss;
            }
        }

        private void Return_Click(object sender, RoutedEventArgs e)
        {
            window.Show();
            Close();
        }

        private void Load_Click(object sender, RoutedEventArgs e)
        {
            FileManager.Load(empList, depList);

            departmentList.Items.Clear();
            foreach (Department dep in depList.depManager)
            {
                departmentList.Items.Add(dep);
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new System.Windows.Forms.FolderBrowserDialog();
            System.Windows.Forms.DialogResult result = dlg.ShowDialog();
            string fileName = "Untitled";
            int index = 0;
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                fileName = dlg.SelectedPath;
                index = fileName.LastIndexOf(@"\");
            }

            string empListName = fileName + fileName.Remove(0, index) + "(Employee).json";
            string depListName = fileName + fileName.Remove(0, index) + "(Department).json";

            using (FileStream fs = new FileStream(empListName, FileMode.OpenOrCreate))
            {
                JsonFormatterEmp.WriteObject(fs, empList.empManager);
            }

            using (FileStream fs = new FileStream(depListName, FileMode.OpenOrCreate))
            {
                JsonFormatterDep.WriteObject(fs, depList.depManager);                      
            }

            if (Directory.Exists(fileName))
            {
                ZipFile.CreateFromDirectory(fileName, fileName + ".zip");
                Directory.Delete(fileName, true);
            }
        }

        private void DepSearch_KeyUp(object sender, KeyEventArgs e)
        {
            departmentList.Items.Clear();
            var newDepList = depList.depManager.Where(department => department.Name.Contains(depSearch.Text)).AsParallel();
            foreach (Department dep in newDepList)
            {
                departmentList.Items.Add(dep);
            }
        }

        private void DepSearch_GotFocus(object sender, RoutedEventArgs e)
        {
            depSearch.Clear();
        }

        private void DepSearch_LostFocus(object sender, RoutedEventArgs e)
        {
            depSearch.Text = "Поиск";
        }

        private void Sort(ListSortDirection direction)
        {
            if (direction == ListSortDirection.Ascending)
            {
                var newDepList = depList.depManager.AsParallel().OrderBy(department => department.Name);
                departmentList.Items.Clear();
                foreach (Department dep in newDepList)
                {
                    departmentList.Items.Add(dep);
                }
            }
            else if (direction == ListSortDirection.Descending)
            {
                var newDepList = depList.depManager.AsParallel().OrderByDescending(department => department.Name);
                departmentList.Items.Clear();
                foreach (Department dep in newDepList)
                {
                    departmentList.Items.Add(dep);
                }
            }

        }

        GridViewColumnHeader _lastHeaderClicked = null;
        ListSortDirection _lastDirection = ListSortDirection.Ascending;
        void GridViewColumnHeaderClickedHandler(object sender, RoutedEventArgs e)
        {
            GridViewColumnHeader headerClicked = e.OriginalSource as GridViewColumnHeader;
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
                        else
                        {
                            direction = ListSortDirection.Ascending;
                        }
                    }

                    if (headerClicked.Content.ToString() == "Название")
                        Sort(direction);

                    _lastDirection = direction;
                    _lastHeaderClicked = headerClicked;
                }
            }
        }
    }
}
