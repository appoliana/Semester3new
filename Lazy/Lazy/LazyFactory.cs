using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lazy
{
    class LazyFactory
    {
        public static ILazy<T> CreateSingleThreadedLazy<T>(Func<T> supplier)
            => new Lazy<T>(supplier);

        public static ILazy<T> CreateMultiThreadedLazy<T>(Func<T> supplier)
            => new MultiThreadedLazy<T>(supplier);
    }
}
