using analytics_engine.Services;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading;

namespace analytics_engine.tests
{
    [TestFixture]
    public class FileBasedCountServiceTests
    {
        private FileBasedCountService _counter;

        [SetUp]
        public void Setup()
        {
            _counter = new FileBasedCountService("./counts.test");
        }

        [TearDown]
        public void Teardown()
        {
            var filesToDelete = Directory.GetFiles("./counts.test", "*.json");

            foreach(var file in filesToDelete)
            {
                File.Delete(file);
            }

            _counter = null;
        }

        [Test]
        public void Get_WhenIncrementWithAUrl_ShouldReturnUrlAndCount()
        {
            _counter.Increment("/test/path");
            var result = _counter.Get();
            Assert.Multiple(() =>
            {
                Assert.IsTrue(result.ContainsKey("/test/path"));
                Assert.AreEqual(1, result["/test/path"]);
            });
        }

        [Test]
        public void Get_WhenTwoIncrementCallsWithTheSameUrlAndAnotherOnANewDay_ShouldReturnTheSecondDay() 
        {
            _counter.Increment("/test/path/1");
            _counter.Increment("/test/path/2");
            Clock.Initialize(() => DateTime.Now.AddDays(1));
            _counter.Increment("/test/path/3");
            var result = _counter.Get();

            Assert.Multiple(() => {
                Assert.IsTrue(result.ContainsKey("/test/path/3"));
                Assert.AreEqual(1, result["/test/path/3"]);
            });
        }

        [Test]
        public void Get_WhenTwoIncrementCallsWithTheSameUrlAndAnotherOnANewDay_ShouldProduceTwoJsonFiles()
        {
            _counter.Increment("/test/path/1");
            _counter.Increment("/test/path/2");
            Clock.Initialize(() => DateTime.Now.AddDays(1));
            _counter.Increment("/test/path/3");
            Clock.Initialize(() => DateTime.Now.AddDays(2));
            _counter.Get();

            var files = Directory.GetFiles("./counts.test", "*.json");
            Assert.AreEqual(2, files.Length);

        }

        [Test]
        public void GetAll_WhenAnIncrementCallAndAnotherOnANewDay_ShouldReturnTheCorrectContent()
        {
            var expectedResults = new Dictionary<string, Dictionary<string, int>>();
            expectedResults.Add(Clock.Now.ToString("dd-MM-yyyy"), new Dictionary<string, int>() { { "/test/path/1", 1 } });
            expectedResults.Add(Clock.Now.AddDays(1).ToString("dd-MM-yyyy"), new Dictionary<string, int>() { { "/test/path/2", 1 } });

            _counter.Increment("/test/path/1");
            Clock.Initialize(() => DateTime.Now.AddDays(1));
            _counter.Increment("/test/path/2");
            Clock.Initialize(() => DateTime.Now.AddDays(2));
            var results = _counter.GetAll();

            var expectedDeserialized = JsonSerializer.Serialize(expectedResults);
            var actualDeserialized = JsonSerializer.Serialize(results);
            Assert.AreEqual(expectedDeserialized, actualDeserialized);
        }
    }
}