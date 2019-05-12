using System;

namespace Lazy
{
    /// <summary>
    /// Интерфейс, который реализует Lazy.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface ILazy<T>
    {
        T Get();
    }
}
