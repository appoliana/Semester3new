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
            Console.WriteLine("Write a parth to assembly...");
            string parth = Console.ReadLine();
            string messageAboutWorkingAssemblies = "";
            var listOfAssemblies = new List<Assembly>();
            var getter = new GetAllAssemblies();
            (listOfAssemblies, messageAboutWorkingAssemblies) = getter.GetAll(parth);
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
            var run = new RunTestsInAssembly();
            run.RunTests(assembly);
        }
    }
}
