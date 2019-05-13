﻿using System;
using System.Diagnostics;
using System.Reflection;
using TestDll;

namespace MyNUnit
{
    public class RunTestsInAssembly
    {
        /// <summary>
        /// Метод, который запускает тесты в сборке.
        /// </summary>
        public string RunTests(Assembly assembly)
        {
            Type[] allTypes;
            try
            {
                allTypes = assembly.GetTypes();
            }
            catch (ReflectionTypeLoadException ex)
            {
                return "For assembly " + assembly + "was finding error " + ex.Message;
            }
            foreach (Type type in allTypes) //где-то здесь параллельность
            {
                Object run = Activator.CreateInstance(type);
                RunMethodsWithAnnotationBeforeClass(assembly, type, run);
                foreach (MethodInfo mInfo in type.GetMethods())
                {
                    Attribute attribute = FindTestAttribute(mInfo);
                    if (attribute != null)
                    {
                        string testName = mInfo.Name;
                        string message = "";

                        RunMethodsWithAnnotationBefore(assembly, type, run); 

                        var print = new PrintInformationAboutTests();

                        TestAttribute a = (TestAttribute)attribute;
                        if (a.MessageAboutIgnoreThisTest != "")
                        {
                            message = "was not done. " + a.MessageAboutIgnoreThisTest;
                            print.PrintInformation(default(TimeSpan), testName, message);
                        }
                         
                        if (message == "")
                        { 
                            var watch = new Stopwatch();
                            watch.Start();
                            try
                            {
                                mInfo.Invoke(run, Array.Empty<Object>());
                                message = "was successful.";
                                watch.Stop();
                            }
                            catch (Exception e)
                            {
                                watch.Stop();
                                var exceptionType = e.InnerException;
                                if (exceptionType !=  a.Excepted)
                                {
                                    message = $"thrown the {exceptionType.ToString()}. Exception message is: {e.Message}";
                                }
                                else
                                {
                                    message = "was successful";
                                }
                            }
                            TimeSpan ts = watch.Elapsed;
                            print.PrintInformation(ts, message, testName);
                        }
                        RunMethodsWithAnnotationAfter(assembly, type, run);
                    }
                    RunMethodsWithAnnotationAfterClass(assembly, type, run);
                }
            }
            return "0";
        }

        /// <summary>
        /// Метод, который ищет TestAttribute  у метода.
        /// </summary>
        public Attribute FindTestAttribute(MethodInfo mInfo)
        {
            foreach (var attribute in Attribute.GetCustomAttributes(mInfo))
            {
                if (attribute.GetType().FullName == typeof(TestAttribute).FullName)
                {
                    //TestAttribute a = (TestAttribute)attribute;
                    return attribute;
                }
            }
            return null;
        }

        /// <summary>
        /// Метод, который запускает методы с указанной анотацией.
        /// </summary>
        public void RunMethodsWithAnnotationBefore(Assembly assembly, Type type, Object run)
        {
            var allTypes = assembly.GetTypes();
            foreach (MethodInfo mInfo in type.GetMethods())
            {
                if (Attribute.GetCustomAttributes(mInfo).GetType() == typeof(BeforeAttribute))
                {
                    mInfo.Invoke(run, Array.Empty<Object>());
                }
            }
        }

        /// <summary>
        /// Метод, который запускает методы с указанной анотацией.
        /// </summary>
        public void RunMethodsWithAnnotationAfter(Assembly assembly, Type type, Object run)
        {
            var allTypes = assembly.GetTypes();
            foreach (MethodInfo mInfo in type.GetMethods())
            {
                if (Attribute.GetCustomAttributes(mInfo).GetType() == typeof(AfterAttribute))
                {
                    mInfo.Invoke(run, Array.Empty<Object>());
                }
            }
        }

        /// <summary>
        /// Метод, который запускает методы с указанной анотацией.
        /// </summary>
        public void RunMethodsWithAnnotationBeforeClass(Assembly assembly, Type type, Object run)
        {
            var allTypes = assembly.GetTypes();
            foreach (MethodInfo mInfo in type.GetMethods())
            {
                if (Attribute.GetCustomAttributes(mInfo).GetType() == typeof(BeforeClassAttribute))
                {
                    mInfo.Invoke(run, Array.Empty<Object>());
                }
            }
        }

        /// <summary>
        /// Метод, который запускает методы с указанной анотацией.
        /// </summary>
        public void RunMethodsWithAnnotationAfterClass(Assembly assembly, Type type, Object run)
        {
            var allTypes = assembly.GetTypes();
            foreach (MethodInfo mInfo in type.GetMethods())
            {
                if (Attribute.GetCustomAttributes(mInfo).GetType() == typeof(AfterClassAttribute))
                {
                    mInfo.Invoke(run, Array.Empty<Object>());
                }
            }
        }
    }
}
