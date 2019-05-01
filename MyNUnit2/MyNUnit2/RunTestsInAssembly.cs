using System;
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
        /// <param name="assembly"></param>
        /// <returns></returns>
        public string RunTests(Assembly assembly)
        {
            var allTypes = new Type[10000];
            try
            {
                allTypes = assembly.GetTypes();
            }
            catch (ReflectionTypeLoadException ex)
            {
                return "For assembly " + assembly + "was finding error " + ex.Message;
            }
            foreach (Type type in allTypes)
            {
                foreach (MethodInfo mInfo in type.GetMethods())
                {
                    Attribute attribute = FindTestAttribute(mInfo);
                    if (attribute != null)
                    {
                        string testName = mInfo.Name;
                        string message = "";

                        RunMethodsWithAnnotationBefore(assembly, type); //что это

                        PrintInformationAboutTests print = new PrintInformationAboutTests();

                        TestAttribute a = (TestAttribute)attribute;
                        if (a.MessageAboutIgnoreThisTest != "") // если есть аргумерт Ignore
                        {
                            message = "was not done. " + a.MessageAboutIgnoreThisTest;
                            print.PrintInformation(default(TimeSpan), testName, message);
                            //дальше выполнять код не надо
                        }
                         
                        if (message == "")
                        { 
                            var watch = new Stopwatch();
                            watch.Start();
                            try
                            {
                                
                                Object run = Activator.CreateInstance(type);
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
                        RunMethodsWithAnnotationAfter(assembly, type);
                    }
                    RunMethodsWithAnnotationAfterClass(assembly, type);
                }
            }
            return "0";
        }

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

        public void RunMethodsWithAnnotationBefore(Assembly assembly, Type type)
        {
            var allTypes = assembly.GetTypes();
            foreach (MethodInfo mInfo in type.GetMethods())
            {
                if (Attribute.GetCustomAttributes(mInfo).GetType() == typeof(BeforeAttribute))
                {
                    Object run = Activator.CreateInstance(type);
                    mInfo.Invoke(run, Array.Empty<Object>());
                }
            }
        }

        public void RunMethodsWithAnnotationAfter(Assembly assembly, Type type)
        {
            var allTypes = assembly.GetTypes();
            foreach (MethodInfo mInfo in type.GetMethods())
            {
                if (Attribute.GetCustomAttributes(mInfo).GetType() == typeof(AfterAttribute))
                {
                    Object run = Activator.CreateInstance(type);
                    mInfo.Invoke(run, Array.Empty<Object>());
                }
            }
        }

        public void RunMethodsWithAnnotationBeforeClass(Assembly assembly, Type type)
        {
            var allTypes = assembly.GetTypes();
            foreach (MethodInfo mInfo in type.GetMethods())
            {
                if (Attribute.GetCustomAttributes(mInfo).GetType() == typeof(BeforeClassAttribute))
                {
                    Object run = Activator.CreateInstance(type);
                    mInfo.Invoke(run, Array.Empty<Object>());
                }
            }
        }

        public void RunMethodsWithAnnotationAfterClass(Assembly assembly, Type type)
        {
            var allTypes = assembly.GetTypes();
            foreach (MethodInfo mInfo in type.GetMethods())
            {
                if (Attribute.GetCustomAttributes(mInfo).GetType() == typeof(AfterClassAttribute))
                {
                    Object run = Activator.CreateInstance(type);
                    mInfo.Invoke(run, Array.Empty<Object>());
                }
            }
        }
    }
}

/*
 Обработка метода после его получения:
 1) Посмотреть есть ли у этого метода TestAttribute
 2) Если да, то запускаем таймер и смотрим на аргументы
 Если нет аргументов, то просто запускаем
 Если есть и это Ignore, то игнорирует тест и выводит сообщение о причине
 Если вылетело исключение, то смотрим, соответствует ли оно нашему Excepted - если да, то все ок, если нет, то выводим excepted
 * */