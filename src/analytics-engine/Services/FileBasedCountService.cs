using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Timers;

namespace analytics_engine.Services
{
    public sealed class FileBasedCountService : ICountService
    {
        private string _countFileBase;
        private Dictionary<string, int> _currentCounts = new Dictionary<string, int>();
        private string _currentDate;

        public FileBasedCountService(string countFileBase)
        {
            _countFileBase = countFileBase;
            _currentDate = Clock.Now.ToString("dd-MM-yyyy");

            if (!File.Exists($"{countFileBase}/ProcessedOld") && Directory.Exists(countFileBase))
            {
                var oldFiles = Directory.GetFiles(_countFileBase, "*.txt");
                foreach (var file in oldFiles)
                {
                    var data = File.ReadAllText(file);
                    var newData = new Dictionary<string, int>() { { "/", int.Parse(data) } };
                    var newSerializeData = JsonSerializer.Serialize(newData);
                    File.WriteAllText($"{countFileBase}/{Path.GetFileNameWithoutExtension(file)}.json", newSerializeData);
                }
                File.WriteAllText($"{ countFileBase}/ProcessedOld", "");
            }
        }
        
        public void Increment(string url)
        {
            SaveCountsFile();

            if (_currentCounts.ContainsKey(url))
            {
                _currentCounts[url]++;
            }
            else
            {
                _currentCounts.Add(url, 1);
            }
        }

        public Dictionary<string, int> Get()
        {
            SaveCountsFile();
            return _currentCounts;
        }

        public Dictionary<string, Dictionary<string, int>> GetAll()
        {
            SaveCountsFile();

            var dataFiles = Directory.GetFiles(_countFileBase, "*.json");
            var data = new Dictionary<string, Dictionary<string, int>>();
            foreach(var file in dataFiles)
            {
                var read = File.ReadAllText(file);
                var json = JsonSerializer.Deserialize<Dictionary<string, int>>(read);
                data.Add(Path.GetFileNameWithoutExtension(file), json);
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

            if (File.Exists($"{_countFileBase}/{Clock.Now.AddDays(-1):dd-MM-yyyy}.json"))
            {
                return;
            }

            var todaysJson = JsonSerializer.Serialize(_currentCounts);

            File.WriteAllText($"{_countFileBase}/{Clock.Now.AddDays(-1):dd-MM-yyyy}.json", todaysJson);

            _currentCounts = new Dictionary<string, int>();
            _currentDate = now;
        }
    }
}