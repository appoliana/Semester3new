using System;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MyThreadPool.UnitTest
{
    [TestClass]
    public class MyThreadPoolTest
    {
        [TestMethod]
        public void CheckIfContinueWithWorks()
        {
            var quantity = 10;
            var myPool = new MyThreadPool(quantity);

            var task = myPool.AddJob(() => 6);
            var nextTask = task.ContinueWith(x => x / 2);

            Assert.AreEqual(6, task.Result);
            Assert.AreEqual(3, nextTask.Result);

            myPool.Shutdown();
        }

        [TestMethod]
        public void CheckIfExceptionDontStopEvaluation()
        {
            var quantity = 10;
            var myPool = new MyThreadPool(quantity);

            for (int i = 0; i <= 10; i++)
            {
                myPool.AddJob(() => 10 / (i - 5));
            }

            myPool.Shutdown();
        }

        [TestMethod]
        [ExpectedException(typeof(AggregateException))]
        public void CheckIfFallExceptionInContinueDontStopEvaluation()
        {
            int quantity = 10;
            var myPool = new MyThreadPool(quantity);

            var task = myPool.AddJob(() => "Hello world");
            var nextTask = task.ContinueWith<int>(i => throw new ArgumentException());
            var result = task.Result;
            var nextResult = nextTask.Result;
            myPool.Shutdown();
        }

        [TestMethod]
        [ExpectedException(typeof(AggregateException))]
        public void CheckIfAggregateExceptionFallInJob()
        {
            var quantity = 10;
            var myThreadPool = new MyThreadPool(quantity);
            var task = myThreadPool.AddJob(() => 20 / (quantity - 10));
            var result = task.Result;
        }


        [TestMethod]
        [ExpectedException(typeof(MyThreadPoolCanceledException))]
        public void CheckIfFallExceptionAfterShutdownAndAddTask()
        {
            var quantity = 4;
            var myPool = new MyThreadPool(quantity);

            myPool.Shutdown();

            for (var i = 0; i < quantity; i++)
            {
                var task = myPool.AddJob(() => 2 * i);
                Assert.AreEqual(2 * i, task.Result);
            }
        }
    }
}
