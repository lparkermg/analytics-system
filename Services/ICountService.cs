using System.Collections.Generic;

namespace Services
{
    public interface ICountService
    {
        void Increment();
        int Get();
        Dictionary<string, int> GetAll();
    }
}
