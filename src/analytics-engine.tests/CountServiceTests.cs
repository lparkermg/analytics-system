using analytics_engine.Services;
using NUnit.Framework;
using System;
using System.IO;
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
            var filesToDelete = Directory.GetFiles("./counts.test", "*.txt");

            foreach(var file in filesToDelete)
            {
                File.Delete(file);
            }

            _counter = null;
        }

        [Test]
        public void Given_One_Increment_Call_When_Get_Should_Return_One_Count()
        {
            _counter.Increment();
            Assert.AreEqual(1, _counter.Get());
        }

        [Test]
        public void Given_Two_Increment_Calls_And_Another_On_A_New_Day_When_Get_Should_Return_One_Count()
        {
            _counter.Increment();
            _counter.Increment();
            Clock.Initialize(() => DateTime.Now.AddDays(1));
            _counter.Increment();

            Assert.AreEqual(1, _counter.Get());
        }

        [Test]
        public void Given_Two_Increment_Calls_And_Another_On_A_New_Day_Then_Progress_To_A_New_Day_And_Get_A_Count_Should_Produce_Two_Files()
        {
            _counter.Increment();
            _counter.Increment();
            Clock.Initialize(() => DateTime.Now.AddDays(1));
            _counter.Increment();
            Clock.Initialize(() => DateTime.Now.AddDays(2));
            _counter.Get();

            var files = Directory.GetFiles("./counts.test", "*.txt");
            Assert.AreEqual(2, files.Length);
        }
    }
}