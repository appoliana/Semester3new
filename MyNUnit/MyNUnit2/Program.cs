using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;

namespace MyNUnit
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Write a path to assembly...");
            string path = Console.ReadLine();
            var (listOfAssemblies, messageAboutWorkingAssemblies) = AssembliesGetter.GetAll(path);
            if (messageAboutWorkingAssemblies == "Все в порядке, ошибок нет!")
            {
                Console.WriteLine("Assemblies are ready, tests begining to work.");
                Parallel.ForEach(listOfAssemblies, RunAssemblies);
            }
            else
            {
                Console.WriteLine(messageAboutWorkingAssemblies);
                Console.ReadLine();
            }
            Console.ReadLine();
        }

        /// <summary>
        /// Метод, который запускает указанную сборку.
        /// </summary>
        public static void RunAssemblies(Assembly assembly)
        {
            var run = RunTestsInAssembly.RunTests(assembly);
        }
    }
}
