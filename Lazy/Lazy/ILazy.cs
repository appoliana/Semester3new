using System;

namespace Lazy
{
    /// <summary>
    /// Интерфейс, который реализует Lazy.
    /// </summary>
    public interface ILazy<T>
    {
        T Get();
    }
}
