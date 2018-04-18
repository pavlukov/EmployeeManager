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
using System.Windows.Shapes;

namespace EmployeeManager
{

    public partial class ShowDep : Window
    {
        Department dep;
        Window window;
        MainWindow mainWindow;
        bool showMain;
        List<DepPage> pages;
        List<DepPage> lastPages = new List<DepPage>();
        int currentPage;

        public ShowDep(Window _window, MainWindow _mainWindow, Department _dep)
        {
            InitializeComponent();

            dep = _dep;
            window = _window;
            mainWindow = _mainWindow;
            showMain = true;

            pages = new List<DepPage>();
            currentPage = mainWindow.depList.depManager.IndexOf(dep);
            Task.Factory.StartNew(() =>
            {
                Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                {
                    lastPages.Add(new DepPage(window, mainWindow, mainWindow.depList.depManager[0]));
                    lastPages.Add(new DepPage(window, mainWindow, mainWindow.depList.depManager[mainWindow.depList.depManager.Count - 1]));
                    if (currentPage != 0)
                        pages.Add(new DepPage(window, mainWindow, mainWindow.depList.depManager[currentPage - 1]));
                    if (currentPage != mainWindow.depList.depManager.Count - 1)
                        pages.Add(new DepPage(window, mainWindow, mainWindow.depList.depManager[currentPage + 1]));
                }));
            });
            depPage.NavigationService.Navigate(new DepPage(window, mainWindow, dep));

            if (mainWindow.depList.depManager.Count == 1)
                Next.IsEnabled = false;
            if (currentPage == mainWindow.depList.depManager.Count - 1)
                Next.IsEnabled = false;
            if (currentPage == 0)
                Prev.IsEnabled = false;
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            if (showMain == true)
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

            pages = new List<DepPage>();
            Task.Factory.StartNew(() =>
            {
                Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                {
                    if (currentPage != mainWindow.depList.depManager.Count - 1)
                        pages.Add(new DepPage(window, mainWindow, mainWindow.depList.depManager[currentPage + 1]));
                }));
            });
            depPage.NavigationService.Navigate(lastPages[0]);

            if (currentPage != pages.Count - 1)
                Next.IsEnabled = true;
            else
                Next.IsEnabled = false;
            Prev.IsEnabled = false;
        }

        private void Next_Click(object sender, RoutedEventArgs e)
        {
            currentPage++;
            if (pages.Count > 1)
                depPage.NavigationService.Navigate(pages[1]);
            else
                depPage.NavigationService.Navigate(pages[0]);
            pages = new List<DepPage>();
            Task.Factory.StartNew(() =>
            {
                Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                {
                    if (currentPage != 0)
                        pages.Add(new DepPage(window, mainWindow, mainWindow.depList.depManager[currentPage - 1]));
                    if (currentPage != mainWindow.depList.depManager.Count - 1)
                        pages.Add(new DepPage(window, mainWindow, mainWindow.depList.depManager[currentPage + 1]));
                }));
            });
            if (currentPage == mainWindow.depList.depManager.Count - 1)
                Next.IsEnabled = false;
            Prev.IsEnabled = true;
        }

        private void Prev_Click(object sender, RoutedEventArgs e)
        {
            currentPage--;
            depPage.NavigationService.Navigate(pages[0]);
            pages = new List<DepPage>();
            Task.Factory.StartNew(() =>
            {
                Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                {
                    if (currentPage != 0)
                        pages.Add(new DepPage(window, mainWindow, mainWindow.depList.depManager[currentPage - 1]));
                    if (currentPage != mainWindow.depList.depManager.Count - 1)
                        pages.Add(new DepPage(window, mainWindow, mainWindow.depList.depManager[currentPage + 1]));
                }));
            });
            if (currentPage == 0)
                Prev.IsEnabled = false;
            Next.IsEnabled = true;
        }

        private void Last_Click(object sender, RoutedEventArgs e)
        {
            currentPage = mainWindow.depList.depManager.Count - 1;
            pages = new List<DepPage>();
            Task.Factory.StartNew(() =>
            {
                Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                {
                    if (currentPage != 0)
                        pages.Add(new DepPage(window, mainWindow, mainWindow.depList.depManager[currentPage - 1]));
                }));
            });
            depPage.NavigationService.Navigate(lastPages[1]);

            if (currentPage != 0)
                Prev.IsEnabled = true;
            else
                Prev.IsEnabled = false;
            Next.IsEnabled = false;
        }
    }
}
