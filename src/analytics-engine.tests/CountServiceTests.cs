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
    public class CountServiceTests
    {
        private CountService _counter;

        [SetUp]
        public void Setup()
        {
            _counter = new CountService("./counts.test");
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
        [Ignore("To be deleted upon full implementation of url handling.")]
        public void Given_One_Increment_Call_When_Get_Should_Return_One_Count()
        {
            _counter.Increment("");
            Assert.AreEqual(1, _counter.Get());
        }

        [Test]
        [Ignore("To be deleted upon full implementation of url handling.")]
        public void Given_Two_Increment_Calls_And_Another_On_A_New_Day_When_Get_Should_Return_One_Count()
        {
            _counter.Increment("");
            _counter.Increment("");
            Clock.Initialize(() => DateTime.Now.AddDays(1));
            _counter.Increment("");

            Assert.AreEqual(1, _counter.Get());
        }

        [Test]
        [Ignore("To be deleted up full implementation of url handling.")]
        public void Given_Two_Increment_Calls_And_Another_On_A_New_Day_Then_Progress_To_A_New_Day_And_Get_A_Count_Should_Produce_Two_Files()
        {
            _counter.Increment("");
            _counter.Increment("");
            Clock.Initialize(() => DateTime.Now.AddDays(1));
            _counter.Increment("");
            Clock.Initialize(() => DateTime.Now.AddDays(2));
            _counter.Get();

            var files = Directory.GetFiles("./counts.test", "*.txt");
            Assert.AreEqual(2, files.Length);
        }

        [Test]
        public void Given_One_Increment_Call_With_A_Url_When_Get_Return_Url_And_Count()
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
        public void Given_Two_Increment_Calls_With_The_Same_Url_And_Another_On_A_New_Day_When_Get_Should_Return_The_Second_Day() 
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
        public void Given_Two_Increment_Calls_With_The_Same_Url_And_Another_On_A_New_Day_Should_Produce_Two_Json_Files()
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
        public void Given_An_Increment_Call_And_Another_On_A_New_Day_When_GetAll_Should_Return_The_Correct_Content()
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