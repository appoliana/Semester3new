using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MyNUnit
{
    /// <summary>
    /// Класс, который извлекает exe и dll файлы.
    /// </summary>
    public class GetAllAssemblies
    {
        /// <summary>
        /// Метод, который извлекает exe и dll файлы.
        /// </summary>
        /// <param name="path"></param>
        public (List<Assembly>, string) GetAll(string path)
        {
            var arrayOfFiles = new string[100];
            var listOfAssemblies = new List<Assembly>();
            if (Directory.Exists(path))
            {
                arrayOfFiles = Directory.GetFiles(path, "*.exe", SearchOption.AllDirectories)
                    .Concat(Directory.GetFiles(path, "*.dll", SearchOption.AllDirectories)).ToArray();
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
