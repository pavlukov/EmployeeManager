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
using Library;
using System.Windows.Threading;
using EmployeeManager.SDK;

namespace plug
{
    public partial class MainWindow : Window, IPlugin
    {
        DispatcherTimer timer;
        public MainWindow()
        {
            InitializeComponent();

            Loaded();
        }


        public void Loaded()
        {
            setClock();
            this.Show();
        }

        public void setClock()
        {
            string hour;
            string minute;
            string second;
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1.0);
            timer.Start();
            timer.Tick += new EventHandler(delegate (object s, EventArgs a)
            {
                if(DateTime.Now.Hour >=0 && DateTime.Now.Hour <=9)
                    hour = "0" + DateTime.Now.Hour;
                else
                    hour = Convert.ToString(DateTime.Now.Hour);

                if (DateTime.Now.Minute >= 0 && DateTime.Now.Minute <= 9)
                    minute = "0" + DateTime.Now.Minute;
                else
                    minute = Convert.ToString(DateTime.Now.Minute);

                if (DateTime.Now.Second >= 0 && DateTime.Now.Second <= 9)
                    second = "0" + DateTime.Now.Second;
                else
                    second = Convert.ToString(DateTime.Now.Second);

                tb.Text = "" + hour + ":"
               + minute + ":"
               + second;
            });
        }
    }
}
