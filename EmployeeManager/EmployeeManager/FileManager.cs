using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace EmployeeManager
{
    public static class FileManager
    {
        public static void Save(EmployeeList empList, DepartmentList depList)
        {
            DataContractJsonSerializer JsonFormatterEmp = new DataContractJsonSerializer(typeof(List<Employee>));
            DataContractJsonSerializer JsonFormatterDep = new DataContractJsonSerializer(typeof(List<Department>));


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

        public static void Load(EmployeeList empList, DepartmentList depList)
        {
            DataContractJsonSerializer JsonFormatterEmp = new DataContractJsonSerializer(typeof(List<Employee>));
            DataContractJsonSerializer JsonFormatterDep = new DataContractJsonSerializer(typeof(List<Department>));

            try
            {
                Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
                dlg.Filter = "*.ZIP | *.zip";
                string fileName = "";
                int index = 0;

                bool? result = dlg.ShowDialog();
                if (result == true)
                {
                    fileName = dlg.FileName.Remove(dlg.FileName.Length - 4, 4);
                    index = fileName.LastIndexOf(@"\");
                }

                string empListPath = fileName + fileName.Remove(0, index) + "(Employee).json";
                string depListPath = fileName + fileName.Remove(0, index) + "(Department).json";

                ZipFile.ExtractToDirectory(fileName + ".zip", fileName);

                using (FileStream fs = new FileStream(empListPath, FileMode.Open))
                {
                    if (fs.Length > 0)
                    {
                        empList.empManager = (List<Employee>)JsonFormatterEmp.ReadObject(fs);
                    }
                }

                using (FileStream fs = new FileStream(depListPath, FileMode.Open))
                {
                    if (fs.Length > 0)
                    {
                        depList.depManager = (List<Department>)JsonFormatterDep.ReadObject(fs);
                    }
                }

                foreach (Employee item in empList.empManager)
                {
                    if (item.BossIndex != -1)
                    {
                        item.boss = empList.empManager[item.BossIndex];
                        item.boss.subWorker = new List<Employee>();
                        item.boss.subWorker.Add(item);
                    }

                    if (item.DepIndex != -1)
                        item.dep = depList.depManager[item.DepIndex];

                    if (item.SubWorkerIndex != null)
                    {
                        foreach (int i in item.SubWorkerIndex)
                            item.subWorker.Add(empList.empManager[i]);

                        foreach (Employee sub in item.subWorker)
                            sub.boss = item;
                    }

                }

                foreach (Department item in depList.depManager)
                {
                    if (item.BossIndex != -1)
                    {
                        item.boss = empList.empManager[item.BossIndex];
                        item.boss.dep = item;
                    }
                }

                Directory.Delete(fileName, true);
            }
            catch
            {
                MessageBox.Show("Ошибка при открытии файла!");
            }
        }
    }
}
