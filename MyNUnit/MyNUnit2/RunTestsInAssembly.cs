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
        private static void RunAnnotations(Assembly assembly, Type currentType)
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
                        RunAnnotations(assembly, currentType);
                    });
            return "0";
        }

        /// <summary>
        /// Метод, который запускает конкретный тест по имени.
        /// </summary>
        public static string RunTest(Assembly assembly)
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
                RunAnnotations(assembly, currentType);
            });
            return "0";
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
