using System.Collections.Generic;
namespace Test.Search.Interfaces
{
    public interface IStorage<T>
    {
        public void Create(T newInstantce);
        public void Update(T instatceToUpdate);
        public void Delete(int ID);
        public IEnumerable<T> Get();
        public T Get(int ID);
    }
}

