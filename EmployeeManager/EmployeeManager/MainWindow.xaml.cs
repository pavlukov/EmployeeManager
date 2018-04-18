using System;
using System.Collections.Generic;
using System.IO;
using EmployeeManager.SDK;
using System.Runtime.Serialization.Json;
using System.Windows;
using System.Windows.Input;
using System.Configuration;
using System.Drawing;
using System.Windows.Media.Imaging;
using System.Windows.Interop;
using System.Windows.Media.Animation;

namespace EmployeeManager
{
    public class Plugin
    {
        public string Name { get; set; }
    }

    public class RelayCommand : ICommand
    {
        private Action<object> execute;
        private Func<object, bool> canExecute;

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public RelayCommand(Action<object> execute, Func<object, bool> canExecute = null)
        {
            this.execute = execute;
            this.canExecute = canExecute;
        }

        public bool CanExecute(object parameter)
        {
            return this.canExecute == null || this.canExecute(parameter);
        }

        public void Execute(object parameter)
        {
            this.execute(parameter);
        }
    }

    public partial class MainWindow : Window
    {
        DataContractJsonSerializer JsonFormatterEmp = new DataContractJsonSerializer(typeof(List<Employee>));
        DataContractJsonSerializer JsonFormatterDep = new DataContractJsonSerializer(typeof(List<Department>));
        internal EmployeeList empList = new EmployeeList();
        internal DepartmentList depList = new DepartmentList();
        internal List<Plugin> pluginList = new List<Plugin>();

        public MainWindow()
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
                    PluginMenu window = new PluginMenu(pluginList);
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
                    animation.Completed += animation_Completed;
                    this.BeginAnimation(HeightProperty, animation);
                }, o => true), escKeyGesture);
                InputBindings.Add(escKeyBinding);
            }
            catch { }

            //загрузка иконок
            try
            {
                var bitmapSource = Imaging.CreateBitmapSourceFromHBitmap(EMResource.FileIcon.GetHbitmap(),
                                          IntPtr.Zero,
                                          Int32Rect.Empty,
                                          BitmapSizeOptions.FromEmptyOptions());
                fileImg.Source = bitmapSource;
            }
            catch { }

            try
            {
                var bitmapSource = Imaging.CreateBitmapSourceFromHBitmap(EMResource.PluginIcon.GetHbitmap(),
                                          IntPtr.Zero,
                                          Int32Rect.Empty,
                                          BitmapSizeOptions.FromEmptyOptions());
                pluginImg.Source = bitmapSource;
            }
            catch { }

            //список плагинов
            try
            {
                foreach (string file in Directory.GetFiles(Directory.GetCurrentDirectory() + (string)reader.GetValue("pluginPath", typeof(string))))
                {
                    string assemblyName = file.Remove(0, file.LastIndexOf(@"\") + 1);
                    assemblyName = assemblyName.Remove(assemblyName.Length - 4, 4);

                    if (assemblyName != "EmployeeManager.SDK")
                    {
                        Plugin tmp = new Plugin();
                        tmp.Name = assemblyName;
                        pluginList.Add(tmp);
                    }
                }
            }
            catch { }
        }


        void animation_Completed(object sender, EventArgs e)
        {
            Close();
        }

        private void Employee_Click(object sender, RoutedEventArgs e)
        {
            EmployeeWindow employeeWindow = new EmployeeWindow(this, this, empList, depList);
            employeeWindow.Show();
            Hide();
        }

        private void Departments_Click(object sender, RoutedEventArgs e)
        {
            DepartmentWindow departmentWindow = new DepartmentWindow(this, this, empList, depList);
            departmentWindow.Show();
            Hide();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            if (empList.empManager.Count > 0 || depList.depManager.Count > 0)
            {   
                if (MessageBox.Show("Сохранить перед выходом?", "Выход", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    FileManager.Save(empList, depList);
                }
            }
            Environment.Exit(0);
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            FileManager.Load(empList, depList);
        }

        private void MenuItem_Click_1(object sender, RoutedEventArgs e)
        {
            FileManager.Save(empList, depList);
        }

        private void MenuItem_Click_2(object sender, RoutedEventArgs e)
        {
            PluginMenu window = new PluginMenu(pluginList);
            window.Show();
        }
    }
}
