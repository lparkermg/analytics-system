using analytics_engine.Services;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace analytics_engine.tests
{
    [TestFixture]
    internal sealed class FileBasedCountServiceTests
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

        [TestCase(null)]
        [TestCase("")]
        [TestCase("          ")]
        public void Increment_GivenNoUrl_ShouldThrowArgumentException(string noUrl) =>
            Assert.That(() => _counter.Increment(noUrl), Throws.ArgumentException.With.Message.EqualTo("Url cannot be null."));

        [TestCase("/test/path")]
        [TestCase("/another/test/path")]
        public void Increment_GivenUrl_OnGet_ShouldReturnTheCorrectData(string path)
        {
            _counter.Increment(path);
            var result = _counter.Get();

            Assert.Multiple(() =>
            {
                Assert.That(result.ContainsKey(path));
                Assert.That(result, Has.One.Items);
                Assert.That(result[path], Is.EqualTo(1));
            });
        }

        [Test]
        public void Get_WhenIncrementWithAUrl_ShouldReturnUrlAndCount()
        {
            _counter.Increment("/test/path");
            var result = _counter.Get();
            Assert.Multiple(() =>
            {
                Assert.That(result.ContainsKey("/test/path"));
                Assert.That(result["/test/path"], Is.EqualTo(1));
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