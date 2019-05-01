using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MyNUnit
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Right a parth to assembly...");
            string parth = Console.ReadLine();
            string messageAboutWorkingAssemblies = "";
            var listOfAssemblies = new List<Assembly>();
            GetAllAssemblies getter = new GetAllAssemblies();
            (listOfAssemblies, messageAboutWorkingAssemblies) = getter.GetAll(parth);
            if (messageAboutWorkingAssemblies == "Все в порядке, ошибок нет!")
            {
                Console.WriteLine("Assemblies are ready, tests begining to work.");
                RunTestsInAssembly run = new RunTestsInAssembly();

                for (int i = 0; i < listOfAssemblies.Count; ++i)
                {
                    //Console.WriteLine("Assembly " + listOfAssemblies[i].GetName(false) + " " + " is watching now.");
                    //if (listOfAssemblies[i].GetName(false).ToString().Contains("ComparatorTests"))
                    run.RunTests(listOfAssemblies[i]);
                }
            }
            else
            {
                Console.WriteLine(messageAboutWorkingAssemblies);
                Console.ReadLine();
            }
            Console.ReadLine();
        }
    }
}

// найти сборки, запустить тесты в сборках
// информацию вывести на экран

// получить информацию о сборке с помощью рефлексии
// найти методы, помеченные анотацией Test, Expected и Ignore
// параллельно запустить тесты

// обработать тест
// получить результат работы теста

// написать тесты на систему тестирования



// ?? что такое методы before и after
