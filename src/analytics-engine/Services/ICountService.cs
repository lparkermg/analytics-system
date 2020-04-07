using System.Collections.Generic;

namespace analytics_engine.Services
{
    public interface ICountService
    {
        void Increment();
        int Get();
        Dictionary<string, int> GetAll();
    }
}
