using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeManager
{
    public class DepartmentList : IEnumerable<Department>
    {
        public List<Department> depManager;

        public DepartmentList()
        {
            depManager = new List<Department>();
        }
        public IEnumerator<Department> GetEnumerator()
        {
            return new DepartmentListEnumerator(depManager);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        class DepartmentListEnumerator : IEnumerator<Department>
        {
            List<Department> _Manager;

            private Department _current;
            private int _index = -1;

            public DepartmentListEnumerator(List<Department> Manager)
            {
                _Manager = Manager;
            }

            public Department Current => _current;

            object IEnumerator.Current => Current;

            public void Dispose()
            {
            }

            public bool MoveNext()
            {
                _index++;
                if (_index >= _Manager.Count)
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
