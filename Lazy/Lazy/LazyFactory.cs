using System;

namespace Lazy
{
    /// <summary>
    /// Класс, который создает однопоточный и многопоточный Lazy.
    /// </summary>
    public static class LazyFactory
    {
        /// <summary>
        /// Метод, который реализует создание однопоточного Lazy.
        /// </summary>
        public static ILazy<T> CreateSingleThreadedLazy<T>(Func<T> supplier)
            => new Lazy<T>(supplier);

        /// <summary>
        /// Метод, который реализует создание многопоточного Lazy.
        /// </summary>
        public static ILazy<T> CreateMultiThreadedLazy<T>(Func<T> supplier)
            => new MultiThreadedLazy<T>(supplier);
    }
}
