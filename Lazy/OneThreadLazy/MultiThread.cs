using System;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace OneThreadLazy
{
    [TestClass]
    public class MultiThread
    {
            const int threadsCount = 1000;
            Thread[] threads = new Thread[threadsCount];

            [TestMethod]
            public void MultiThreadGetShouldReturnTheSameObject()
            {
                string testString = "hello";
                Func<string> supplier = () => testString;

                var lazy = Lazy.LazyFactory.CreateSingleThreadedLazy(supplier);
                for (int i = 0; i < threadsCount; ++i)
                {
                    threads[i] = new Thread(() =>
                    {
                        for (int j = 0; j < 10; ++j)
                        {
                            Assert.AreSame(testString, lazy.Get());
                        }
                    });
                }

                foreach (var thread in threads)
                {
                    thread.Start();
                }

                foreach (var thread in threads)
                {
                    thread.Join();
                }
            }

            [TestMethod]
            public void MultiTreadSupplierShouldBeCalculatedOnce()
            {
                int counter = 0;
                var supplier = new Func<int>(() =>
                {
                    counter++;
                    return counter;
                });

                var lazy = Lazy.LazyFactory.CreateSingleThreadedLazy(supplier);
                for (int i = 0; i < threadsCount; ++i)
                {
                    threads[i] = new Thread(() =>
                    {
                        for (int j = 0; j < 10; ++j)
                        {
                            Assert.AreEqual(2, lazy.Get());
                        }
                    });
                }

                foreach (var thread in threads)
                {
                    thread.Start();
                }

                Thread.Sleep(200);

                foreach (var thread in threads)
                {
                    thread.Join();
                }

                Assert.AreEqual(2, counter);
            }
        }
    }
