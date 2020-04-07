using System;
using System.Collections.Generic;
using System.IO;
using System.Timers;

namespace analytics_engine.Services
{
    public class CountService : ICountService
    {
        private string _countFileBase;
        private int _currentCount = 0;
        private string _currentDate;

        public CountService(string countFileBase)
        {
            _countFileBase = countFileBase;
            _currentDate = Clock.Now.ToString("dd-MM-yyyy");
        }
        
        public void Increment()
        {
            SaveCountsFile();
            _currentCount++;
        }

        public int Get()
        {
            SaveCountsFile();
            return _currentCount;
        }

        public Dictionary<string, int> GetAll()
        {
            SaveCountsFile();

            var dataFiles = Directory.GetFiles(_countFileBase, "*.txt");
            var data = new Dictionary<string, int>();
            foreach(var file in dataFiles)
            {
                var read = File.ReadAllText(file);
                data.Add(Path.GetFileNameWithoutExtension(file),int.Parse(read));
            }

            return data;
        }

        private void SaveCountsFile()
        {
            var now = Clock.Now.ToString("dd-MM-yyyy");
            if (_currentDate == now)
            {
                return;
            }

            if (!Directory.Exists(_countFileBase))
            {
                Directory.CreateDirectory(_countFileBase);
            }

            if (File.Exists($"{_countFileBase}/{Clock.Now.AddDays(-1):dd-MM-yyyy}.txt"))
            {
                return;
            }

            File.WriteAllText($"{_countFileBase}/{Clock.Now.AddDays(-1):dd-MM-yyyy}.txt", _currentCount.ToString());

            _currentCount = 0;
            _currentDate = now;
        }
    }
}