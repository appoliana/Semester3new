using System;
using System.Collections.Generic;
using System.Reflection;

namespace CompClass
{
    /// <summary>
    /// Класс, который решает поставленную задачу.
    /// </summary>
    public class ClassComparator
    {
        /// <summary>
        /// Конструктор, в котором инициализируются списки.
        /// </summary>
        public ClassComparator()
        {
            firstMethods = new List<string>();
            secondMethods = new List<string>();
            firstFields = new List<string>();
            secondFields = new List<string>();
        }

        /// <summary>
        /// Метод, который берет классы на сравнение.
        /// </summary>
        /// <param name="firstClass"></param>
        /// <param name="secondClass"></param>
        public void GetClasses(Type firstClass, Type secondClass)
        {
            firstCl = firstClass;
            secondCl = secondClass;
        }

        /// <summary>
        /// Метод, который все методы класса записывает в список.
        /// </summary>
        public void GetMethods()
        {
            foreach (var method in firstCl.GetMethods())
            {
                if (method is MethodInfo)
                    firstMethods.Add(method.ToString());
            }

            foreach (var method in secondCl.GetMethods())
            {
                if (method is MethodInfo)
                    secondMethods.Add(method.ToString());
            }
        }

        /// <summary>
        /// Метод, который все поля классов записывает в списки.
        /// </summary>
        public void GetFields()
        {
            foreach (var field in firstCl.GetFields())
            {
                if (field is FieldInfo)
                    firstFields.Add(field.ToString());
            }

            foreach (var field in secondCl.GetFields())
            {
                if (field is FieldInfo)
                    secondFields.Add(field.ToString());
            }
        }

        /// <summary>
        /// Метод, который печатает разные поля и методы.
        /// </summary>
        public void PrintDiffMethodsAndFields()
        {
            Console.WriteLine("Different Fields:");
            GetDiffFields();
            Console.WriteLine("Different Methods:");
            GetDiffMethods();
        }

        /// <summary>
        /// Метод для выявления различных методов и полей в классах.
        /// </summary>
        public List<string> GetDiffMethods()
        {
            var resultListWithMethods1 = new List<string>();
            GetMethods();
            foreach (var m in firstMethods)
            {
                if (!secondMethods.Contains(m))
                {
                    resultListWithMethods1.Add(m);
                }
                //Console.WriteLine(m);
            }

            foreach (var m in secondMethods)
            {
                if (!firstMethods.Contains(m) && !resultListWithMethods1.Contains(m))
                {
                    resultListWithMethods1.Add(m);
                }
            }
            return resultListWithMethods1;
        }

        /// <summary>
        /// Метод для печати различных полей сравнивымаемых классов.
        /// </summary>
        public List<string> GetDiffFields()
        {
            var resultListWithFields1 = new List<string>();
            GetFields();
            foreach (var f in firstFields)
            {
                if (!secondFields.Contains(f))
                {
                    resultListWithFields1.Add(f);
                }
            }

            foreach (var f in secondFields)
            {
                if (!firstFields.Contains(f))
                {
                    resultListWithFields1.Add(f);
                }
            }
            return resultListWithFields1;
        }

        public Type firstCl;
        public List<string> firstMethods;
        public List<string> firstFields;

        public Type secondCl;
        public List<string> secondMethods;
        public List<string> secondFields;
    }
}
