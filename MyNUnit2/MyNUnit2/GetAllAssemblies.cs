using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MyNUnit
{
    class GetAllAssemblies
    {
        /// <summary>
        /// Метод, который извлекает из сборки все exe и dll файлы.
        /// </summary>
        /// <param name="parth"></param>
        public (List<Assembly>, string) GetAll(string parth)
        {
            var arrayOfFiles = new string[100];
            var listOfAssemblies = new List<Assembly>();
            if (Directory.Exists(parth))
            {
                arrayOfFiles = Directory.GetFiles(parth, "*.exe", SearchOption.AllDirectories)
                    .Concat(Directory.GetFiles(parth, "*.dll", SearchOption.AllDirectories)).ToArray();
                /*for(int i = 0; i < arrayOfFiles.Length; ++i)
                {
                    Console.WriteLine(arrayOfFiles[i]);
                }
                */
                try
                {
                    for (int i = 0; i < arrayOfFiles.Length; ++i)
                    {
                        listOfAssemblies.Add(Assembly.LoadFrom(arrayOfFiles[i]));
                    }
                    return (listOfAssemblies, "Все в порядке, ошибок нет!");
                }
                catch (Exception ex)
                {
                    return (listOfAssemblies, "Произошла ошибка в загрузке сборки" + ex);
                }
            }
            return (listOfAssemblies, "Указанного пути не существует.");
        }
    }
}
