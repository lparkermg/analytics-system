using System.Collections.Generic;

namespace analytics_engine.Services
{
    public interface ICountService
    {
        void Increment(string url);
        Dictionary<string, int> Get();
        Dictionary<string, Dictionary<string, int>> GetAll();
    }
}
