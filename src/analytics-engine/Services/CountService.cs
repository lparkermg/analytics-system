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

            var timer = new Timer();
            timer.Elapsed += (a, ctx) => SaveCountsFile();
            timer.Interval = 1000;
            timer.AutoReset = true;
            timer.Start();
        }
        
        public void Increment()
        {
            _currentCount++;
        }

        public int Get() => _currentCount;

        public Dictionary<string, int> GetAll()
        {
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
            if (_currentDate == Clock.Now.ToString("dd-MM-yyyy"))
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
            _currentDate = Clock.Now.ToString("dd-MM-yyyy");
        }
    }
}