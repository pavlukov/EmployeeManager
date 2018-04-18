using System;
using System.Collections.Generic;
using System.Linq;
using EmployeeManager.SDK;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.IO;
using System.Windows.Forms;

namespace EmployeeManager
{
    public static class PluginInvoker
    {
        public static void InvokePlugin()
        {
            foreach (string file in Directory.GetFiles(Directory.GetCurrentDirectory() + @"\Data\Plugins"))
            {               
                string assemblyName = file.Remove(0, file.LastIndexOf(@"\") + 1);
                assemblyName = assemblyName.Remove(assemblyName.Length - 4, 4);

                if (assemblyName != "EmployeeManager.SDK")
                {
                    var pluginAssembly = Assembly.Load(assemblyName);

                    try
                    {
                        var pluginType = pluginAssembly.GetTypes().Where(t => typeof(IPlugin).IsAssignableFrom(t)).Single();
                        IPlugin plugin = (IPlugin)pluginType.GetConstructor(new Type[0]).Invoke(null);
                        plugin.Loaded();
                    }
                    catch
                    {
                        MessageBox.Show( assemblyName + " не загружен, т.к. не реализует заданный интерфейс.");
                    }
                }
            }          
        }
    }
}
