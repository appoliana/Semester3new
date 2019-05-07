using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace OneThreadLazy
{
    [TestClass]
    public class OneThread
    {
        [TestMethod]
        public void GetShouldReturnTheSameObject()
        {
            string testString = "hello";
            Func<string> supplier = () => testString;

            var lazy = Lazy.LazyFactory.CreateSingleThreadedLazy(supplier); 
            var obj1 = lazy.Get();
            var obj2 = lazy.Get();

            Assert.AreSame(obj1, obj2);
        }

        [TestMethod]
        public void SupplierShouldBeCalculatedOnce()
        {
            int counter = 0;
            var supplier = new Func<int>(() =>
            {
                counter++;
                return counter;
            });

            var lazy = Lazy.LazyFactory.CreateSingleThreadedLazy(supplier);
            var obj1 = lazy.Get();
            var obj2 = lazy.Get();

            Assert.AreEqual(1, counter);
        }
    }
}
