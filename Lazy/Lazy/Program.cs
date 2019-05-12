using System;
using System.Threading;

namespace Lazy
{
    class Program
    {
        public static void Main(string[] args)
        {
            Func<int> createInteger = () => 3;
            Func<string> createString = () => "hello";

            var simpleLazyObject = LazyFactory.CreateSingleThreadedLazy(createInteger);
            Console.WriteLine(simpleLazyObject.Get());

            var lazyObject = LazyFactory.CreateMultiThreadedLazy(createString);
            var threads = new Thread[5];
            for (int i = 0; i < threads.Length; ++i)
            {
                threads[i] = new Thread(() => Console.WriteLine(lazyObject.Get()));
                threads[i].Start();
            }
        }
    }
}
