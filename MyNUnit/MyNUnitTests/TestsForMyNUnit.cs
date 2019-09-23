﻿using System;
using System.IO;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MyNUnit;

namespace MyNUnitTests
{
    [TestClass]
    public class TestsForMyNUnit
    {
        private RunTestsInAssembly myNUnit;
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
            myNUnit = new RunTestsInAssembly();

            path = Assembly.GetExecutingAssembly().Location;
            path = path.Substring(0, path.IndexOf("MyNUnitTests\\bin\\Debug\\MyNUnitTests.dll")) + "ForTests";
        }

        [TestMethod]
        public void CheckTheRigthNumberOfAssemblies()
        {
            var getter = new GetAllAssemblies();
            var (listOfAssemblies, message) = getter.GetAll(path);
            Assert.AreEqual(80, listOfAssemblies.Count);
        }

        [TestMethod]
        public void CheckRunTestOnWithAttrParams()
        {
            path = path + "\\Refleksiya\\ComparatorTests\\bin\\Debug";
            var getter = new GetAllAssemblies();
            bool isAttribute = false;
            var (listOfAssemblies, message) = getter.GetAll(path);
            foreach (var i in listOfAssemblies)
            {
                var allTypes = new Type[10000];
                allTypes = i.GetTypes();
                

                foreach (Type type in allTypes)
                {
                    foreach (MethodInfo mInfo in type.GetMethods())
                    {
                        if (myNUnit.FindTestAttribute(mInfo) != null)
                        {
                            isAttribute = true;
                        }
                    }
                }    
            }
            Assert.IsTrue(isAttribute);
        }
    }
}

