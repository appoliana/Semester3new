using System;

namespace MyThreadPool
{
    public class MyThreadPoolCanceledException : Exception
    {
        public MyThreadPoolCanceledException(string msg = "MyThreadPool is canceled")
            : base(msg)
        {

        }
    }
}
