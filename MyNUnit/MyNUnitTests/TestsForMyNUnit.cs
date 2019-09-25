using System;
using System.IO;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MyNUnit;

namespace MyNUnitTests
{
    [TestClass]
    public class TestsForMyNUnit
    {
        private string path;

        private void CheckFileExistance(string path)
        {
            if (File.Exists(path))
            {
                File.Delete(path);
                Assert.Fail("Smth`s wrong with order of called methods");
            }
        }

        [TestInitialize]
        public void Init()
        {
            path = Assembly.GetExecutingAssembly().Location;
            path = path.Substring(0, path.IndexOf("MyNUnitTests\\bin\\Debug\\MyNUnitTests.dll")) + "ForTests";
        }

        [TestMethod]
        public void CheckTheRigthNumberOfAssemblies()
        {
            var (listOfAssemblies, message) = AssembliesGetter.GetAll(path);
            Assert.AreEqual(3, listOfAssemblies.Count);
        }

        [TestMethod]
        public void CheckRunTestOnWithAttrParams()
        {
            path = path + "\\Refleksiya\\ComparatorTests\\bin\\Debug";
            bool isAttribute = false;
            var (listOfAssemblies, message) = AssembliesGetter.GetAll(path);
            foreach (var i in listOfAssemblies)
            {
                var allTypes = i.GetTypes();
                
                foreach (Type type in allTypes)
                {
                    foreach (MethodInfo mInfo in type.GetMethods())
                    {
                        var myNUnit = RunTestsInAssembly.FindTestAttribute(mInfo);
                        if (myNUnit != null)
                        {
                            isAttribute = true;
                        }
                    }
                }    
            }
            Assert.IsTrue(isAttribute);
        }


        [TestMethod]
        public void IfWeAreWaitingTheExceptionButDoNotCatchIt()
        {
            path = path + "\\Refleksiya\\ComparatorTests\\bin\\Debug";
            var (listOfAssemblies, message) = AssembliesGetter.GetAll(path);
            foreach (var i in listOfAssemblies)
            {
                var (time, messageAboutTest, testName) = RunTestsInAssembly.RunOneTest(i, "TestForEmpty");
                Assert.AreEqual("Failed.", messageAboutTest);
            }
        }
    }
}

