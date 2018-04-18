using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeManager
{
    public class EmployeeList : IEnumerable<Employee>
    {
        public List<Employee> empManager;

        public EmployeeList()
        {
            empManager = new List<Employee>();
        }
        public IEnumerator<Employee> GetEnumerator()
        {
            return new EmployeeListEnumerator(empManager);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        class EmployeeListEnumerator : IEnumerator<Employee>
        {
            List<Employee> _Manager;

            private Employee _current;
            private int _index = -1;

            public EmployeeListEnumerator(List<Employee> Manager)
            {
                _Manager = Manager;
            }

            public Employee Current => _current;
            
            object IEnumerator.Current => Current;

            public void Dispose()
            {
            }

            public bool MoveNext()
            {
                _index++;
                if(_index >= _Manager.Count)
                {
                    _current = null;
                    return false;
                }

                _current = _Manager[_index];
                return true;
            }

            public void Reset()
            {
                _index = -1;
            }
        }
    }
}
