using System;
using System.Diagnostics;
using System.Reflection;
using System.Threading.Tasks;
using TestDll;

namespace MyNUnit
{
    /// <summary>
    /// 
    /// </summary>
    public static class RunTestsInAssembly
    {
        /// <summary>
        /// Метод, который запускает aннотации.
        /// </summary>
        private static void RunAnnotationsForAllTests(Assembly assembly, Type currentType)
        {
            RunMethodsWithAnnotationBeforeClass(assembly, currentType);
            foreach (MethodInfo mInfo in currentType.GetMethods())
            {
                Attribute attribute = FindTestAttribute(mInfo);
                if (attribute != null)
                {
                    string testName = mInfo.Name;
                    string message = "";

                    Object run = Activator.CreateInstance(currentType);
                    RunMethodsWithAnnotationBefore(assembly, currentType, run);

                    var print = new PrintInformationAboutTests();

                    var a = (TestAttribute)attribute;
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
                            watch.Stop();
                        }
                        catch (Exception e)
                        {
                            watch.Stop();
                            var exceptionType = e.InnerException;
                            if (exceptionType != a.Expected)
                            {
                                message = $"thrown the {exceptionType.ToString()}. Exception message is: {e.Message}";
                            }
                            else
                            {
                                if (a.Expected == null)
                                {
                                    message = "Error. The exception that was caught is not correct.";
                                }
                                else
                                {
                                    message = "Successfully.";
                                }
                            }
                        }

                        if (a.Expected != null)
                        {
                            message = $"Failed. The test hasn't thrown the {a.Expected.ToString()}";
                        }

                        TimeSpan ts = watch.Elapsed;
                        print.PrintInformation(ts, message, testName);
                    }
                    RunMethodsWithAnnotationAfter(assembly, currentType, run);
                }
                RunMethodsWithAnnotationAfterClass(assembly, currentType);
            }
        }

        /// <summary>
        /// Метод, который запускает aннотации для конкретного теста.
        /// </summary>
        private static (TimeSpan, string, string) RunAnnotationsForOneTest(Assembly assembly, Type currentType, string testNameWeNeed)
        {
            var flag = false;
            string message = "";
            RunMethodsWithAnnotationBeforeClass(assembly, currentType);
            foreach (MethodInfo mInfo in currentType.GetMethods())
            {
                string testName = mInfo.Name;
                if (testName == testNameWeNeed)
                {
                    flag = true;
                    Attribute attribute = FindTestAttribute(mInfo);
                    if (attribute != null)
                    {
                        Object run = Activator.CreateInstance(currentType);
                        RunMethodsWithAnnotationBefore(assembly, currentType, run);

                        var a = (TestAttribute)attribute;
                        if (a.MessageAboutIgnoreThisTest != "")
                        {
                            message = "was not done. " + a.MessageAboutIgnoreThisTest;
                            return(default(TimeSpan), message, testName);
                        }

                        if (message == "")
                        {
                            var watch = new Stopwatch();
                            watch.Start();
                            try
                            {
                                mInfo.Invoke(run, Array.Empty<Object>());
                                watch.Stop();
                            }
                            catch (Exception e)
                            {
                                watch.Stop();
                                var exceptionType = e.InnerException;
                                if (exceptionType != a.Expected)
                                {
                                    message = $"thrown the {exceptionType.ToString()}. Exception message is: {e.Message}";
                                }
                                else
                                {
                                    if (a.Expected == null)
                                    {
                                        message = "Error. The exception that was caught is not correct.";
                                    }
                                    else
                                    {
                                        message = "Successfully.";
                                    }
                                }
                            }

                            if (a.Expected != null)
                            {
                                message = $"Failed. The test hasn't thrown the {a.Expected.ToString()}";
                            }

                            TimeSpan ts = watch.Elapsed;
                            return(ts, message, testName);
                        }
                        RunMethodsWithAnnotationAfter(assembly, currentType, run);
                    }
                }
                RunMethodsWithAnnotationAfterClass(assembly, currentType);
            }
            message = "Failed.";
            return(default(TimeSpan), message, testNameWeNeed);
        }

        /// <summary>
        /// Метод, который запускает тесты в сборке.
        /// </summary>
        public static string RunTests(Assembly assembly)
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
            Parallel.ForEach(allTypes, (currentType) =>
                    {
                        RunAnnotationsForAllTests(assembly, currentType);
                    });
            return "0";
        }

        /// <summary>
        /// Метод, который запускает конкретный тест в сборке.
        /// </summary>
        public static (TimeSpan, string, string) RunOneTest(Assembly assembly, string testNameWeNeed)
        {
            Type[] allTypes;
            try
            {
                allTypes = assembly.GetTypes();
            }
            catch (ReflectionTypeLoadException ex)
            {
                return (default(TimeSpan), "Возникли проблемы со сборкой: ", ex.Message);
            }
            var flag = false;
            var timeForReturn = default(TimeSpan);
            var messageForReturn = "";
            Parallel.ForEach(allTypes, (currentType) =>
            {
                var (time, message, testName) = RunAnnotationsForOneTest(assembly, currentType, testNameWeNeed);
                if (message != "Failed.")
                {
                    flag = true;
                    messageForReturn = message;
                    timeForReturn = time;
                }
            });
            if (!flag)
            { 
                return (default(TimeSpan), "Failed.", testNameWeNeed);
            }
            else
            {
                return (timeForReturn, messageForReturn, testNameWeNeed);
            }
        }

        /// <summary>
        /// Метод, который ищет TestAttribute  у метода.
        /// </summary>
        public static Attribute FindTestAttribute(MethodInfo mInfo)
        {
            foreach (var attribute in Attribute.GetCustomAttributes(mInfo))
            {
                if (attribute.GetType().FullName == typeof(TestAttribute).FullName)
                {
                    return attribute;
                }
            }
            return null;
        }

        /// <summary>
        /// Метод, который запускает методы с указанной анотацией.
        /// </summary>
        public static void RunMethodsWithAnnotationBefore(Assembly assembly, Type type, Object run)
        {
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
        public static void RunMethodsWithAnnotationAfter(Assembly assembly, Type type, Object run)
        {
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
        public static void RunMethodsWithAnnotationBeforeClass(Assembly assembly, Type type)
        {
            var allTypes = assembly.GetTypes();
            foreach (MethodInfo mInfo in type.GetMethods())
            {
                if (Attribute.GetCustomAttributes(mInfo).GetType() == typeof(BeforeClassAttribute))
                {
                    mInfo.Invoke(null, Array.Empty<Object>());
                }
            }
        }

        /// <summary>
        /// Метод, который запускает методы с указанной анотацией.
        /// </summary>
        public static void RunMethodsWithAnnotationAfterClass(Assembly assembly, Type type)
        {
            var allTypes = assembly.GetTypes();
            foreach (MethodInfo mInfo in type.GetMethods())
            {
                if (Attribute.GetCustomAttributes(mInfo).GetType() == typeof(AfterClassAttribute))
                {
                    mInfo.Invoke(null, Array.Empty<Object>());
                }
            }
        }
    }
}
