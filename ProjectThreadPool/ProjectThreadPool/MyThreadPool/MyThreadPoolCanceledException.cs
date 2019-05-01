using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
