using System;

namespace Lazy
{
    /// <summary>
    /// Класс, который реализует многопоточный Lazy.
    /// </summary>
    public class MultiThreadedLazy<T> : ILazy<T>
    {
        private readonly object lockObject = new object();
        private volatile bool isEvaluated = false;
        private Func<T> supplier;
        private T result;

        public MultiThreadedLazy(Func<T> supplier)
            => this.supplier = supplier;

        public T Get()
        {
            if (!this.isEvaluated)
            {
                lock (this.lockObject)
                {
                    if (!this.isEvaluated)
                    {
                        this.result = this.supplier();
                        this.isEvaluated = true;
                        this.supplier = null;
                    }
                }
            }
            return this.result;
        }
    }
}
