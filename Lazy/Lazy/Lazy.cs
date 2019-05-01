using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lazy
{
    public class Lazy<T> : ILazy<T>
    {
        private bool isEvaluated = false;
        private Func<T> supplier;
        private T result;

        public Lazy(Func<T> supplier)
            => this.supplier = supplier;

        public T Get()
        {
            if (!this.isEvaluated)
            {
                this.result = this.supplier();
                this.isEvaluated = true;
                this.supplier = null;

            }

            return this.result;
        }
    }
}
