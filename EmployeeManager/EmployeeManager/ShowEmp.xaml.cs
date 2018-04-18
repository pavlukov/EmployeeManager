using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;

namespace EmployeeManager
{
    public partial class ShowEmp : Window
    {
        Employee emp;
        public Window window;
        MainWindow mainWindow;
        bool showMain;
        List<EmpPage> pages;    
        List<EmpPage> lastPages = new List<EmpPage>();
        int currentPage;

        public ShowEmp(Window _window, MainWindow _mainWindow, Employee _emp)
        {
            InitializeComponent();

            window = _window;
            mainWindow = _mainWindow;
            emp = _emp;
            showMain = true;

            pages = new List<EmpPage>();
            currentPage = mainWindow.empList.empManager.IndexOf(emp);
            Task.Factory.StartNew(() => 
            {
                Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                {
                    lastPages.Add(new EmpPage(window, mainWindow, mainWindow.empList.empManager[0]));
                    lastPages.Add(new EmpPage(window, mainWindow, mainWindow.empList.empManager[mainWindow.empList.empManager.Count - 1]));
                    if (currentPage != 0)
                        pages.Add(new EmpPage(window, mainWindow, mainWindow.empList.empManager[currentPage - 1]));
                    if (currentPage != mainWindow.empList.empManager.Count - 1)
                            pages.Add(new EmpPage(window, mainWindow, mainWindow.empList.empManager[currentPage + 1]));
                }));
            });
            empPage.NavigationService.Navigate(new EmpPage(window, mainWindow, emp));

            if (mainWindow.empList.empManager.Count == 1)
                Next.IsEnabled = false;
            if (currentPage == 0)
                Prev.IsEnabled = false;
            if (currentPage == mainWindow.empList.empManager.Count - 1)
                Next.IsEnabled = false;
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            if(showMain == true)
                mainWindow.Show();
            Close();
        }

        private void Return_Click(object sender, RoutedEventArgs e)
        {
            showMain = false;
            window.Show();
            Close(); 
        }

        private void First_Click(object sender, RoutedEventArgs e)
        {
            currentPage = 0;
            pages = new List<EmpPage>();
            Task.Factory.StartNew(() =>
            {
                Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                {
                    if (currentPage != mainWindow.empList.empManager.Count - 1)
                        pages.Add(new EmpPage(window, mainWindow, mainWindow.empList.empManager[currentPage + 1]));
                }));
            });
            empPage.NavigationService.Navigate(lastPages[0]);

            if (currentPage != pages.Count - 1)
                Next.IsEnabled = true;
            else
                Next.IsEnabled = false;
            Prev.IsEnabled = false;
        }

        private void Next_Click(object sender, RoutedEventArgs e)
        {
            currentPage++;
            if(pages.Count > 1)
                empPage.NavigationService.Navigate(pages[1]);
            else
                empPage.NavigationService.Navigate(pages[0]);
            pages = new List<EmpPage>();
            Task.Factory.StartNew(() =>
            {
                Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                {
                    if (currentPage != 0)
                        pages.Add(new EmpPage(window, mainWindow, mainWindow.empList.empManager[currentPage - 1]));
                    if (currentPage != mainWindow.empList.empManager.Count - 1)
                        pages.Add(new EmpPage(window, mainWindow, mainWindow.empList.empManager[currentPage + 1]));
                }));
            });
            if (currentPage == mainWindow.empList.empManager.Count - 1)
                Next.IsEnabled = false;
            
            Prev.IsEnabled = true;
        }

        private void Prev_Click(object sender, RoutedEventArgs e)
        {
            currentPage--;
            empPage.NavigationService.Navigate(pages[0]);
            pages = new List<EmpPage>();
            Task.Factory.StartNew(() =>
            {
                Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                {
                    if (currentPage != 0)
                        pages.Add(new EmpPage(window, mainWindow, mainWindow.empList.empManager[currentPage - 1]));
                    if (currentPage != mainWindow.empList.empManager.Count - 1)
                        pages.Add(new EmpPage(window, mainWindow, mainWindow.empList.empManager[currentPage + 1]));
                }));
            });

            if (currentPage == 0)
                Prev.IsEnabled = false;
            Next.IsEnabled = true;
        }

        private void Last_Click(object sender, RoutedEventArgs e)
        {
            currentPage = mainWindow.empList.empManager.Count - 1;
            pages = new List<EmpPage>();
            Task.Factory.StartNew(() =>
            {
                Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                {
                    if (currentPage != 0)
                        pages.Add(new EmpPage(window, mainWindow, mainWindow.empList.empManager[currentPage - 1]));
                }));
            });
            empPage.NavigationService.Navigate( lastPages[1]);

            if (currentPage != 0)
                Prev.IsEnabled = true;
            else
                Prev.IsEnabled = false;
            Next.IsEnabled = false;
        }
    }
}
