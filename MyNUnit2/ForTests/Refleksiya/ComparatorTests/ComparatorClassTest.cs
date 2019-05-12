using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestDll;

namespace CompClass
{
    [TestClass]
    public class ComparatorClassTest
    {
        class A
        {
            public void Foo()
            {

            }
        }

        class B
        {
            public void Foo()
            {

            }

            public void Boo()
            {

            }
        }

        [Test]
        [TestMethod]
        public void ComparedTwoClasses()
        {
            var listTestMethods = new List<string>();
            listTestMethods.Add("Void Boo()");
            var listTestFields = new List<string>();
            var test = new ClassComparator();
            var a = typeof(A);
            var b = typeof(B);
            test.GetClasses(a, b);
            test.GetMethods();
            var list1 = test.GetDiffMethods();
            var list2 = test.GetDiffFields();
            foreach (var i in list1)
            {
                Assert.IsTrue(listTestMethods.Contains(i));
            }

            foreach (var f in list2)
            {
                Assert.IsTrue(listTestFields.Contains(f));
            }
        }
    }
}
