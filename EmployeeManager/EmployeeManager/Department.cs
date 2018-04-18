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
    [DataContract]
    public class Department
    {
        public Department()
        {
            ClassName = "Department";
            BossIndex = -1;
        }
        [DataMember]
        public string ClassName { get; set; }
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public int Year { get; set; }
        [DataMember]
        public string Activity { get; set; }
        public Employee boss;
        [DataMember]
        public int BossIndex { get; set; }

        public void Add(DepartmentWindow window)
        {
            if (window.depName.Text != "")
                Name = window.depName.Text;
            else
                Name = "Пусто";

            if(window.depYear.SelectedItem != null)
                Year = int.Parse(window.depYear.Text);

            Activity = window.depActivity.Text;

            if (window.depBoss.SelectedItem != null)
            {
                boss = (Employee)window.depBoss.SelectedItem;
                boss.dep = this;
                BossIndex = window.empList.empManager.IndexOf(boss);
            }      
        }
    }
}
