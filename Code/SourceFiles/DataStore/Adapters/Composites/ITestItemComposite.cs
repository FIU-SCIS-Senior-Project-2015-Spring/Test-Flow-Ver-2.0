using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataStore.Adapters.Composites
{
    /// <summary>
    /// This composite pattern allows for the creation, editing, retrieving, and maping of test items
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface ITestItemComposite<T>
    {
    
        int Create(T item);

        bool Edit(T item);

        T Get(int id);

        List<T> GetFromParent(int parentId);
    }
}
